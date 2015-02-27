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
using GAFEditor.Utils;

namespace Saltr.UnitySdk
{
    //TODO:: @daal add some flushCache method.
    /// <summary>
    /// The entry point to SDK, and the main class, used to send and receive data from Saltr.
    /// </summary>
    public class SLTUnity
    {
        #region Constants

        /// <summary>
        /// The name of the Saltr GameObject, through which you can access SLTUnity instance.
        /// </summary>
        public const string SALTR_GAME_OBJECT_NAME = "Saltr";
        public const string CLIENT = "Unity";
        public const string API_VERSION = "1.0.0"; //"0.9.0";

        #endregion Constants

        #region Fields

        private string _socialId;
        private string _deviceId;
        private string _clientKey;
        private bool _isLoading;
        private bool _isConected;

        private bool _isDevMode;
        private bool _isStarted;
        private bool _isSynced;
        private bool _useNoLevels;
        private bool _useNoFeatures;
        private bool _isAutoRegisteredDevice;

        private int _requestIdleTimeout;

        private SLTLevelType _levelType;
        private SaltrWrapper _wrapper;
        private ISLTRepository _repository;

        private Dictionary<string, SLTFeature> _activeFeatures;
        private Dictionary<string, SLTFeature> _developerFeatures;

        private List<SLTExperiment> _experiments = new List<SLTExperiment>();
        private List<SLTLevelPack> _levelPacks = new List<SLTLevelPack>();

        private Action _onConnectSuccess;
        private Action<SLTStatus> _onConnectFail;
        private Action _onLevelContentLoadSuccess;
        private Action<SLTStatus> _onLevelContentLoadFail;

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
        public bool IsDevMode
        {
            set { _isDevMode = value; }
        }

        /// <summary>
        /// Sets a value indicating whether device registratioon dialog should be automaticaly shown in dev mode(<see cref="saltr.SLTUnity.IsDevMode"/>),
        /// after successful <see cref="saltr.SLTUnity.connect"/> call, if the device was not registered already.
        /// </summary>
        /// <value><c>true</c> to enable; <c>false</c> to disable. By default is set to <c>true</c></value>
        public bool IsAutoRegisteredDevice
        {
            set { _isAutoRegisteredDevice = value; }
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
        /// Sets the repository used by this instance. An appropriate repository is already set by a constructor,
        /// so you will need this only if you want to implement and use your own custom repository (<see cref="saltr.Repository.ISLTRepository"/>).
        /// </summary>        
        internal ISLTRepository Repository
        {
            set { _repository = value; }
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
        /// <param name="_clientKey">Client key.</param>
        /// <param name="DeviceId">Device identifier.</param>
        /// <param name="_useCache">If set to <c>true</c> use cache. If not specified defaults to <c>true</c></param>
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
                saltr.AddComponent<NetworkWrapper>();
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

            _isDevMode = false;
            _isStarted = false;
            _useNoLevels = false;
            _useNoFeatures = false;
            _isAutoRegisteredDevice = true;
            _requestIdleTimeout = 0;

            _levelType = SLTLevelType.Unknown;

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

            urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionGetAppData; //TODO @GSAR: remove later
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionGetAppData;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = API_VERSION;
            args.ClientKey = _clientKey;
            args.Client = CLIENT;

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
            }
            else
            { 
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

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

            urlVars[SLTConstants.UrlParamArguments] = Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConstants.SALTR_API_URL, urlVars, _requestIdleTimeout);
            return new SLTResource(SLTConstants.ResourceIdSaltrAppConfig, ticket, loadSuccessCallback, loadFailCallback);
        }

        private object LoadLevelContentInternally(SLTLevel level)
        {
            object contentData = LoadLevelContentFromCache(level);
            if (contentData == null)
            {
                contentData = LoadLevelContentFromDisk(level);
            }
            return contentData;
        }

