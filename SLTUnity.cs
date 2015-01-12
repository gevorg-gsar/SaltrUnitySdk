using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Saltr.UnitySdk.Repository;
using Saltr.UnitySdk.Resource;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Status;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk
{
    //TODO:: @daal add some flushCache method.
    /// <summary>
    /// The entry point to SDK, and the main class, used to send and receive data from Saltr.
    /// </summary>
    public class SLTUnity // TODO @gyln: reorder memebrs as is in as3 sdk
    {
        #region Constants

        public const string CLIENT = "Unity";
        public const string API_VERSION = "1.0.0"; //"0.9.0";
        /// <summary>
        /// The name of the Saltr GameObject, through which you can access SLTUnity instance.
        /// </summary>
        public const string SALTR_GAME_OBJECT_NAME = "Saltr";

        #endregion Constants

        #region Fields

        private string _socialId;
        private string _deviceId;
        private string _clientKey;
        private bool _isLoading;
        private bool _isConected;

        private bool _isDevMode;
        private bool _autoRegisterDevice;
        private bool _isStarted;
        private bool _isSynced;
        private bool _useNoLevels;
        private bool _useNoFeatures;
        private int _requestIdleTimeout;
        private SLTLevelType _levelType;

        private SaltrWrapper _wrapper;
        private ISLTRepository _repository;

        private Dictionary<string, SLTFeature> _activeFeatures;
        private Dictionary<string, SLTFeature> _developerFeatures;

        private List<SLTExperiment> _experiments = new List<SLTExperiment>();
        private List<SLTLevelPack> _levelPacks = new List<SLTLevelPack>();

        private Action _connectSuccessCallback;
        private Action<SLTStatus> _connectFailCallback;
        private Action _levelContentLoadSuccessCallback;
        private Action<SLTStatus> _levelContentLoadFailCallback;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Sets the social identifier of the user.
        /// </summary>
        public string SocialId
        {
            set { _socialId = value; }
        }

        /// <summary>
        /// Sets the repository used by this instance. An appropriate repository is already set by a constructor,
        /// so you will need this only if you want to implement and use your own custom repository (<see cref="saltr.Repository.ISLTRepository"/>).
        /// </summary>        
        internal ISLTRepository Repository
        {
            set { _repository = value; }
        }

        /// <summary>
        /// Sets a value indicating whether the application does not use features.
        /// </summary>
        /// <value><c>true</c> if features are not used; otherwise, <c>false</c>.</value>
        public bool UseNoFeatures
        {
            set { _useNoFeatures = value; }
        }

        /// <summary>
        /// Sets a value indicating whether the application does not use levels.
        /// </summary>
        /// <value><c>true</c> if levels are not used; otherwise, <c>false</c>.</value>
        public bool UseNoLevels
        {
            set { _useNoLevels = value; }
        }

        /// <summary>
        /// Sets a value indicating weather this <see cref="saltr.SLTUnity"/> should operate in dev(developer) mode.
        /// In this mode client data(e.g. developer defined features) will be synced with Saltr, once, after successful <see cref="saltr.SLTUnity.Connect"/> call.
        /// </summary>
        /// <value><c>true</c> to enable; <c>false</c> to disable.</value>
        public bool DevMode
        {
            set { _isDevMode = value; }
        }

        /// <summary>
        /// Sets a value indicating whether device registratioon dialog should be automaticaly shown in dev mode(<see cref="saltr.SLTUnity.DevMode"/>),
        /// after successful <see cref="saltr.SLTUnity.connect"/> call, if the device was not registered already.
        /// </summary>
        /// <value><c>true</c> to enable; <c>false</c> to disable. By default is set to <c>true</c></value>
        public bool AutoRegisterDevice
        {
            set { _autoRegisterDevice = value; }
        }

        /// <summary>
        /// Sets the request idle timeout. If a URL request takes more than timeout to complete, it would be canceled.
        /// </summary>
        /// <value>The request idle timeout in milliseconds.</value>
        public int RequestIdleTimeout
        {
            set { _requestIdleTimeout = value; }
        }

        /// <summary>
        /// Gets the level packs.
        /// </summary>
        public List<SLTLevelPack> LevelPacks
        {
            get { return _levelPacks; }
        }

        /// <summary>
        /// Gets a list all levels.
        /// </summary>
        public List<SLTLevel> AllLevels
        {
            get
            {
                List<SLTLevel> allLevels = new List<SLTLevel>();
                for (int i = 0; i < _levelPacks.Count; i++)
                {
                    List<SLTLevel> packLevels = _levelPacks[i].Levels;
                    for (int j = 0; j < packLevels.Count; j++)
                    {
                        allLevels.Add(packLevels[j]);
                    }
                }
                return allLevels;
            }
        }

        /// <summary>
        /// Gets the count of all levels.
        /// </summary>
        public uint AllLevelsCount
        {
            get
            {
                uint count = 0;
                for (int i = 0; i < _levelPacks.Count; i++)
                {
                    count += (uint)_levelPacks[i].Levels.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Gets a list of all experiments.
        /// </summary>
        public List<SLTExperiment> Experiments
        {
            get { return _experiments; }
        }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="saltr.SLTUnity"/> class.
        /// </summary>
        /// <param name="clientKey">Client key.</param>
        /// <param name="DeviceId">Device identifier.</param>
        /// <param name="useCache">If set to <c>true</c> use cache. If not specified defaults to <c>true</c></param>
        public SLTUnity(string clientKey, string deviceId, bool useCache)
        {
            Init(clientKey, deviceId, useCache);
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.SLTUnity"/>.
        /// </summary>
        public SLTUnity(string clientKey, string deviceId)
        {
            Init(clientKey, deviceId, true);
        }

        #endregion Ctor

        #region Static Methods

        private static SLTResourceTicket GetTicket(string url, Dictionary<string, string> urlVars, int timeout)
        {
            SLTResourceTicket ticket = new SLTResourceTicket(url, urlVars);
            //ticket.method = "post"; // to implement
            if (timeout > 0)
            {
                ticket.DropTimeout = timeout;
            }

            return ticket;
        }

        private static SLTResourceTicket GetTicket(string url, Dictionary<string, string> urlVars)
        {
            return GetTicket(url, urlVars, 0);
        }

        #endregion Static Methods

        #region Internal Methods

        private void Init(string clientKey, string deviceId, bool useCache)
        {
            GameObject saltr = GameObject.Find(SALTR_GAME_OBJECT_NAME);
            if (saltr == null)
            {
                saltr = new GameObject();
                saltr.SetActive(false);
                saltr.name = SALTR_GAME_OBJECT_NAME;
                saltr.AddComponent<GETPOSTWrapper>();
                _wrapper = saltr.AddComponent<SaltrWrapper>();
                _wrapper.Saltr = this;
                saltr.SetActive(true);
            }
            else
            {
                _wrapper = saltr.GetComponent<SaltrWrapper>();
            }

            _clientKey = clientKey;
            _deviceId = deviceId;
            _isLoading = false;
            _isConected = false;
            _useNoLevels = false;
            _useNoFeatures = false;
            _levelType = SLTLevelType.Unknown;

            _isDevMode = false;
            _autoRegisterDevice = true;
            _isStarted = false;
            _requestIdleTimeout = 0;

            _activeFeatures = new Dictionary<string, SLTFeature>();
            _developerFeatures = new Dictionary<string, SLTFeature>();
            _experiments = new List<SLTExperiment>();
            _levelPacks = new List<SLTLevelPack>();

            if (useCache)
                _repository = new SLTMobileRepository();
            else
                _repository = new SLTDummyRepository();
        }

        private SLTResource CreateAppDataResource(Action<SLTResource> loadSuccessCallback, Action<SLTResource> loadFailCallback, SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();

            urlVars["cmd"] = SLTConfig.ActionGetAppData; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ActionGetAppData;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = API_VERSION;
            args.ClientKey = _clientKey;
            args.Client = CLIENT;

            if (_deviceId != null)
                args.DeviceId = _deviceId;
            else
                throw new Exception("Field 'deviceId' is required.");

            if (_socialId != null)
            {
                args.SocialId = _socialId;
            }

            if (basicProperties != null)
            {
                args.BasicProperties = basicProperties;
            }

            if (customProperties != null)
            {
                args.CustomProperties = customProperties;
            }

            urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            return new SLTResource("saltAppConfig", ticket, loadSuccessCallback, loadFailCallback);
        }


        private object LoadLevelContentFromDisk(SLTLevel level)
        {
            string url = Util.FormatString(SLTConfig.LocalLevelContentPackageUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromApplication(url);
        }

        private object LoadLevelContentFromCache(SLTLevel level)
        {

            string url = Util.FormatString(SLTConfig.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromCache(url);
        }

        private void LoadLevelContentFromSaltr(SLTLevel level)
        {
            string dataUrl = level.ContentUrl + "?_time_=" + DateTime.Now.ToShortTimeString();
            SLTResourceTicket ticket = GetTicket(dataUrl, null, _requestIdleTimeout);

            // some ugly stuff here came from AS3... you don't do like this in normal languages
            Action<SLTResource> loadFromSaltrSuccessCallback = delegate(SLTResource res)
            {
                object contentData = res.Data;
                if (contentData != null)
                {
                    CacheLevelContent(level, contentData);
                }
                else
                {
                    contentData = LoadLevelContentInternally(level);
                }
                if (contentData != null)
                {
                    LevelContentLoadSuccessHandler(level, contentData);
                }
                else
                {
                    LevelContentLoadFailHandler();
                }
                res.Dispose();
            };

            Action<SLTResource> loadFromSaltrFailCallback = delegate(SLTResource SLTResource)
            {
                object contentData = LoadLevelContentInternally(level);
                if (contentData != null)
                {
                    LevelContentLoadSuccessHandler(level, contentData);
                }
                else
                {
                    LevelContentLoadFailHandler();
                }
                SLTResource.Dispose();
            };


            SLTResource resource = new SLTResource("saltr", ticket, loadFromSaltrSuccessCallback, loadFromSaltrFailCallback);
            resource.Load();
        }

        private string GetCachedLevelVersion(SLTLevel level)
        {
            string cachedFileName = Util.FormatString(SLTConfig.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectVersion(cachedFileName);
        }

        private void LevelContentLoadSuccessHandler(SLTLevel level, object content)
        {
            level.UpdateContent(content.ToDictionaryOrNull());
            _levelContentLoadSuccessCallback();
        }

        private object LoadLevelContentInternally(SLTLevel level)
        {
            object contentData = LoadLevelContentFromCache(level);
            if (contentData == null)
                contentData = LoadLevelContentFromDisk(level);
            return contentData;
        }

        private void AppDataLoadSuccessCallback(SLTResource resource)
        {
            Dictionary<string, object> data = resource.Data;

            if (data == null)
            {
                _connectFailCallback(new SLTStatusAppDataLoadFail());
                resource.Dispose();
                return;
            }

            bool success = false;
            Dictionary<string, object> response = new Dictionary<string, object>();

            if (data.ContainsKey("response"))
            {
                IEnumerable<object> res = (IEnumerable<object>)data["response"];
                response = res.FirstOrDefault().ToDictionaryOrNull();
                success = (bool)response["success"]; //.ToString().ToLower() == "true";
            }
            else
            {
                //TODO @GSAR: remove later when API is versioned!
                if (data.ContainsKey("responseData"))
                    response = data["responseData"].ToDictionaryOrNull();

                success = (data.ContainsKey("status") && data["status"].ToString() == SLTConfig.ResultSuccess);
            }

            _isLoading = false;

            if (success)
            {
                if (_isDevMode && !_isSynced)
                {
                    Sync();
                }

                if (response.ContainsKey("levelType"))
                {
                    _levelType = (SLTLevelType)Enum.Parse(typeof(SLTLevelType), response["levelType"].ToString(), true);
                }

                Dictionary<string, SLTFeature> saltrFeatures = new Dictionary<string, SLTFeature>();
                try
                {
                    saltrFeatures = SLTDeserializer.DecodeFeatures(response);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    _connectFailCallback(new SLTStatusFeaturesParseError());
                    return;
                }

                try
                {
                    _experiments = SLTDeserializer.DecodeExperiments(response);
                }
                catch
                {
                    _connectFailCallback(new SLTStatusExperimentsParseError());
                    return;
                }

                // if developer didn't announce use without levels, and levelType in returned JSON is not "noLevels",
                // then - parse levels
                if (!_useNoLevels && _levelType != SLTLevelType.NoLevels)
                {
                    List<SLTLevelPack> newLevelPacks = null;
                    try
                    {
                        newLevelPacks = SLTDeserializer.DecodeLevels(response);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                        _connectFailCallback(new SLTStatusLevelsParseError());
                        return;
                    }

                    // if new levels are received and parsed, then only dispose old ones and assign new ones.
                    if (newLevelPacks != null)
                    {
                        DisposeLevelPacks();
                        _levelPacks = newLevelPacks;
                    }
                }

                _isConected = true;
                _repository.CacheObject(SLTConfig.AppDataUrlCache, "0", response);

                _activeFeatures = saltrFeatures;
                _connectSuccessCallback();

                Debug.Log("[SALTR] AppData load success. LevelPacks loaded: " + _levelPacks.Count);
                //TODO @GSAR: later we need to report the feature set differences by an event or a callback to client;
            }
            else
            {
                if (response.ContainsKey("error"))
                {
                    _connectFailCallback(new SLTStatus(int.Parse(response.GetValue<Dictionary<string, object>>("error").GetValue<string>("code")), response.GetValue<Dictionary<string, object>>("error").GetValue<string>("message")));
                }
                else
                {
                    _connectFailCallback(new SLTStatus(response.GetValue<string>("errorCode").ToIntegerOrZero(), response.GetValue<string>("errorMessage")));
                }

            }
            resource.Dispose();
        }

        private void DisposeLevelPacks()
        {
            for (int i = 0; i < _levelPacks.Count; ++i)
            {
                _levelPacks[i].Dispose();
            }
            _levelPacks.Clear();
        }

        private void Sync()
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["cmd"] = SLTConfig.ActionDevSyncData; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ActionDevSyncData;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = API_VERSION;
            args.ClientKey = _clientKey;
            args.DevMode = _isDevMode;
            urlVars["devMode"] = _isDevMode.ToString();
            args.Client = CLIENT;

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
                urlVars["deviceId"] = _deviceId;
            }
            else
                throw new Exception("Field 'deviceId' is required.");

            if (_socialId != null)
            {
                args.SocialId = _socialId;
            }

            List<object> featureList = new List<object>();
            foreach (SLTFeature feature in _developerFeatures.Values)
            {
                Dictionary<string, object> featureDictionary = feature.ToDictionary();
                featureDictionary.RemoveEmptyOrNull();
                featureList.Add(featureDictionary);
            }

            args.DeveloperFeatures = featureList;
            args.RawData.RemoveEmptyOrNull();
            urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConfig.SALTR_DEVAPI_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("syncFeatures", ticket, SyncSuccessHandler, SyncFailHandler);
            resource.Load();
        }

        private void AddDeviceToSALTR(string email)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["action"] = SLTConfig.ActionDevRegisterDevice;
            urlVars["clientKey"] = _clientKey;

            SLTRequestArguments args = new SLTRequestArguments();
            args.DevMode = _isDevMode;
            args.ApiVersion = API_VERSION;


            if (_deviceId != null)
            {
                args.Id = _deviceId;
            }
            else
            {
                throw new Exception("Field 'deviceId' is required");
            }

            string model = "Unknown";
            string os = "Unknown";
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    model = Util.GetHumanReadableDeviceModel(SystemInfo.deviceModel);
                    os = SystemInfo.operatingSystem.Replace("Phone ", "");
                    break;
                case RuntimePlatform.Android:
                    model = SystemInfo.deviceModel;
                    int iVersionNumber = 0;
                    string androidVersion = SystemInfo.operatingSystem;
                    int sdkPos = androidVersion.IndexOf("API-");
                    iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2).ToString());
                    os = "Android " + iVersionNumber;
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    model = "PC";
                    os = SystemInfo.operatingSystem;
                    break;
                default:
                    model = SystemInfo.deviceModel;
                    os = SystemInfo.operatingSystem;
                    break;
            }

            args.Source = model;
            args.OS = os;

            if (email != null && email != "")
            {
                args.Email = email;
            }
            else
            {
                throw new Exception("Email is required.");
            }

            urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConfig.SALTR_DEVAPI_URL, urlVars);
            SLTResource resource = new SLTResource("addDevice", ticket, AddDeviceSuccessHandler, AddDeviceFailHandler);
            resource.Load();
        }

        private void CacheLevelContent(SLTLevel level, object content)
        {
            string cachedFileName = Util.FormatString(SLTConfig.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            _repository.CacheObject(cachedFileName, level.Version.ToString(), content);
        }

        #endregion Internal Methods

        #region Handlers

        private void SyncSuccessHandler(SLTResource resource)
        {
            object data = resource.Data;

            if (data == null)
            {
                Debug.Log("[Saltr] Dev feature Sync's data is null.");
                return;
            }

            IEnumerable<object> response = (IEnumerable<object>)data.ToDictionaryOrNull().GetValue("response");
            if (response == null)
            {
                Debug.Log("[Saltr] Dev feature Sync's response is null.");
                return;
            }

            if (response.Count() <= 0)
            {
                Debug.Log("[Saltr] Dev feature Sync response's length is <= 0.");
                return;
            }

            Dictionary<string, object> responseObject = response.ElementAt(0).ToDictionaryOrNull();

            if ((bool)responseObject.GetValue("success") == false)
            {
                if ((SLTStatusCode)responseObject.GetValue("error").ToDictionaryOrNull().GetValue("code").ToIntegerOrZero() == SLTStatusCode.RegistrationRequired && _autoRegisterDevice)
                {
                    RegisterDevice();
                }
                Debug.Log("[Saltr] Sync error: " + responseObject.GetValue("error").ToDictionaryOrNull().GetValue<string>("message"));
            }
            else
            {
                Debug.Log("[Saltr] Dev feature Sync is complete.");
                _isSynced = true;
            }
        }

        private void SyncFailHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev feature Sync has failed.");
        }

        private void AddDeviceSuccessHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev adding new device is complete.");
            Dictionary<string, object> data = resource.Data.ToDictionaryOrNull();
            bool success = false;
            Dictionary<string, object> response;
            if (data.ContainsKey("response"))
            {
                response = ((IEnumerable<object>)(data["response"])).ElementAt(0).ToDictionaryOrNull();
                success = (bool)response.GetValue("success");
                if (success)
                {
                    _wrapper.SetStatus("Success");
                    Sync();
                }
                else
                {
                    _wrapper.SetStatus(response.GetValue("error").ToDictionaryOrNull().GetValue<string>("message"));
                }
            }
            else
            {
                AddDeviceFailHandler(resource);
            }
        }

        private void AddDeviceFailHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev adding new device has failed.");
            _wrapper.SetStatus("Failed");
        }

        private void AppDataLoadFailCallback(SLTResource resource)
        {
            resource.Dispose();
            _isLoading = false;
            _connectFailCallback(new SLTStatusAppDataLoadFail());
        }

        private void LevelContentLoadFailHandler()
        {
            _levelContentLoadFailCallback(new SLTStatusLevelContentLoadFail());
        }

        #endregion Handlers

        #region Business Methods


        /// <summary>
        /// Gets a level by its global index in all levels.
        /// </summary>
        /// <param name="index">Index in all levels.</param>
        public SLTLevel GetLevelByGlobalIndex(int index)
        {
            int levelSum = 0;
            for (int i = 0; i < _levelPacks.Count; i++)
            {
                int packLenght = _levelPacks[i].Levels.Count;
                if (index >= levelSum + packLenght)
                {
                    levelSum += packLenght;
                }
                else
                {
                    int localIndex = index - levelSum;
                    return _levelPacks[i].Levels[localIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the level pack that contains the level with given global index in all levels.
        /// </summary>
        /// <param name="index">Index of the level in all levels.</param>
        public SLTLevelPack GetPackByLevelGlobalIndex(int index)
        {
            int levelSum = 0;
            for (int i = 0; i < _levelPacks.Count; i++)
            {
                int packLenght = _levelPacks[i].Levels.Count;
                if (index >= levelSum + packLenght)
                {
                    levelSum += packLenght;
                }

                else
                {
                    return _levelPacks[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a list of tokens(unique identifiers) of all features, active in Saltr.
        /// </summary>
        public List<string> GetActiveFeatureTokens()
        {
            List<string> tokens = new List<string>();
            foreach (SLTFeature feature in _activeFeatures.Values)
            {
                if (feature != null && feature.Token != null)
                {
                    tokens.Add(feature.Token);
                }
            }
            return tokens;
        }

        /// <summary>
        /// Gets the properties of the feature specified by the token. 
        /// If a feature is set to be required and is not active in, or cannot be retrieved from, Saltr, 
        /// the properties will be retrieved from default developer defined features.
        /// </summary>
        /// <param name="token">The feature token.</param>
        public Dictionary<string, object> GetFeatureProperties(string token)
        {
            if (_activeFeatures.ContainsKey(token))
            {
                return (_activeFeatures[token]).Properties.ToDictionaryOrNull();
            }
            else
                if (_developerFeatures.ContainsKey(token))
                {
                    SLTFeature devFeature = _developerFeatures[token];
                    if (devFeature != null && devFeature.Required)
                    {
                        return devFeature.Properties.ToDictionaryOrNull();
                    }
                }

            return null;
        }

        /// <summary>
        /// Imports the level data from local files, that can be downloaded from Saltr. 
        /// If your application is using levels, this must be called before calling Start(), otherwise has no effect.
        /// </summary>
        /// <param name="path">
        /// The path to level packs in Resources folder. 
        /// If not specified the <see cref="saltr.SLTConfig.LocalLevelPackageUrl"/> will be used.
        /// </param>
        public void ImportLevels(string path)
        {
            if (_useNoLevels)
            {
                return;
            }

            if (_isStarted == false)
            {
                path = path == null ? SLTConfig.LocalLevelPackageUrl : path;
                object applicationData = _repository.GetObjectFromApplication(path);
                _levelPacks = SLTDeserializer.DecodeLevels(applicationData.ToDictionaryOrNull());
            }
            else
            {
                throw new Exception("Method 'importLevels()' should be called before 'Start()' only.");
            }
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.ImportLevels"/>.
        /// </summary>
        public void ImportLevels()
        {
            ImportLevels(null);
        }

        // If you want to have a feature synced with SALTR you should call define before getAppData call.
        /// <summary>
        /// Defines a feature (<see cref="saltr.SLTFeature"/>).
        /// </summary>
        /// <param name="token">Token - a unique identifier for the feature.</param>
        /// <param name="properties">A dictionary of properties, that should be of "JSON friendly" datatypes 
        /// (string, int, double, Dictionary, List, etc.). To represent color use standard HTML format: <c>"#RRGGBB"</c> </param>
        /// <param name="required">If set to <c>true</c> feature is required(see <see cref="saltr.SLTUnity.GetFeatureProperties"/>). <c>false</c> by default.</param>
        public void DefineFeature(string token, Dictionary<string, object> properties, bool required)
        {
            if (_useNoFeatures)
            {
                return;
            }

            if (_isStarted == false)
            {
                _developerFeatures[token] = new SLTFeature(token, properties, required);
            }
            else
            {
                throw new Exception("Method 'defineFeature()' should be called before 'Start()' only.");
            }
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.DefineFeature"/>.
        /// </summary>
        public void DefineFeature(string token, Dictionary<string, object> properties)
        {
            DefineFeature(token, properties, false);
        }

        /// <summary>
        /// Checks if everything is initialized properly and starts the instance.
        /// After this call you can access application data (levels, features), and establish connection with the server.
        /// </summary>
        public void Start()
        {
            if (_deviceId == null)
            {
                throw new Exception("deviceId field is required and can't be null.");
            }

            if (_developerFeatures.Count == 0 && !_useNoFeatures)
            {
                throw new Exception("Features should be defined.");
            }

            if (_levelPacks.Count == 0 && !_useNoLevels)
            {
                throw new Exception("Levels should be imported.");
            }

            object cachedData = _repository.GetObjectFromCache(SLTConfig.AppDataUrlCache);
            if (cachedData == null)
            {
                foreach (var item in _developerFeatures.Keys)
                {
                    _activeFeatures[item] = _developerFeatures[item];
                }
            }
            else
            {
                if (cachedData != null)
                {
                    _activeFeatures = SLTDeserializer.DecodeFeatures(cachedData.ToDictionaryOrNull());
                    _experiments = SLTDeserializer.DecodeExperiments(cachedData.ToDictionaryOrNull());
                }
            }
            _isStarted = true;
        }

        /// <summary>
        /// Establishes connection to the server and updates application data(levels, features and experiments).
        /// After connecting successfully you can load level content from server with <see cref="saltr.SLTUnity.LoadLevelContent"/> .
        /// </summary>
        /// <param name="successCallback">Success callback.</param>
        /// <param name="failCallback">Fail callback, receives <see cref="saltr.Status.SLTStatus"/> object as the first parameter.</param>
        /// <param name="basicProperties">(Optional)Basic properties. Same as in <see cref="saltr.SLTUnity.AddProperties"/>.</param>
        /// <param name="customProperties">(Optional)Custom properties. Same as in <see cref="saltr.SLTUnity.AddProperties"/>.</param>
        public void Connect(Action successCallback, Action<SLTStatus> failCallback, SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            if (!_isStarted)
            {
                throw new Exception("Method 'connect()' should be called after 'Start()' only.");
            }

            if (_isLoading)
            {
                failCallback(new SLTStatusAppDataConcurrentLoadRefused());
                return;
            }

            _connectFailCallback = failCallback;
            _connectSuccessCallback = successCallback;

            _isLoading = true;
            SLTResource resource = CreateAppDataResource(AppDataLoadSuccessCallback, AppDataLoadFailCallback, basicProperties, customProperties);
            resource.Load();
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.Connect"/>.
        /// </summary>
        public void Connect(Action successCallback, Action<SLTStatus> failCallback, SLTBasicProperties basicProperties)
        {
            Connect(successCallback, failCallback, basicProperties, null);
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.Connect"/>.
        /// </summary>
        public void Connect(Action successCallback, Action<SLTStatus> failCallback)
        {
            Connect(successCallback, failCallback, null);
        }


        /// <summary>
        /// Loads the content(boards and properties) of the level.
        /// Contents may be loaded from server, cache, or local level data(<see cref="saltr.SLTUnity.ImportLevels"/>).
        /// </summary>
        /// <param name="SLTLevel">The level, contents of which will be updated.</param>
        /// <param name="loadSuccessCallback">Load success callback.</param>
        /// <param name="loadFailCallback">Load fail callback, receives <see cref="saltr.Status.SLTStatus"/> object as the first parameter.</param>
        /// <param name="useCache">If set to <c>false</c> cached level data will be ignored, forcing content to be loaded from server or local data if connection is not established. <c>true</c> by default. </param>
        public void LoadLevelContent(SLTLevel level, Action loadSuccessCallback, Action<SLTStatus> loadFailCallback, bool useCache)
        {
            _levelContentLoadSuccessCallback = loadSuccessCallback;
            _levelContentLoadFailCallback = loadFailCallback;
            object content = null;
            if (_isConected == false)
            {
                if (useCache)
                {
                    content = LoadLevelContentInternally(level);
                }
                else
                {
                    content = LoadLevelContentFromDisk(level);
                }

                LevelContentLoadSuccessHandler(level, content);
            }
            else
            {
                if (useCache == false || level.Version != GetCachedLevelVersion(level))
                {
                    LoadLevelContentFromSaltr(level);
                }
                else
                {
                    content = LoadLevelContentFromCache(level);
                    LevelContentLoadSuccessHandler(level, content);
                }
            }
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.LoadLevelContent"/>.
        /// </summary>
        public void LoadLevelContent(SLTLevel level, Action loadSuccessCallback, Action<SLTStatus> loadFailCallback)
        {
            LoadLevelContent(level, loadSuccessCallback, loadFailCallback, true);
        }

        /// <summary>
        /// Associates some properties with this client, that are used to assign it to a certain user group in Saltr.
        /// </summary>
        /// <param name="basicProperties">Basic properties. Standard set of client properties</param>
        /// <param name="customProperties">(Optional)Custom properties.</param>
        public void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            if (basicProperties == null && customProperties == null)
            {
                return;
            }

            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["cmd"] = SLTConfig.ActionAddProperties; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ActionAddProperties;

            SLTRequestArguments args = new SLTRequestArguments()
            {
                ApiVersion = API_VERSION,
                ClientKey = _clientKey,
                Client = CLIENT
            };

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
            }
            else
                throw new Exception("Field 'deviceId' is a required.");

            if (_socialId != null)
            {
                args.SocialId = _socialId;
            }

            if (basicProperties != null)
            {
                args.BasicProperties = basicProperties;
            }

            if (customProperties != null)
            {
                args.CustomProperties = customProperties;
            }

            Action<SLTResource> propertyAddSuccess = delegate(SLTResource res)
            {
                Debug.Log("success");
                Dictionary<string, object> data = res.Data;
                res.Dispose();
            };

            Action<SLTResource> propertyAddFail = delegate(SLTResource res)
            {
                Debug.Log("error");
                res.Dispose();
            };

            urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("property", ticket, propertyAddSuccess, propertyAddFail);
            resource.Load();
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.AddProperties"/>.
        /// </summary>
        public void AddProperties(SLTBasicProperties basicProperties)
        {
            AddProperties(basicProperties, null);
        }

        /// <summary>
        /// Opens the device registration dialog. Can be called after <see cref="saltr.SLTUnity.Start"/> only.
        /// </summary>
        public void RegisterDevice()
        {
            if (!_isStarted)
            {
                throw new Exception("Method 'registerDevice()' should be called after 'Start()' only.");
            }
            _wrapper.ShowDeviceRegistationDialog(AddDeviceToSALTR);
        }

        #endregion Public Methods

    }
}

 