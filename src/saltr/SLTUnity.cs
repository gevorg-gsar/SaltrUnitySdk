using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using saltr.repository;
using saltr.resource;
using saltr.utils;
using saltr.game;
using saltr.status;

namespace saltr
{
    //TODO:: @daal add some flushCache method.
	/// <summary>
	/// The entry point to SDK, and the main class, used to send and receive data from Saltr.
	/// </summary>
    public class SLTUnity
    {
		/// <summary>
		/// Type of client SDK, used to communicate with Saltr.
		/// </summary>
        public const string CLIENT = "Unity";
        /// <summary>
		/// The API version used to communicate with Saltr.
		/// </summary>
		public const string API_VERSION = "1.0.1"; //"0.9.0";
		/// <summary>
		/// The name of the Saltr GameObject, through which you can access SLTUnity instance.
		/// </summary>
        public const string SALTR_GAME_OBJECT_NAME = "Saltr";

        protected string _socialId;
        private string _deviceId;
        protected bool _conected;
        protected string _clientKey;
        protected bool _isLoading;

        private ISLTRepository _repository;

        protected Dictionary<string, object> _activeFeatures;
        protected Dictionary<string, object> _developerFeatures;

        private List<SLTExperiment> _experiments = new List<SLTExperiment>();
        private List<SLTLevelPack> _levelPacks = new List<SLTLevelPack>();

        protected Action _connectSuccessCallback;
        protected Action<SLTStatus> _connectFailCallback;
        protected Action _levelContentLoadSuccessCalbck;
        protected Action<SLTStatusLevelContentLoadFail> _levelContentLoadFailCallback;

        private int _requestIdleTimeout;
        private bool _devMode;
        private bool _started;
        private bool _useNoLevels;
        private bool _useNoFeatures;
        private string _levelType;


