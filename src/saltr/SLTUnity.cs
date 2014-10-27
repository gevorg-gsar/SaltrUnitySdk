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
    public class SLTUnity
    {
        public const string CLIENT = "Unity";
        public const string API_VERSION = "1.0.1"; //"0.9.0";
        public const string SALTR_GAME_OBJECT_NAME = "Saltr";
        protected string _socialId;
        private string _deviceId;
        protected bool _conected;
        protected string _clientKey;
        protected bool _isLoading;

        private ISLTRepository _repository;

        protected Dictionary<string, SLTFeature> _activeFeatures;
        protected Dictionary<string, SLTFeature> _developerFeatures;

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

            _activeFeatures = new Dictionary<string, SLTFeature>();
			_developerFeatures = new Dictionary<string, SLTFeature>();
            _experiments = new List<SLTExperiment>();
            _levelPacks = new List<SLTLevelPack>();

            if (useCache)
                _repository = new SLTMobileRepository();
            else
                _repository = new SLTDummyRepository();
        }

        public ISLTRepository repository
        {
            set { _repository = value; }
        }

        public bool useNoFeatures
        {
            set { _useNoFeatures = value; }
        }

        public bool useNoLevels
        {
            set { _useNoLevels = value; }
        }

        public bool devMode
        {
            set { _devMode = value; }
        }

        public int requestIdleTimeout
        {
            set { _requestIdleTimeout = value; }
        }

        public List<SLTLevelPack> levelPacks
        {
            get { return _levelPacks; }
        }

        public List<SLTLevel> allLevels
        {
            get
            {


                // levelPacks = levelPacks.OrderByDescending(l => l.index).ToList();
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

        protected List<SLTExperiment> experiments
        {
            get { return _experiments; }
        }

        public string socialId
        {
            set { _socialId = value; }
        }

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

        public List<string> getActiveFeatureTokens()
        {
            List<string> tokens = new List<string>();
			foreach (SLTFeature feature in _activeFeatures.Values)
            {
                if (feature != null && feature.token != null)
                    tokens.Add(feature.token);
            }
            return tokens;
        }

        public Dictionary<string, object> getFeatureProperties(string token)
        {
            if (_activeFeatures.ContainsKey(token))
            {
                return (_activeFeatures[token]).properties.toDictionaryOrNull();
            }
            else
                if (_developerFeatures.ContainsKey(token))
                {
                    SLTFeature devFeature = _developerFeatures[token];
					if (devFeature != null && devFeature.required)
                        return devFeature.properties.toDictionaryOrNull();
                }

            return null;
        }

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
        public void defineFeature(string token, Dictionary<string,object> properties, bool required = false)
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

        // complete
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

				Dictionary<string, SLTFeature> saltrFeatures = new Dictionary<string, SLTFeature>();
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
			foreach (SLTFeature feature in _developerFeatures.Values)
            {
				featureList.Add(new { token = feature.token, value = LitJson.JsonMapper.ToJson(feature.properties) });
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