        private object LoadLevelContentFromDisk(SLTLevel level)
        {
            string url = string.Format(SLTConstants.LocalLevelContentPackageUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromApplication(url);
        }

        private object LoadLevelContentFromCache(SLTLevel level)
        {

            string url = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
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

            SLTResource resource = new SLTResource(SLTConstants.ResourceIdSaltr, ticket, loadFromSaltrSuccessCallback, loadFromSaltrFailCallback);
            resource.Load();
        }

        private string GetCachedLevelVersion(SLTLevel level)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectVersion(cachedFileName);
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
            urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionDevSyncData; //TODO @GSAR: remove later
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionDevSyncData;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = API_VERSION;
            args.Client = CLIENT;
            args.ClientKey = _clientKey;
            args.IsDevMode = _isDevMode;

            urlVars[SLTConstants.UrlParamDevMode] = _isDevMode.ToString();

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
                urlVars[SLTConstants.DeviceId] = _deviceId;
            }
            else
            {
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            if (_socialId != null)
            {
                args.SocialId = _socialId;
            }

            List<object> featureList = new List<object>();
            foreach (SLTFeature feature in _developerFeatures.Values)
            {
                Dictionary<string, object> featureDictionary = feature.ToDictionary();
                featureDictionary.RemoveEmptyOrNullEntries();
                featureList.Add(featureDictionary);
            }

            args.DeveloperFeatures = featureList;
            args.RawData.RemoveEmptyOrNullEntries();
            urlVars[SLTConstants.UrlParamArguments] = Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConstants.SALTR_DEVAPI_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource(SLTConstants.ResourceIdSyncFeatures, ticket, SyncSuccessHandler, SyncFailHandler);
            resource.Load();
        }

        private void AddDeviceToSaltr(string email)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionDevRegisterDevice;
            urlVars[SLTConstants.UrlParamClientKey] = _clientKey;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = API_VERSION;
            args.IsDevMode = _isDevMode;

            if (_deviceId != null)
            {
                args.Id = _deviceId;
            }
            else
            {
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            string deviceModel = SLTConstants.Unknown;
            string os = SLTConstants.Unknown;

            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    deviceModel = Util.GetHumanReadableDeviceModel(SystemInfo.deviceModel);
                    os = SystemInfo.operatingSystem.Replace("Phone ", string.Empty);
                    break;
                case RuntimePlatform.Android:
                    deviceModel = SystemInfo.deviceModel;
                    int versionNumber = 0;
                    string androidVersion = SystemInfo.operatingSystem;
                    versionNumber = int.Parse(androidVersion.Substring(androidVersion.IndexOf("API-") + 4, 2).ToString());
                    os = "Android " + versionNumber;
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    deviceModel = "PC";
                    os = SystemInfo.operatingSystem;
                    break;
                default:
                    deviceModel = SystemInfo.deviceModel;
                    os = SystemInfo.operatingSystem;
                    break;
            }

            args.Source = deviceModel;
            args.OS = os;

            if (!string.IsNullOrEmpty(email))
            {
                args.Email = email;
            }
            else
            {
                throw new Exception(ExceptionConstants.EmailIsRequired);
            }

