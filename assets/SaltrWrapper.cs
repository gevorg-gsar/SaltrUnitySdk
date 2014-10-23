using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using saltr;

public class SaltrWrapper : MonoBehaviour {

	public string clientKey;
	public string deviceId = "";
	public string socialId;
	public bool devMode = false;
	public bool useCache = true;
	public bool useNoLevels = false;
	public bool useNoFeatures = false;
	public int requestIdleTimeout = 0;
	public string localLevelPackage = SLTConfig.LOCAL_LEVELPACK_PACKAGE_URL; // in Resources
	
	[System.Serializable]
	public class featureEntry
	{
		public string token;
		public propertyEntry[] properties;
		public bool required = false;
	}
	[System.Serializable]
	public class propertyEntry
	{
		public string key;
		public string value;
	}
	public featureEntry[] defaultFeatures;  
	public bool autoStart = false;


	private SLTUnity _saltr;

	void Awake()
	{
		gameObject.name = SLTUnity.SALTR_GAME_OBJECT_NAME;
		deviceId = deviceId != "" ? deviceId : SystemInfo.deviceUniqueIdentifier;
		_saltr = new SLTUnity(clientKey, deviceId, useCache);
		_saltr.socialId = socialId;
		_saltr.devMode = devMode;
		_saltr.useNoLevels = useNoLevels;
		_saltr.useNoFeatures = useNoFeatures;
		_saltr.requestIdleTimeout = requestIdleTimeout;
		_saltr.importLevels(localLevelPackage);

		foreach(featureEntry feateure in defaultFeatures)
		{
			Dictionary<string,object> properties = new Dictionary<string, object>();
			foreach(propertyEntry property in feateure.properties)
				properties[property.key] = property.value;
			_saltr.defineFeature(feateure.token, properties, feateure.required);
		}

		if(autoStart)
			_saltr.start();
	}

	public SLTUnity saltr
	{
		get { return _saltr;}
	}
}
