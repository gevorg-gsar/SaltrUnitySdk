﻿using UnityEngine;
using System.Collections;
using saltr_unity_sdk;
using System;
using System.Collections.Generic;
using Assets;
using System.Timers;
using System.Linq;



public class SLTUnity
{
    public const string API_VERSION = "1.0.1";


    protected string _socialId;
    private string _deviceId;

    public string deviceId
    {
        get { return _deviceId; }
        set { _deviceId = value; }
    }
    protected bool _conected;
    protected string _clientKey;
    protected string _saltrUserId;
    protected bool _isLoading;

    private ISLTRepository _repository;

    protected ISLTRepository repository
    {
        get { return _repository; }
        set { _repository = value; }
    }

    protected Dictionary<string, object> _activeFeatures;
    protected Dictionary<string, object> _developerFeatures;
    private List<SLTExperiment> _experiments = new List<SLTExperiment>();

    protected List<SLTExperiment> experiments
    {
        get { return _experiments; }
    }
    private List<SLTLevelPack> _levelPacks = new List<SLTLevelPack>();

    public List<SLTLevelPack> levelPacks
    {
        get { return _levelPacks; }
        set { _levelPacks = value; }
    }
    protected Action _connectSuccessCallback;
    protected Action<SLTStatus> _connectFailCallback;


    protected Action _levelContentLoadSuccessCalbck;
    protected Action<SLTStatusLevelContentLoadFail> _levelContentLoadFailCallback;

    private int _requestIdleTimeout;

    public int requestIdleTimeout
    {
        get { return _requestIdleTimeout; }
        set { _requestIdleTimeout = value; }
    }
    private bool _devMode;

    public bool devMode
    {
        set { _devMode = value; }
    }
    

    private bool _started;
    private bool _useNoLevels;

    public bool useNoLevels
    {
        get { return _useNoLevels; }
        set { _useNoLevels = value; }
    }
    private bool _useNoFeatures;

    public bool useNoFeatures
    {
        get { return _useNoFeatures; }
        set { _useNoFeatures = value; }
    }

	private string _levelType;


    public void addProperties(Dictionary<string, object> basicProperties = null, Dictionary<string, object> customProperties = null)
    {
        if (basicProperties == null && customProperties == null || _saltrUserId == null)
            return;

		Dictionary<string,string> urlVars = new Dictionary<string, string>();
		urlVars["cmd"] = SLTConfig.ACTION_ADD_PROPERTIES; //TODO @GSAR: remove later
		urlVars["action"] = SLTConfig.ACTION_ADD_PROPERTIES; 
		
		SLTRequestArguments args = new SLTRequestArguments()
        {
            apiVersion = API_VERSION,
            clientKey = _clientKey
        };

        if (_deviceId != null)
        {
            args.deviceId = _deviceId;
        }
        else
            Debug.Log("DeviceId is required");

        if (_socialId != null)
            args.socialId = _socialId;

        if (_saltrUserId != null)
            args.saltrUserId = _saltrUserId;

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


    private static SLTResourceTicket getTicket(string url, Dictionary<string,string> urlVars, int timeout = 0)
    {
		SLTResourceTicket ticket = new SLTResourceTicket(url, urlVars);
		//ticket.method = "post";
        if (timeout > 0)
        {
            ticket.idleTimeout = timeout;
        }

        return ticket;
    }



    public SLTUnity(string clientKey, string DeviceId, bool useCache = true)
    {

        GameObject saltr = new GameObject();
        saltr.name = "saltr";
        saltr.AddComponent<GETPOSTWrapper>();

        _clientKey = clientKey;
        _isLoading = false;
        _conected = false;

        _useNoLevels = false;
        _useNoFeatures = false;
        _deviceId = DeviceId;
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
            _repository = new SLTMobileRepository();
    }


    public string socialId
    {
		set{ _socialId = value;}
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


    public void importLevels(string path = null)
    {
		if(_useNoLevels) 
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
            Debug.Log("Method 'importLevels()' should be called before 'start()' only.");
        }
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

	/**
     * If you want to have a feature synced with SALTR you should call define before getAppData call.
     */
    public void defineFeature(string token, object properties, bool required = false)
    {
		if(_useNoFeatures)
		{
			return;
		}

        if (_started == false)
        {
            _developerFeatures["token"] = new SLTFeature(token, properties, required);
        }
        else
        {
            Debug.Log("Method 'defineFeature()' should be called before 'start()' only.");
        }
    }

    public void start()
    {
        if (_deviceId == null)
            Debug.Log("deviceId field is required and can't be null.");

        if (_developerFeatures.Count == 0 && !_useNoFeatures)
            Debug.Log("Features should be defined.");

        if (_levelPacks.Count == 0 && !_useNoLevels)
            Debug.Log("Levels should be imported.");


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
                _saltrUserId = cachedData.toDictionaryOrNull()["saltrUserId"].ToString();
            }
        }
        _started = true;
    }