        private static SLTResourceTicket getTicket(string url, Dictionary<string, string> urlVars, int timeout = 0)
        {
            SLTResourceTicket ticket = new SLTResourceTicket(url, urlVars);
            //ticket.method = "post"; // to implement
            if (timeout > 0)
            {
                ticket.idleTimeout = timeout;
            }

            return ticket;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="saltr.SLTUnity"/> class.
		/// </summary>
		/// <param name="clientKey">Client key.</param>
		/// <param name="DeviceId">Device identifier.</param>
		/// <param name="useCache">If set to <c>true</c> use cache.</param>
        public SLTUnity(string clientKey, string DeviceId, bool useCache = true)
        {
            if (GameObject.Find(SALTR_GAME_OBJECT_NAME) == null)
            {
                GameObject saltr = new GameObject();
                saltr.name = SALTR_GAME_OBJECT_NAME;
                saltr.AddComponent<GETPOSTWrapper>();
            }

            _clientKey = clientKey;
            _deviceId = DeviceId;
            _isLoading = false;
            _conected = false;
            _useNoLevels = false;
            _useNoFeatures = false;
            _levelType = null;

            _devMode = false;
            _started = false;
            _requestIdleTimeout = 0;

            _activeFeatures = new Dictionary<string, object>();
            _developerFeatures = new Dictionary<string, object>();
            _experiments = new List<SLTExperiment>();
            _levelPacks = new List<SLTLevelPack>();

            if (useCache)
                _repository = new SLTMobileRepository();
            else
                _repository = new SLTDummyRepository();
        }

		/// <summary>
		/// Sets the repository used by this instance. An appropriate repository is allready set by a constructor,
		/// so you will need this only if you want to implement and use your own custom repository (<see cref="saltr.repository.ISLTRepository"/>).
		/// </summary>
        public ISLTRepository repository
        {
            set { _repository = value; }
        }

		/// <summary>
		/// Sets a value indicating whether the application does not use features.
		/// </summary>
		/// <value><c>true</c> if features are not used; otherwise, <c>false</c>.</value>
        public bool useNoFeatures
        {
            set { _useNoFeatures = value; }
        }

		/// <summary>
		/// Sets a value indicating whether the application does not use levels.
		/// </summary>
		/// <value><c>true</c> if levels are not used; otherwise, <c>false</c>.</value>
        public bool useNoLevels
        {
            set { _useNoLevels = value; }
        }

		/// <summary>
		/// Sets a value indicating weather this <see cref="saltr.SLTUnity"/> should operate in dev(developer) mode.
		/// In this mode developer defined features will be synced with Saltr on next connect() call.
		/// </summary>
		/// <value><c>true</c> in dev mode; otherwise, <c>false</c>.</value>
        public bool devMode
        {
            set { _devMode = value; }
        }

		/// <summary>
		/// Sets the request idle timeout. If a URL request takes more than timeout to complete, it would be canceled.
		/// </summary>
		/// <value>The request idle timeout in milliseonds.</value>
        public int requestIdleTimeout
        {
            set { _requestIdleTimeout = value; }
        }

		/// <summary>
		/// Gets the level packs.
		/// </summary>
        public List<SLTLevelPack> levelPacks
        {
            get { return _levelPacks; }
        }

		/// <summary>
		/// Gets a list all levels.
		/// </summary>
        public List<SLTLevel> allLevels
        {
            get
            {
                List<SLTLevel> allLevels = new List<SLTLevel>();
                for (int i = 0; i < _levelPacks.Count; i++)
                {
                    List<SLTLevel> packLevels = _levelPacks[i].levels;
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
        public uint allLevelsCount
        {
            get
            {
                uint count = 0;
                for (int i = 0; i < _levelPacks.Count; i++)
                {
                    count += (uint)_levelPacks[i].levels.Count;
                }
                return count;
            }
        }

		/// <summary>
		/// Gets the experiments.
		/// </summary>
        protected List<SLTExperiment> experiments
        {
            get { return _experiments; }
        }

		/// <summary>
		/// Sets the social identifier of the user.
		/// </summary>
        public string socialId
        {
            set { _socialId = value; }
        }

		/// <summary>
		/// Gets a level by its global index in all levels.
		/// </summary>
		/// <param name="index">Index in all levels.</param>
        public SLTLevel getLevelByGlobalIndex(int index)
        {
            int levelSum = 0;
            for (int i = 0; i < _levelPacks.Count; i++)
            {
                int packLenght = _levelPacks[i].levels.Count;
                if (index >= levelSum + packLenght)
                {
                    levelSum += packLenght;
                }
                else
                {
                    int localIndex = index - levelSum;
                    return _levelPacks[i].levels[localIndex];
                }
            }
            return null;
        }

		/// <summary>
		/// Gets the level pack that contains the level with given global index in all levels.
		/// </summary>
		/// <param name="index">Index of the level in all levels.</param>
        public SLTLevelPack getPackByLevelGlobalIndex(int index)
        {
            int levelSum = 0;
            for (int i = 0; i < _levelPacks.Count; i++)
            {
                int packLenght = _levelPacks[i].levels.Count;
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
        public List<string> getActiveFeatureTokens()
        {
            List<string> tokens = new List<string>();
            foreach (object item in _activeFeatures.Values)
            {
                SLTFeature feature = item as SLTFeature;
                if (feature != null && feature.token != null)
                    tokens.Add(feature.token);
            }
            return tokens;
        }

		/// <summary>
		/// Gets the properties of the feature specified by the token. 
		/// If a feature is set to be required and is not active in, or cannot be retrieved from, Saltr, 
		/// the properties will be retrieved from default developer defined features.
		/// </summary>
		/// <param name="token">The feature token.</param>
        public Dictionary<string, object> getFeatureProperties(string token)
        {
            if (_activeFeatures.ContainsKey(token))
            {
                return (_activeFeatures[token] as SLTFeature).properties.toDictionaryOrNull();
            }
            else
                if (_developerFeatures.ContainsKey(token))
                {
                    SLTFeature devFeature = _developerFeatures[token] as SLTFeature;
                    if (devFeature != null)
                        return devFeature.properties.toDictionaryOrNull();
                }

            return null;
        }

		/// <summary>
		/// Imports the level data from local files, that can be downloaded from Saltr. 
		/// If your application is using levels, this must be called before calling start().
		/// </summary>
		/// <param name="path">
		/// The path to level packs in Resources folder. 
		/// If not specified the <see cref="saltr.SLTConfig.LOCAL_LEVELPACK_PACKAGE_URL"/> will be used.
		/// </param>
        public void importLevels(string path = null)
        {
            if (_useNoLevels)
            {
                return;
            }

            if (_started == false)
            {
                path = path == null ? SLTConfig.LOCAL_LEVELPACK_PACKAGE_URL : path;
                object applicationData = _repository.getObjectFromApplication(path);
                _levelPacks = SLTDeserializer.decodeLevels(applicationData.toDictionaryOrNull());
            }
            else
            {
                throw new Exception("Method 'importLevels()' should be called before 'start()' only.");
            }
        }

        /**
         * If you want to have a feature synced with SALTR you should call define before getAppData call.
         */
        public void defineFeature(string token, object properties, bool required = false)
        {
            if (_useNoFeatures)
            {
                return;
            }

            if (_started == false)
            {
                _developerFeatures[token] = new SLTFeature(token, properties, required);
            }
            else
            {
                throw new Exception("Method 'defineFeature()' should be called before 'start()' only.");
            }
        }

        public void start()
        {
            if (_deviceId == null)
                throw new Exception("deviceId field is required and can't be null.");

            if (_developerFeatures.Count == 0 && !_useNoFeatures)
				throw new Exception("Features should be defined.");

            if (_levelPacks.Count == 0 && !_useNoLevels)
				throw new Exception("Levels should be imported.");


            object cachedData = _repository.getObjectFromCache(SLTConfig.APP_DATA_URL_CACHE);
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
                    _activeFeatures = SLTDeserializer.decodeFeatures(cachedData.toDictionaryOrNull());
                    _experiments = SLTDeserializer.decodeExperiments(cachedData.toDictionaryOrNull());
                }
            }
            _started = true;
        }

        public void connect(Action successCallback, Action<SLTStatus> failCallback, Dictionary<string, object> basicProperties = null, Dictionary<string, object> customProperties = null)
        {

            if (_isLoading || !_started)
                return;

            _connectFailCallback = failCallback;
            _connectSuccessCallback = successCallback;

            _isLoading = true;
            SLTResource resource = createAppDataResource(appDataLoadSuccessCallback, appDataLoadFailCallback, basicProperties, customProperties);
            resource.load();
        }

        private SLTResource createAppDataResource(Action<SLTResource> loadSuccessCallback, Action<SLTResource> loadFailCallback, Dictionary<string, object> basicProperties,
            Dictionary<string, object> customProperties)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();

            urlVars["cmd"] = SLTConfig.ACTION_GET_APP_DATA; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ACTION_GET_APP_DATA;

            SLTRequestArguments args = new SLTRequestArguments();
            args.apiVersion = API_VERSION;
            args.clientKey = _clientKey;
            args.CLIENT = CLIENT;

            if (_deviceId != null)
                args.deviceId = _deviceId;
            else
                throw new Exception("Field 'deviceId' is required.");

            if (_socialId != null)
            {
                args.socialId = _socialId;
            }

            if (basicProperties != null)
            {
                args.basicProperties = basicProperties;
            }

            if (customProperties != null)
            {
                args.customProperties = customProperties;
            }

            urlVars["args"] = LitJson.JsonMapper.ToJson(args);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            return new SLTResource("saltAppConfig", ticket, loadSuccessCallback, loadFailCallback);
        }

        public void loadLevelContent(SLTLevel SLTLevel, Action loadSuccessCallback, Action<SLTStatusLevelContentLoadFail> loadFailCallback, bool useCache = true)
        {
            _levelContentLoadSuccessCalbck = loadSuccessCallback;
            _levelContentLoadFailCallback = loadFailCallback;
            object content = null;
            if (_conected == false)
            {
                if (useCache)
                    content = loadLevelContentInternally(SLTLevel);
                else
                    content = loadLevelContentFromDisk(SLTLevel);

                levelContentLoadSuccessHandler(SLTLevel, content);
            }
            else
            {
                if (useCache == false || SLTLevel.version != getCachedLevelVersion(SLTLevel))
                    loadLevelContentFromSaltr(SLTLevel);
                else
                {
                    content = loadLevelContentFromCache(SLTLevel);
                    levelContentLoadSuccessHandler(SLTLevel, content);
                }
            }
        }
		
        public void addProperties(Dictionary<string, object> basicProperties = null, Dictionary<string, object> customProperties = null)
        {
            if (basicProperties == null && customProperties == null)
                return;

            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["cmd"] = SLTConfig.ACTION_ADD_PROPERTIES; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ACTION_ADD_PROPERTIES;

            SLTRequestArguments args = new SLTRequestArguments()
            {
                apiVersion = API_VERSION,
                clientKey = _clientKey,
                CLIENT = CLIENT
            };

            if (_deviceId != null)
            {
                args.deviceId = _deviceId;
            }
            else
				throw new Exception("Field 'deviceId' is a required.");

            if (_socialId != null)
                args.socialId = _socialId;

            if (basicProperties != null)
                args.basicProperties = basicProperties;

            if (customProperties != null)
                args.customProperties = customProperties;

            Action<SLTResource> propertyAddSuccess = delegate(SLTResource res)
            {
                Debug.Log("success");
                Dictionary<string, object> data = res.data;
                res.dispose();
            };

            Action<SLTResource> propertyAddFail = delegate(SLTResource res)
            {
                Debug.Log("error");
                res.dispose();
            };

            urlVars["args"] = LitJson.JsonMapper.ToJson(args);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("property", ticket, propertyAddSuccess, propertyAddFail);
            resource.load();
        }

        private object loadLevelContentFromDisk(SLTLevel sltLevel)
        {
            string url = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_PACKAGE_URL_TEMPLATE, sltLevel.packIndex.ToString(), sltLevel.localIndex.ToString());
            return _repository.getObjectFromApplication(url);
        }

        private object loadLevelContentFromCache(SLTLevel sltLevel)
        {

            string url = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, sltLevel.packIndex.ToString(), sltLevel.localIndex.ToString());
            return _repository.getObjectFromCache(url);
        }


        private void loadLevelContentFromSaltr(SLTLevel sltLevel)
        {
            string dataUrl = sltLevel.contentUrl + "?_time_=" + DateTime.Now.ToShortTimeString();
            SLTResourceTicket ticket = getTicket(dataUrl, null, _requestIdleTimeout);

            Action<SLTResource> loadFromSaltrSuccessCallback = delegate(SLTResource res)
            {
                object contentData = res.data;
                if (contentData != null)
                    cacheLevelContent(sltLevel, contentData);
                else
                    contentData = loadLevelContentInternally(sltLevel);
                if (contentData != null)
                    levelContentLoadSuccessHandler(sltLevel, contentData);
                else
                    levelContentLoadFailHandler();
                res.dispose();

            };

            Action<SLTResource> loadFromSaltrFailCallback = delegate(SLTResource SLTResource)
            {
                object contentData = loadLevelContentInternally(sltLevel);
                levelContentLoadSuccessHandler(sltLevel, contentData);
                SLTResource.dispose();
            };

            SLTResource resource = new SLTResource("saltr", ticket, loadFromSaltrSuccessCallback, loadFromSaltrFailCallback);
            resource.load();
        }


        private string getCachedLevelVersion(SLTLevel sltLevel)
        {
            string cachedFileName = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, sltLevel.packIndex.ToString(), sltLevel.localIndex.ToString());
            return _repository.getObjectVersion(cachedFileName);
        }


