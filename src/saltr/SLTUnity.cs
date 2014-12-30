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
    public class SLTUnity // TODO @gyln: reorder memebrs as is in as3 sdk
    {
		internal const string CLIENT = "Unity";
        internal const string API_VERSION = "1.0.0"; //"0.9.0";
		/// <summary>
		/// The name of the Saltr GameObject, through which you can access SLTUnity instance.
		/// </summary>
        public const string SALTR_GAME_OBJECT_NAME = "Saltr";

		private string _socialId;
        private string _deviceId;
		private bool _conected;
		private string _clientKey;
		private bool _isLoading;

        private ISLTRepository _repository;

        private Dictionary<string, SLTFeature> _activeFeatures;
		private Dictionary<string, SLTFeature> _developerFeatures;

        private List<SLTExperiment> _experiments = new List<SLTExperiment>();
        private List<SLTLevelPack> _levelPacks = new List<SLTLevelPack>();

		private Action _connectSuccessCallback;
		private Action<SLTStatus> _connectFailCallback;
		private Action _levelContentLoadSuccessCallback;
		private Action<SLTStatus> _levelContentLoadFailCallback;

        private int _requestIdleTimeout;
        private bool _devMode;
		private bool _autoRegisterDevice;
		private bool _started;
		private bool _isSynced;
        private bool _useNoLevels;
        private bool _useNoFeatures;
        private string _levelType;

		SaltrWrapper _wrapper;

        private static SLTResourceTicket getTicket(string url, Dictionary<string, string> urlVars, int timeout)
        {
            SLTResourceTicket ticket = new SLTResourceTicket(url, urlVars);
            //ticket.method = "post"; // to implement
            if (timeout > 0)
            {
                ticket.dropTimeout = timeout;
            }

            return ticket;
        }

		private static SLTResourceTicket getTicket(string url, Dictionary<string, string> urlVars)
		{
			return getTicket(url, urlVars, 0);
		}

		void init(string clientKey, string DeviceId, bool useCache)
		{
			GameObject saltr = GameObject.Find(SALTR_GAME_OBJECT_NAME);
			if (saltr == null)
			{
				saltr = new GameObject();
				saltr.SetActive(false);
				saltr.name = SALTR_GAME_OBJECT_NAME;
				saltr.AddComponent<GETPOSTWrapper>();
				_wrapper = saltr.AddComponent<SaltrWrapper>();
				_wrapper.saltr = this;
				saltr.SetActive(true);
			}
			else 
			{
				_wrapper = saltr.GetComponent<SaltrWrapper>();
			}
			
			_clientKey = clientKey;
			_deviceId = DeviceId;
			_isLoading = false;
			_conected = false;
			_useNoLevels = false;
			_useNoFeatures = false;
			_levelType = null;
			
			_devMode = false;
			_autoRegisterDevice = true;
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

		/// <summary>
		/// Initializes a new instance of the <see cref="saltr.SLTUnity"/> class.
		/// </summary>
		/// <param name="clientKey">Client key.</param>
		/// <param name="DeviceId">Device identifier.</param>
		/// <param name="useCache">If set to <c>true</c> use cache. If not specified defaults to <c>true</c></param>
        public SLTUnity(string clientKey, string DeviceId, bool useCache)
        {
			init(clientKey, DeviceId, useCache);
        }

		/// <summary>
		/// See <see cref="saltr.SLTUnity.SLTUnity"/>.
		/// </summary>
		public SLTUnity(string clientKey, string DeviceId)
		{
			init(clientKey, DeviceId, true);
		}

		// <summary>
		// Sets the repository used by this instance. An appropriate repository is already set by a constructor,
		// so you will need this only if you want to implement and use your own custom repository (<see cref="saltr.repository.ISLTRepository"/>).
		// </summary>        
		internal ISLTRepository repository
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
		/// In this mode client data(e.g. developer defined features) will be synced with Saltr, once, after successful <see cref="saltr.SLTUnity.connect"/> call.
		/// </summary>
		/// <value><c>true</c> to enable; <c>false</c> to disable.</value>
        public bool devMode
        {
            set { _devMode = value; }
        }

		/// <summary>
		/// Sets a value indicating whether device registratioon dialog should be automaticaly shown in dev mode(<see cref="saltr.SLTUnity.devMode"/>),
		/// after successful <see cref="saltr.SLTUnity.connect"/> call, if the device was not registered already.
		/// </summary>
		/// <value><c>true</c> to enable; <c>false</c> to disable. By default is set to <c>true</c></value>
		public bool autoRegisterDevice
		{
			set { _autoRegisterDevice = value; }
		}
		
		/// <summary>
		/// Sets the request idle timeout. If a URL request takes more than timeout to complete, it would be canceled.
		/// </summary>
		/// <value>The request idle timeout in milliseconds.</value>
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
		/// Gets a list of all experiments.
		/// </summary>
		public List<SLTExperiment> experiments
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
			foreach (SLTFeature feature in _activeFeatures.Values)
            {
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

		/// <summary>
		/// Imports the level data from local files, that can be downloaded from Saltr. 
		/// If your application is using levels, this must be called before calling start(), otherwise has no effect.
		/// </summary>
		/// <param name="path">
		/// The path to level packs in Resources folder. 
		/// If not specified the <see cref="saltr.SLTConfig.LOCAL_LEVELPACK_PACKAGE_URL"/> will be used.
		/// </param>
        public void importLevels(string path)
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

		/// <summary>
		/// See <see cref="saltr.SLTUnity.importLevels"/>.
		/// </summary>
		public void importLevels()
		{
			importLevels (null);
		}

        // If you want to have a feature synced with SALTR you should call define before getAppData call.
		/// <summary>
		/// Defines a feature (<see cref="saltr.SLTFeature"/>).
		/// </summary>
		/// <param name="token">Token - a unique identifier for the feature.</param>
		/// <param name="properties">A dictionary of properties, that should be of "JSON friendly" datatypes 
		/// (string, int, double, Dictionary, List, etc.). To represent color use standard HTML format: <c>"#RRGGBB"</c> </param>
		/// <param name="required">If set to <c>true</c> feature is required(see <see cref="saltr.SLTUnity.getFeatureProperties"/>). <c>false</c> by default.</param>
        public void defineFeature(string token, Dictionary<string,object> properties, bool required)
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

		/// <summary>
		/// See <see cref="saltr.SLTUnity.defineFeature"/>.
		/// </summary>
		public void defineFeature(string token, Dictionary<string,object> properties)
		{
			defineFeature(token, properties, false);
		}

		/// <summary>
		/// Checks if everything is initialized properly and starts the instance.
		/// After this call you can access application data (levels, features), and establish connection with the server.
		/// </summary>
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

		/// <summary>
		/// Establishes connection to the server and updates application data(levels, features and experiments).
		/// After connecting successfully you can load level content from server with <see cref="saltr.SLTUnity.loadLevelContent"/> .
		/// </summary>
		/// <param name="successCallback">Success callback.</param>
		/// <param name="failCallback">Fail callback, receives <see cref="saltr.status.SLTStatus"/> object as the first parameter.</param>
		/// <param name="basicProperties">(Optional)Basic properties. Same as in <see cref="saltr.SLTUnity.addProperties"/>.</param>
		/// <param name="customProperties">(Optional)Custom properties. Same as in <see cref="saltr.SLTUnity.addProperties"/>.</param>
        public void connect(Action successCallback, Action<SLTStatus> failCallback, SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            if (!_started)
			{
				throw new Exception("Method 'connect()' should be called after 'start()' only.");
			}

			if(_isLoading)
			{
				failCallback(new SLTStatusAppDataConcurrentLoadRefused());
				return;
			}
			
            _connectFailCallback = failCallback;
            _connectSuccessCallback = successCallback;

            _isLoading = true;
            SLTResource resource = createAppDataResource(appDataLoadSuccessCallback, appDataLoadFailCallback, basicProperties, customProperties);
            resource.load();
        }

		/// <summary>
		/// See <see cref="saltr.SLTUnity.connect"/>.
		/// </summary>
		public void connect(Action successCallback, Action<SLTStatus> failCallback, SLTBasicProperties basicProperties)
		{
			connect(successCallback, failCallback, basicProperties, null);
		}

		/// <summary>
		/// See <see cref="saltr.SLTUnity.connect"/>.
		/// </summary>
		public void connect(Action successCallback, Action<SLTStatus> failCallback)
		{
			connect(successCallback, failCallback, null);
		}

		private SLTResource createAppDataResource(Action<SLTResource> loadSuccessCallback, Action<SLTResource> loadFailCallback, SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
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

			urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            return new SLTResource("saltAppConfig", ticket, loadSuccessCallback, loadFailCallback);
        }

		/// <summary>
		/// Loads the content(boards and properties) of the level.
		/// Contents may be loaded from server, cache, or local level data(<see cref="saltr.SLTUnity.importLevels"/>).
		/// </summary>
		/// <param name="SLTLevel">The level, contents of which will be updated.</param>
		/// <param name="loadSuccessCallback">Load success callback.</param>
		/// <param name="loadFailCallback">Load fail callback, receives <see cref="saltr.status.SLTStatus"/> object as the first parameter.</param>
		/// <param name="useCache">If set to <c>false</c> cached level data will be ignored, forcing content to be loaded from server or local data if connection is not established. <c>true</c> by default. </param>
        public void loadLevelContent(SLTLevel SLTLevel, Action loadSuccessCallback, Action<SLTStatus> loadFailCallback, bool useCache)
        {
            _levelContentLoadSuccessCallback = loadSuccessCallback;
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

		/// <summary>
		/// See <see cref="saltr.SLTUnity.loadLevelContent"/>.
		/// </summary>
		public void loadLevelContent(SLTLevel SLTLevel, Action loadSuccessCallback, Action<SLTStatus> loadFailCallback)
		{
			loadLevelContent(SLTLevel, loadSuccessCallback, loadFailCallback, true);
		}

		/// <summary>
		/// Associates some properties with this client, that are used to assign it to a certain user group in Saltr.
		/// </summary>
		/// <param name="basicProperties">Basic properties. Standard set of client properties</param>
		/// <param name="customProperties">(Optional)Custom properties.</param>
        public void addProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
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

			urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_API_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("property", ticket, propertyAddSuccess, propertyAddFail);
            resource.load();
        }

		/// <summary>
		/// See <see cref="saltr.SLTUnity.addProperties"/>.
		/// </summary>
		public void addProperties(SLTBasicProperties basicProperties)
		{
			addProperties(basicProperties, null);
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

			// some ugly stuff here came from AS3... you don't do like this in normal languages
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
				if (contentData != null)
				levelContentLoadSuccessHandler(sltLevel, contentData);
				else
				levelContentLoadFailHandler();
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
            _levelContentLoadSuccessCallback();
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
				if (_devMode && !_isSynced)
				{
					sync();
				}

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
		
        void sync()
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars["cmd"] = SLTConfig.ACTION_DEV_SYNC_DATA; //TODO @GSAR: remove later
            urlVars["action"] = SLTConfig.ACTION_DEV_SYNC_DATA;

            SLTRequestArguments args = new SLTRequestArguments();
            args.apiVersion = API_VERSION;
            args.clientKey = _clientKey;
			args.devMode = _devMode;
			urlVars["devMode"] = _devMode.ToString();
			args.CLIENT = CLIENT;

            if (_deviceId != null)
			{
                args.deviceId = _deviceId;
				urlVars["deviceId"] = _deviceId;
			}
            else
                throw new Exception("Field 'deviceId' is required.");

            if (_socialId != null)
            {
                args.socialId = _socialId;
            }

            List<object> featureList = new List<object>();
			foreach (SLTFeature feature in _developerFeatures.Values)
            {
				Dictionary<string, object> featureDictionary = feature.ToDictionary();
				featureDictionary.removeEmptyOrNull();
				featureList.Add(featureDictionary);
            }

            args.developerFeatures = featureList;
			args.RawData.removeEmptyOrNull();
			urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

            SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_DEVAPI_URL, urlVars, _requestIdleTimeout);
            SLTResource resource = new SLTResource("syncFeatures", ticket, syncSuccessHandler, syncFailHandler);
            resource.load();
        }

		/// <summary>
		/// Opens the device registration dialog. Can be called after <see cref="saltr.SLTUnity.start"/> only.
		/// </summary>
		public void registerDevice()
		{
			if(!_started) 
			{
				throw new Exception("Method 'registerDevice()' should be called after 'start()' only.");
			}
			_wrapper.showDeviceRegistationDialog(addDeviceToSALTR);
		}

        void syncSuccessHandler(SLTResource SLTResource)
        {
			object data = SLTResource.data;

			if(data == null)
			{
				Debug.Log("[Saltr] Dev feature Sync's data is null.");
				return;
			}

			IEnumerable<object> response = (IEnumerable<object>)data.toDictionaryOrNull().getValue("response");
			if(response == null) 
			{
				Debug.Log("[Saltr] Dev feature Sync's response is null.");
				return;
			}

			if(response.Count() <= 0)
			{
				Debug.Log("[Saltr] Dev feature Sync response's length is <= 0.");
				return;
			}

			Dictionary<string,object> responseObject = response.ElementAt(0).toDictionaryOrNull();

			if((bool)responseObject.getValue("success") == false)
			{
				if((SLTStatus.Code)responseObject.getValue("error").toDictionaryOrNull().getValue("code").toIntegerOrZero() == SLTStatus.Code.REGISTRATION_REQUIRED && _autoRegisterDevice)
				{
					registerDevice();
				}
				Debug.Log("[Saltr] Sync error: " +	responseObject.getValue("error").toDictionaryOrNull().getValue<string>("message"));
			}
			else
			{
				Debug.Log("[Saltr] Dev feature Sync is complete.");
				_isSynced = true;
			}
        }

        void syncFailHandler(SLTResource resource)
        {
            Debug.Log("[Saltr] Dev feature Sync has failed.");
        }

		void addDeviceToSALTR(string email)
		{
			Dictionary<string, string> urlVars = new Dictionary<string, string>();
			urlVars["action"] = SLTConfig.ACTION_DEV_REGISTER_DEVICE;
			urlVars["clientKey"] = _clientKey;
		
			SLTRequestArguments args = new SLTRequestArguments();
			args.devMode = _devMode;
			args.apiVersion = API_VERSION;


			if(_deviceId != null)
			{
				args.id =_deviceId;
			}
			else
			{
				throw new Exception("Field 'deviceId' is required");
			}

			string model = "Unknown";
			string os = "Unknown";
			switch(Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				model = Utils.getHumanReadableDeviceModel(SystemInfo.deviceModel); 
				os = SystemInfo.operatingSystem.Replace("Phone ", "");
				break;
			case RuntimePlatform.Android:
				model = SystemInfo.deviceModel;
				int iVersionNumber = 0;
				string androidVersion = SystemInfo.operatingSystem;
				int sdkPos = androidVersion.IndexOf("API-");
				iVersionNumber = int.Parse(androidVersion.Substring(sdkPos+4,2).ToString());
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

			args.source = model;
			args.os = os;

			if(email != null && email != "")
			{
				args.email = email;
			}
			else
			{
				throw new Exception("Email is required.");
			}

			urlVars["args"] = MiniJSON.Json.Serialize(args.RawData);

			SLTResourceTicket ticket = getTicket(SLTConfig.SALTR_DEVAPI_URL, urlVars);
			SLTResource resource = new SLTResource("addDevice", ticket, addDeviceSuccessHandler, addDeviceFailHandler);
			resource.load();
		}

		void addDeviceSuccessHandler (SLTResource resource)
		{
			Debug.Log("[Saltr] Dev adding new device is complete.");
			Dictionary<string, object> data = resource.data.toDictionaryOrNull();
			bool success = false;
			Dictionary<string, object> response;
			if(data.ContainsKey("response"))
			{
				response = ((IEnumerable<object>) (data["response"])).ElementAt(0).toDictionaryOrNull();
				success = (bool)response.getValue("success");
				if(success)
				{
					_wrapper.setStatus("Success");
					sync ();
				}
				else
				{
					_wrapper.setStatus(response.getValue("error").toDictionaryOrNull().getValue<string>("message"));
				}
			}
			else
			{
				addDeviceFailHandler (resource);
			}
		}

		void addDeviceFailHandler (SLTResource resource)
		{
			Debug.Log("[Saltr] Dev adding new device has failed.");
			_wrapper.setStatus("Failed");
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


        void levelContentLoadFailHandler()
        {
            _levelContentLoadFailCallback(new SLTStatusLevelContentLoadFail());
        }
        
    }
}