    public void connect(Action successCallback, Action<SLTStatus> failCallback, Dictionary<string,object> basicProperties = null, Dictionary<string,object> customProperties = null)
    {
     
        if (_isLoading || !_started)
            return;

        _connectFailCallback = failCallback;
        _connectSuccessCallback = successCallback;

        _isLoading = true;
        SLTResource resource = createAppDataResource(appDataLoadSuccessCallback, appDataLoadFailCallback,basicProperties,customProperties);
        resource.load();
    }

	private SLTResource createAppDataResource(Action<SLTResource> loadSuccessCallback, Action<SLTResource> loadFailCallback, Dictionary<string,object> basicProperties,
        Dictionary<string,object> customProperties)
    {
		Dictionary<string,string> urlVars = new Dictionary<string, string>();

		urlVars["cmd"] = SLTConfig.ACTION_GET_APP_DATA; //TODO @GSAR: remove later
		urlVars["action"] = SLTConfig.ACTION_GET_APP_DATA;

		SLTRequestArguments args = new SLTRequestArguments();
		args.apiVersion = API_VERSION;
		args.clientKey = _clientKey;

        if (_deviceId != null)
            args.deviceId = _deviceId;
        else
            Debug.Log("Field 'deviceId' is required.");

        if (_socialId != null)
        {
            args.socialId = _socialId;
        }

        if(_saltrUserId != null)
        {
            args.saltrUserId = _saltrUserId;
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

    public void loadLevelContent(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel, Action loadSuccessCallback, Action<SLTStatusLevelContentLoadFail> loadFailCallback, bool useCache = true)
    {
        _levelContentLoadSuccessCalbck = loadSuccessCallback;
        _levelContentLoadFailCallback = loadFailCallback;
        object content = null;
        if (_conected == false)
        {
            if (useCache)
                content = loadLevelContentInternally(SLTLevelPack, SLTLevel);
            else
                content = loadLevelContentFromDisk(SLTLevelPack, SLTLevel);

            levelContentLoadSuccessHandler(SLTLevel, content);
        }
        else
        {
            if (useCache == false || SLTLevel.version != getCachedLevelVersion(SLTLevelPack, SLTLevel))
                loadLevelContentFromSaltr(SLTLevelPack, SLTLevel);
            else
            {
                content = loadLevelContentFromCache(SLTLevelPack, SLTLevel);
                levelContentLoadSuccessHandler(SLTLevel, content);
            }
        }
    }


    private object loadLevelContentFromDisk(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel)
    {
        string url = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_PACKAGE_URL_TEMPLATE, SLTLevelPack.index.ToString(), SLTLevel.index.ToString());

        return _repository.getObjectFromApplication(url);
    }

    private object loadLevelContentFromCache(SLTLevelPack levelPack, SLTLevel level)
    {

        string url = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, levelPack.index.ToString(), level.index.ToString());
        return _repository.getObjectFromCache(url);
    }