        private void levelContentLoadSuccessHandler(SLTLevel sltLevel, object content)
        {
            sltLevel.updateContent(content.toDictionaryOrNull());
            _levelContentLoadSuccessCalbck();
        }


        private object loadLevelContentInternally(SLTLevel sltLevel)
        {
            object contentData = loadLevelContentFromCache(sltLevel);
            if (contentData == null)
                contentData = loadLevelContentFromDisk(sltLevel);
            return contentData;
        }

        private void appDataLoadSuccessCallback(SLTResource resource)
        {
            Dictionary<string, object> data = resource.data;

            if (data == null)
            {
                _connectFailCallback(new SLTStatusAppDataLoadFail());
                resource.dispose();
                return;
            }

            bool success = false;
            Dictionary<string, object> response = new Dictionary<string, object>();

            if (data.ContainsKey("response"))
            {
                IEnumerable<object> res = (IEnumerable<object>)data["response"];
                response = res.FirstOrDefault().toDictionaryOrNull();
				success = (bool)response["success"]; //.ToString().ToLower() == "true";
            }
            else
            {
                //TODO @GSAR: remove later when API is versioned!
                if (data.ContainsKey("responseData"))
                    response = data["responseData"].toDictionaryOrNull();

                success = (data.ContainsKey("status") && data["status"].ToString() == SLTConfig.RESULT_SUCCEED);
            }

            _isLoading = false;

            if (success)
            {
                if (_devMode)
                    syncDeveloperFeatures();

                if (response.ContainsKey("levelType"))
                {
                    _levelType = response["levelType"].ToString();
                }

                Dictionary<string, object> saltrFeatures = new Dictionary<string, object>();
                try
                {
                    saltrFeatures = SLTDeserializer.decodeFeatures(response);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    _connectFailCallback(new SLTStatusFeaturesParseError());
                    return;
                }

                try
                {
                    _experiments = SLTDeserializer.decodeExperiments(response);
                }
                catch
                {
                    _connectFailCallback(new SLTStatusExperimentsParseError());
                    return;
                }

                // if developer didn't announce use without levels, and levelType in returned JSON is not "noLevels",
                // then - parse levels
                if (!_useNoLevels && _levelType != SLTLevel.LEVEL_TYPE_NONE)
                {
                    List<SLTLevelPack> newLevelPacks = null;
                    try
                    {
                        newLevelPacks = SLTDeserializer.decodeLevels(response);
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
                        disposeLevelPacks();
                        _levelPacks = newLevelPacks;
                    }
                }

                _conected = true;
                _repository.cacheObject(SLTConfig.APP_DATA_URL_CACHE, "0", response);

                _activeFeatures = saltrFeatures;
                _connectSuccessCallback();

                Debug.Log("[SALTR] AppData load success. LevelPacks loaded: " + _levelPacks.Count);
                //TODO @GSAR: later we need to report the feature set differences by an event or a callback to client;
            }
            else
            {
                if (response.ContainsKey("error"))
                {
                    _connectFailCallback(new SLTStatus(int.Parse(response.getValue<Dictionary<string, object>>("error").getValue<string>("code")), response.getValue<Dictionary<string, object>>("error").getValue<string>("message")));

                }
                else
                    _connectFailCallback(new SLTStatus(response.getValue<string>("errorCode").toIntegerOrZero(), response.getValue<string>("errorMessage")));
            }
            resource.dispose();
        }

