using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using saltr;
using saltr.status;
using saltr.utils;

public class SaltrWrapper : MonoBehaviour
{
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

    #region Properties
    public int AllLevelsCount
    {
        get { return (int)_saltr.allLevelsCount; }
    }

    public SLTUnity saltr
    {
        get { return _saltr; }
    }
    #endregion


    //public virtual void Awake(){}
    protected void Awake()
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

		defineFeatures();

        if (autoStart)
            _saltr.start();
    }

	virtual protected void defineFeatures()
	{
		foreach (featureEntry feateure in defaultFeatures)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			foreach (propertyEntry property in feateure.properties)
				properties[property.key] = property.value;
			_saltr.defineFeature(feateure.token, properties, feateure.required);
		}
	}

	//TODO @gyln: move everything below to a separate script?
	[SerializeField]
	GUISkin _GUI_Skin = null;

	int _mainAreaWidth = 250;
	int _mainAreaHeight = 150;

	string _defaultEmail = "example@mail.com";

	string _deviceName = "";
	string _email = "";
	string _status = "idle";

	bool _loading = false; 
	bool _showDeviceRegistationDialog = false;

	Action<string,string> _deviceRegistationDialogCallback;

	void OnGUI()
	{
		if(_showDeviceRegistationDialog)
		{
			GUI.skin = _GUI_Skin;

			GUIUtility.ScaleAroundPivot(Vector2.one * (Screen.height/512f), new Vector2(Screen.width / 2, Screen.height / 2));

			GUILayout.BeginArea(new Rect((Screen.width - _mainAreaWidth)/2, (Screen.height - _mainAreaHeight)/2, _mainAreaWidth, _mainAreaHeight));

			GUILayout.BeginVertical("box");
				GUILayout.Label("Register Device with SALTR");

				GUILayout.BeginHorizontal();
//					GUILayout.Label("Email");
					GUI.SetNextControlName ("email_field");
					_email = GUILayout.TextField(_email, 256, GUILayout.Width(242));
				GUILayout.EndHorizontal();

//				GUILayout.BeginHorizontal();
//					GUILayout.Label("Device Name");
//					_deviceName = GUILayout.TextField(_deviceName, 256,  GUILayout.Width(150));
//				GUILayout.EndHorizontal();

				if(_status != "idle") 
					GUILayout.Label("Status:  " + _status);

				GUILayout.BeginHorizontal();
					if(GUILayout.Button("Close") && !_loading)
					{
						hideDeviceRegistationDialog();
					}
					if(GUILayout.Button("Submit") && !_loading)
					{
						if(Utils.validEmail(_email))
						{
							_deviceRegistationDialogCallback(_deviceName, _email);
							_status = "Loading...";
							_loading = true;
						}
						else
						{
							_status = "Invalid email!";
						}
					}
				GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUILayout.EndArea();

			if (UnityEngine.Event.current.type == EventType.Repaint)
			{
				if (GUI.GetNameOfFocusedControl () == "email_field")
				{
					if (_email == _defaultEmail) _email = "";
				}
				else
				{
					if (_email == "") _email = _defaultEmail;
				}
			}

			GUI.skin = null;
		}

	}

	public void showDeviceRegistationDialog(Action<string,string> callback)
	{
		_showDeviceRegistationDialog = true;
		_deviceRegistationDialogCallback = callback;
	}

	public void hideDeviceRegistationDialog()
	{
		_showDeviceRegistationDialog = false;
		_loading = false;
	}

	public void setStatus(string status)
	{
		_status = status;
		_loading = false;
	}
}