    private void loadLevelContentFromSaltr(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel)
    {
        string dataUrl = SLTLevel.contentDataUrl + "?_time_=" + DateTime.Now.ToShortTimeString();
        SLTResourceTicket ticket = new SLTResourceTicket(dataUrl, null);
        if (_requestIdleTimeout > 0)
            ticket.idleTimeout = _requestIdleTimeout;



        Action<SLTResource> loadSuccessInternalHandler = delegate(SLTResource res)
        {
            object contentData = res.data;
            if (contentData != null)
                cacheLevelContent(SLTLevelPack, SLTLevel, contentData);
            else
                contentData = loadLevelContentInternally(SLTLevelPack, SLTLevel);
            if (contentData != null)
                levelContentLoadSuccessHandler(SLTLevel, contentData);
            else
                levelContentLoadFailHandler();
            res.dispose();

        };

        Action<SLTResource> loadFailInternalHandler = delegate(SLTResource SLTResource)
      {
          object contentData = loadLevelContentInternally(SLTLevelPack, SLTLevel);
          levelContentLoadSuccessHandler(SLTLevel, contentData);
          SLTResource.dispose();
      };

        SLTResource resource = new SLTResource("saltr", ticket, loadSuccessInternalHandler, loadFailInternalHandler);

        resource.ticket.idleTimeout = requestIdleTimeout;
        resource.ticket.dropTimeout = requestIdleTimeout;
        resource.load();
    }