        void disposeLevelPacks()
        {
            for (int i = 0; i < _levelPacks.Count; ++i)
            {
                _levelPacks[i].dispose();
            }
            _levelPacks.Clear();
        }

        private void syncDeveloperFeatures()
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["cmd"] = SLTConfig.ACTION_DEV_SYNC_FEATURES; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ACTION_DEV_SYNC_FEATURES;

            SLTRequestArguments args = new SLTRequestArguments();
            args.apiVersion = API_VERSION;
            args.clientKey = _clientKey;
            args.CLIENT = CLIENT;

            if (_deviceId != null)
                args.deviceId = _deviceId;
            else
                throw new Exception("Field 'deviceId' is required.");

            if (_socialId != null)
            {
                args.socialId = _socialId;
            }

            List<object> featureList = new List<object>();
            foreach (string i in _developerFeatures.Keys)
            {
                SLTFeature SLTFeature = _developerFeatures[i] as SLTFeature;
                featureList.Add(new { token = SLTFeature.token, value = LitJson.JsonMapper.ToJson(SLTFeature.properties) });
            }

            args.developerFeatures = featureList;

            urlVars["args"] = LitJson.JsonMapper.ToJson(args);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_DEVAPI_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("syncFeatures", ticket, syncSuccessHandler, syncFailHandler);
            resource.load();
        }



        protected void syncSuccessHandler(SLTResource SLTResource)
        {
            Debug.Log("[Saltr] Dev feature Sync is complete.");
        }

        protected void syncFailHandler(SLTResource SLTResource)
        {
            Debug.Log("[Saltr] Dev feature Sync has failed.");
        }


        private void appDataLoadFailCallback(SLTResource resource)
        {
            resource.dispose();
            _isLoading = false;
            _connectFailCallback(new SLTStatusAppDataLoadFail());
        }


        private void cacheLevelContent(SLTLevel sltLevel, object content)
        {
            string cachedFileName = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, sltLevel.packIndex.ToString(), sltLevel.localIndex.ToString());
            _repository.cacheObject(cachedFileName, sltLevel.version.ToString(), content);
        }


        protected void levelContentLoadFailHandler()
        {
            _levelContentLoadFailCallback(new SLTStatusLevelContentLoadFail());
        }
    }
}