            urlVars[SLTConstants.UrlParamArguments] = Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConstants.SALTR_DEVAPI_URL, urlVars);
            SLTResource resource = new SLTResource(SLTConstants.ResourceIdAddDevice, ticket, AddDeviceSuccessHandler, AddDeviceFailHandler);
            resource.Load();
        }

        private void CacheLevelContent(SLTLevel level, object content)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
            _repository.CacheObject(cachedFileName, level.Version.ToString(), content);
        }

        #endregion Internal Methods

        #region Handlers

        private void LevelContentLoadSuccessHandler(SLTLevel level, object content)
        {
            level.UpdateContent(content as Dictionary<string, object>);
            _onLevelContentLoadSuccess();
        }

        private void AppDataLoadSuccessCallback(SLTResource resource)
        {
            Dictionary<string, object> data = resource.Data;

            if (data == null)
            {
                _onConnectFail(new SLTStatusAppDataLoadFail());
                resource.Dispose();
                return;
            }

            bool isSuccess = false;
            Dictionary<string, object> response = new Dictionary<string, object>();

            if (data.ContainsKey(SLTConstants.Response))
            {
                IEnumerable<object> res = (IEnumerable<object>)data[SLTConstants.Response];
                response = res.FirstOrDefault() as Dictionary<string, object>;
                isSuccess = (bool)response[SLTConstants.Success]; //.ToString().ToLower() == "true";
            }
            else
            {
                //TODO @GSAR: remove later when API is versioned!
                if (data.ContainsKey(SLTConstants.ResponseData))
                {
                    response = data[SLTConstants.ResponseData] as Dictionary<string, object>;
                }

                isSuccess = (data.ContainsKey(SLTConstants.Status) && data[SLTConstants.Status].ToString() == SLTConstants.ResultSuccess);
            }

            _isLoading = false;

            if (isSuccess)
            {
                if (_isDevMode && !_isSynced)
                {
                    Sync();
                }

                if (response.ContainsKey(SLTConstants.LevelType))
                {
                    _levelType = (SLTLevelType)Enum.Parse(typeof(SLTLevelType), response[SLTConstants.LevelType].ToString(), true);
                }

                Dictionary<string, SLTFeature> saltrFeatures = new Dictionary<string, SLTFeature>();
                try
                {
                    saltrFeatures = SLTDeserializer.DecodeFeatures(response);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    _onConnectFail(new SLTStatusFeaturesParseError());
                    return;
                }

                try
                {
                    _experiments = SLTDeserializer.DecodeExperiments(response);
                }
                catch
                {
                    _onConnectFail(new SLTStatusExperimentsParseError());
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
                        _onConnectFail(new SLTStatusLevelsParseError());
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
                _repository.CacheObject(SLTConstants.AppDataUrlCache, "0", response);

                _activeFeatures = saltrFeatures;
                _onConnectSuccess();

                Debug.Log("[SALTR] AppData load success. LevelPacks loaded: " + _levelPacks.Count);
                //TODO @GSAR: later we need to report the feature set differences by an event or a callback to client;
            }
            else
            {
                if (response.ContainsKey(SLTConstants.Error))
                {
                    _onConnectFail(new SLTStatus(int.Parse(response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Code)), response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Message)));
                }
                else
                {
                    int errorCode;
                    int.TryParse(response.GetValue<string>(SLTConstants.ErrorCode), out errorCode);

                    _onConnectFail(new SLTStatus(errorCode, response.GetValue<string>(SLTConstants.ErrorMessage)));
                }

            }
            resource.Dispose();
        }

        private void SyncSuccessHandler(SLTResource resource)
        {
            object data = resource.Data;

            if (data == null)
            {
                Debug.Log("[Saltr] Dev feature Sync's data is null.");
                return;
            }

            var dataDict = data as Dictionary<string, object>;
            if (dataDict != null)
            {
                IEnumerable<object> response = (IEnumerable<object>)dataDict.GetValue(SLTConstants.Response);
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

                Dictionary<string, object> responseObject = response.ElementAt(0) as Dictionary<string, object>;

                if ((bool)responseObject.GetValue(SLTConstants.Success) == false)
                {

                    Dictionary<string, object> errorDict = responseObject.GetValue(SLTConstants.Error) as Dictionary<string, object>;
                    if (errorDict != null)
                    {
                        int errorCode;
                        int.TryParse(errorDict.GetValue(SLTConstants.Code).ToString(), out errorCode);
                        //TODO: change below casting to use Enum.Parse() method.
                        if ((SLTStatusCode)errorCode == SLTStatusCode.RegistrationRequired && _isAutoRegisteredDevice)
                        {
                            RegisterDevice();
                        }

                        Debug.Log("[Saltr] Sync error: " + errorDict.GetValue<string>(SLTConstants.Message));
                    }
                }
                else
                {
                    Debug.Log("[Saltr] Dev feature Sync is complete.");
                    _isSynced = true;
                }
            }
        }

        private void SyncFailHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev feature Sync has failed.");
        }

        private void AddDeviceSuccessHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev adding new device is complete.");
            Dictionary<string, object> data = resource.Data as Dictionary<string, object>;
            bool isSuccess = false;
            Dictionary<string, object> response;
            if (data.ContainsKey(SLTConstants.Response))
            {
                response = ((IEnumerable<object>)(data[SLTConstants.Response])).ElementAt(0) as Dictionary<string, object>;
                isSuccess = (bool)response.GetValue(SLTConstants.Success);
                if (isSuccess)
                {
                    _wrapper.SetStatus(SLTConstants.Success);
                    Sync();
                }
                else
                {
                    Dictionary<string, object> errorDict = response.GetValue(SLTConstants.Error) as Dictionary<string, object>;
                    if (errorDict != null)
                    {
                        _wrapper.SetStatus(errorDict.GetValue<string>(SLTConstants.Message));
                    }
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
            _wrapper.SetStatus(SLTConstants.Failed);
        }

        private void AppDataLoadFailCallback(SLTResource resource)
        {
            resource.Dispose();
            _isLoading = false;
            _onConnectFail(new SLTStatusAppDataLoadFail());
        }

        private void LevelContentLoadFailHandler()
        {
            _onLevelContentLoadFail(new SLTStatusLevelContentLoadFail());
        }

        #endregion Handlers

        #region Business Methods

        /// <summary>
        /// Checks if everything is initialized properly and starts the instance.
        /// After this call you can access application data (levels, features), and establish connection with the server.
        /// </summary>
        public void Start()
        {
            if (_deviceId == null)
            {
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            if (_developerFeatures.Count == 0 && !_useNoFeatures)
            {
                throw new Exception("Features should be defined.");
            }

            if (_levelPacks.Count == 0 && !_useNoLevels)
            {
                throw new Exception("Levels should be imported.");
            }

            object cachedData = _repository.GetObjectFromCache(SLTConstants.AppDataUrlCache);
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
                    _activeFeatures = SLTDeserializer.DecodeFeatures(cachedData as Dictionary<string, object>);
                    _experiments = SLTDeserializer.DecodeExperiments(cachedData as Dictionary<string, object>);
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

            _onConnectFail = failCallback;
            _onConnectSuccess = successCallback;

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
        /// Opens the device registration dialog. Can be called after <see cref="saltr.SLTUnity.Start"/> only.
        /// </summary>
        public void RegisterDevice()
        {
            if (!_isStarted)
            {
                throw new Exception("Method 'registerDevice()' should be called after 'Start()' only.");
            }
            _wrapper.ShowDeviceRegistationDialog(AddDeviceToSaltr);
        }

        // If you want to have a feature synced with SALTR you should call define before getAppData call.
        /// <summary>
        /// Defines a feature (<see cref="saltr.SLTFeature"/>).
        /// </summary>
        /// <param name="token">Token - a unique identifier for the feature.</param>
        /// <param name="properties">A dictionary of properties, that should be of "JSON friendly" datatypes 
        /// (string, int, double, Dictionary, List, etc.). To represent color use standard HTML format: <c>"#RRGGBB"</c> </param>
        /// <param name="isRequired">If set to <c>true</c> feature is isRequired(see <see cref="saltr.SLTUnity.GetFeatureProperties"/>). <c>false</c> by default.</param>
        public void DefineFeature(string token, Dictionary<string, object> properties, bool isRequired)
        {
            if (_useNoFeatures)
            {
                return;
            }

            if (_isStarted == false)
            {
                _developerFeatures[token] = new SLTFeature(token, properties, isRequired);
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
        /// Gets a level by its global index in all levels.
        /// </summary>
        /// <param name="index">Index in all levels.</param>
        public SLTLevel GetLevelByGlobalIndex(int index)
        {
            int levelSum = 0;
            foreach (var levelPack in _levelPacks)
            {
                int packLenght = levelPack.Levels.Count;
                if (index >= levelSum + packLenght)
                {
                    levelSum += packLenght;
                }
                else
                {
                    int localIndex = index - levelSum;
                    return levelPack.Levels[localIndex];
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
            foreach (var levelPack in _levelPacks)
            {
                int packLenght = levelPack.Levels.Count;
                if (index >= levelSum + packLenght)
                {
                    levelSum += packLenght;
                }

                else
                {
                    return levelPack;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a list of tokens(unique identifiers) of all features, active in Saltr.
        /// </summary>
        public List<string> GetActiveFeatureTokens()
        {
            var tokens = from feature in _activeFeatures.Values
                         where feature != null && feature.Token != null
                         select feature.Token;

            return tokens.ToList<string>();
        }

        /// <summary>
        /// Gets the properties of the feature specified by the token. 
        /// If a feature is set to be isRequired and is not active in, or cannot be retrieved from, Saltr, 
        /// the properties will be retrieved from default developer defined features.
        /// </summary>
        /// <param name="token">The feature token.</param>
        public Dictionary<string, object> GetFeatureProperties(string token)
        {
            if (_activeFeatures.ContainsKey(token))
            {
                return (_activeFeatures[token]).Properties as Dictionary<string, object>;
            }
            else
                if (_developerFeatures.ContainsKey(token))
                {
                    SLTFeature devFeature = _developerFeatures[token];
                    if (devFeature != null && devFeature.Required)
                    {
                        return devFeature.Properties as Dictionary<string, object>;
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
                path = path == null ? SLTConstants.LocalLevelPackageUrl : path;
                object applicationData = _repository.GetObjectFromApplication(path);
                _levelPacks = SLTDeserializer.DecodeLevels(applicationData as Dictionary<string, object>);
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

        /// <summary>
        /// Loads the content(boards and properties) of the level.
        /// Contents may be loaded from server, cache, or local level data(<see cref="saltr.SLTUnity.ImportLevels"/>).
        /// </summary>
        /// <param name="SLTLevel">The level, contents of which will be updated.</param>
        /// <param name="loadSuccessCallback">Load isSuccess callback.</param>
        /// <param name="loadFailCallback">Load fail callback, receives <see cref="saltr.Status.SLTStatus"/> object as the first parameter.</param>
        /// <param name="_useCache">If set to <c>false</c> cached level data will be ignored, forcing content to be loaded from server or local data if connection is not established. <c>true</c> by default. </param>
        public void LoadLevelContent(SLTLevel level, Action loadSuccessCallback, Action<SLTStatus> loadFailCallback, bool useCache)
        {
            _onLevelContentLoadSuccess = loadSuccessCallback;
            _onLevelContentLoadFail = loadFailCallback;
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
            urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionAddProperties; //TODO @GSAR: remove later
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionAddProperties;

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
            {
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

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
                Debug.Log(SLTConstants.Success);
                Dictionary<string, object> data = res.Data;
                res.Dispose();
            };

            Action<SLTResource> propertyAddFail = delegate(SLTResource res)
            {
                Debug.Log(SLTConstants.Error);
                res.Dispose();
            };

            urlVars[SLTConstants.UrlParamArguments] = Json.Serialize(args.RawData);

            SLTResourceTicket ticket = GetTicket(SLTConstants.SALTR_API_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource(SLTConstants.ResourceIdProperty, ticket, propertyAddSuccess, propertyAddFail);
            resource.Load();
        }

        /// <summary>
        /// See <see cref="saltr.SLTUnity.AddProperties"/>.
        /// </summary>
        public void AddProperties(SLTBasicProperties basicProperties)
        {
            AddProperties(basicProperties, null);
        }
        

        #endregion Public Methods

    }
}