    private string getCachedLevelVersion(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel)
    {
        string cachedFileName = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, SLTLevelPack.index.ToString(), SLTLevel.index.ToString());
        return _repository.getObjectVersion(cachedFileName);
    }


	private void levelContentLoadSuccessHandler(SLTLevel sltLevel, object content)
    {
		sltLevel.updateContent(content.toDictionaryOrNull());
        _levelContentLoadSuccessCalbck();
    }


    private object loadLevelContentInternally(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel)
    {
        object contentData = loadLevelContentFromCache(SLTLevelPack, SLTLevel);
        if (contentData == null)
            contentData = loadLevelContentFromDisk(SLTLevelPack, SLTLevel);
        return contentData;
    }

	// complete
    private void appDataLoadSuccessCallback(SLTResource resource)
    {
        Dictionary<string, object> data = resource.data;

		if(data == null)
		{
			_connectFailCallback(new SLTStatusAppDataLoadFail());
			resource.dispose();
			return;
		}

		bool success = false;
		Dictionary<string, object> response = new Dictionary<string, object>();

        if(data.ContainsKey("response")) 
		{
			IEnumerable<object> res = (IEnumerable<object>)data["response"];
			response = res.FirstOrDefault().toDictionaryOrNull();
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

			if(response.ContainsKey("levelType"))
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

                foreach (var item in _experiments)
                {
                    Debug.Log("E -" + item.token);
                }
            }
            catch
            {
                _connectFailCallback(new SLTStatusExperimentsParseError());
                return;
            }

			// if developer didn't announce use without levels, and levelType in returned JSON is not "noLevels",
			// then - parse levels
			if(!_useNoLevels && _levelType != SLTLevel.LEVEL_TYPE_NONE)
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

            _saltrUserId = response["saltrUserId"].ToString();
            _conected = true;
            _repository.cacheObject(SLTConfig.APP_DATA_URL_CACHE, "0", response);

            _activeFeatures = saltrFeatures;
            _connectSuccessCallback();

            Debug.Log("[SALTR] AppData load success. LevelPacks loaded: " + _levelPacks.Count);
			//TODO @GSAR: later we need to report the feature set differences by an event or a callback to client;
        }
        else
            _connectFailCallback(new SLTStatus(int.Parse(response["errorCode"].ToString()), response["errorMessage"].ToString()));

        resource.dispose();
    }

    public Dictionary<string, object> getFeatureProperties(string token)
    {
        Debug.Log(_developerFeatures[_developerFeatures.Keys.ElementAt(0)]);
        if (_activeFeatures.ContainsKey(token))
        {
            return (_activeFeatures[token] as SLTFeature).properties.toDictionaryOrNull();
        }
        else
            if (_developerFeatures.ContainsKey(token))
            {
                return _developerFeatures[token].toDictionaryOrNull();
            }

            else return null;
    }

	void disposeLevelPacks ()
	{
		for(int i =0; i<_levelPacks.Count; ++i)
		{
			_levelPacks[i].dispose();
		}
		_levelPacks.Clear();
	}

    private void syncDeveloperFeatures()
    {
		Dictionary<string,string> urlVars = new Dictionary<string, string>();
		urlVars["cmd"] = SLTConfig.ACTION_DEV_SYNC_FEATURES; //TODO @GSAR: remove later
		urlVars["action"] = SLTConfig.ACTION_DEV_SYNC_FEATURES;

		SLTRequestArguments args = new SLTRequestArguments();
		args.apiVersion = API_VERSION;
		args.clientKey = _clientKey;


        if (_deviceId != null)
            args.deviceId = _deviceId;
        else
            Debug.Log("Field 'deviceId' is required.");

        if (_socialId != null)
        {
            args.socialId = _socialId;
        }

		if (_saltrUserId != null) {
			args.saltrUserId = _saltrUserId;
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


    private void cacheLevelContent(SLTLevelPack SLTLevelPack, SLTLevel SLTLevel, object content)
    {
        string cachedFileName = Utils.formatString(SLTConfig.LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE, SLTLevelPack.index.ToString(), SLTLevel.index.ToString());
        _repository.cacheObject(cachedFileName, SLTLevel.version.ToString(), content);
    }


    protected void levelContentLoadFailHandler()
    {
        _levelContentLoadFailCallback(new SLTStatusLevelContentLoadFail());
    }

    //TODO @GSAR: port this later when SALTR is ready

    //private void addUserProperty(List<string> propertyNames, List<object> propertyValues, List<string> operations)
    //{
    //    URLVariable urlVars = new URLVariable();
    //    urlVars.cmd = SLTConfig.CMD_ADD_PROPERTY;
    //    object args = ne

    //}

    //    private function addUserProperty(propertyNames:Vector.<String>, propertyValues:Vector.<*>, operations:Vector.<String>):void {
    ////        var urlVars:URLVariables = new URLVariables();
    ////        urlVars.cmd = SLTConfig.COMMAND_ADD_PROPERTY;
    ////        var args:Object = {saltId: _saltrUserId};
    ////        var properties:Array = [];
    ////        for (var i:uint = 0; i < propertyNames.length; i++) {
    ////            var propertyName:String = propertyNames[i];
    ////            var propertyValue:* = propertyValues[i];
    ////            var operation:String = operations[i];
    ////            properties.push({key: propertyName, value: propertyValue, operation: operation});
    ////        }
    ////        args.properties = properties;
    ////        args.clientKey = _clientKey;
    ////        urlVars.args = JSON.stringify(args);
    ////
    ////        var ticket:SLTResourceURLTicket = new SLTResourceURLTicket(SLTConfig.SALTR_API_URL, urlVars);
    ////        if (_requestIdleTimeout > 0) {
    ////            ticket.idleTimeout = _requestIdleTimeout;
    ////        }
    ////
    ////        var resource:SLTResource = new SLTResource("property", ticket,
    ////                function (resource:SLTResource):void {
    ////                    trace("getSaltLevelPacks : success");
    ////                    var data:Object = resource.jsonData;
    ////                    resource.dispose();
    ////                },
    ////                function (resource:SLTResource):void {
    ////                    trace("getSaltLevelPacks : error");
    ////                    resource.dispose();
    ////                });
    ////        resource.load();
    //    }

}
