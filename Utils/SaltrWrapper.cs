using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Saltr.UnitySdk;
using Saltr.UnitySdk.Status;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Utils
{

/// <summary>
/// Saltr wrapper.
/// </summary>
public class SaltrWrapper : MonoBehaviour
{
	[SerializeField]
	string clientKey = "";
	[SerializeField]
	string deviceId = "";
	[SerializeField]
	string socialId = "";
	[SerializeField]
	bool devMode = false;
	[SerializeField]
	bool isAutoRegisteredDevice = true;
	[SerializeField]
	bool useCache = true;
	[SerializeField]
	bool useNoLevels = false;
	[SerializeField]
	bool useNoFeatures = false;
	[SerializeField]
	int requestIdleTimeout = 0;
	[SerializeField]
    string localLevelPackage = SLTConstants.LocalLevelPackageUrl; // in Resources

    [System.Serializable]
    internal class FeatureEntry
    {
        public string token = "";
        public PropertyEntry[] properties = null;
        public bool required = false;
    }
    [System.Serializable]
    internal class PropertyEntry
    {
        public string key = "";
        public string value = "";
    }
	
	[SerializeField]
    FeatureEntry[] defaultFeatures = null;
	[SerializeField]
    bool autoStart = false;


    private SLTUnity _saltr;

    #region Propertiess
	/// <summary>
	/// Gets all levels count.
	/// </summary>
    public int AllLevelsCount
    {
        get { return (int)_saltr.AllLevelsCount; }
    }

	/// <summary>
	/// Gets the <see cref="saltr.SLTUnity"/> instance.
	/// </summary>
    public SLTUnity Saltr
    {
        get { return _saltr; }
		internal set { _saltr = value; }
    }
    #endregion


    /// <summary>
	/// Creates and initialises <see cref="saltr.SLTUnity"/> instance.
    /// </summary>
    protected virtual void Awake()
    {
		if(_saltr != null)
				return;

        gameObject.name = SLTUnity.SALTR_GAME_OBJECT_NAME;
        deviceId = deviceId != "" ? deviceId : SystemInfo.deviceUniqueIdentifier;
		_saltr = new SLTUnity(clientKey, deviceId, useCache);
        _saltr.SocialId = socialId;
        _saltr.IsDevMode = devMode;
		_saltr.IsAutoRegisteredDevice = isAutoRegisteredDevice;
        _saltr.UseNoLevels = useNoLevels;
        _saltr.UseNoFeatures = useNoFeatures;
        _saltr.RequestIdleTimeout = requestIdleTimeout;
        _saltr.ImportLevels(localLevelPackage);

		DefineFeatures();

        if (autoStart)
            _saltr.Start();
    }

	/// <summary>
	/// Defines features that can be specified in the Inspector.
	/// </summary>
	protected virtual void DefineFeatures()
	{
		foreach (FeatureEntry feateure in defaultFeatures)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			foreach (PropertyEntry property in feateure.properties)
				properties[property.key] = property.value;
			_saltr.DefineFeature(feateure.token, properties, feateure.required);
		}
	}

	//TODO @gyln: move everything below to a separate script?
	[SerializeField]
	GUISkin _GUI_Skin = null;

	int _mainAreaWidth = 250;
	int _mainAreaHeight = 150;

	string _defaultEmail = "example@mail.com";

//	string _deviceName = "";
	string _email = "";
	string _status = "idle";

	bool _loading = false; 
	bool _showDeviceRegistationDialog = false;

	Action<string> _deviceRegistationDialogCallback;

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
						HideDeviceRegistationDialog();
					}
					if(GUILayout.Button("Submit") && !_loading)
					{
						if(Util.ValidEmail(_email))
						{
							_deviceRegistationDialogCallback(_email);
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

	internal void ShowDeviceRegistationDialog(Action<string> callback)
	{
		_showDeviceRegistationDialog = true;
		_deviceRegistationDialogCallback = callback;
	}

	internal void HideDeviceRegistationDialog()
	{
		_showDeviceRegistationDialog = false;
		_loading = false;
	}

	internal void SetStatus(string status)
	{
		_status = status;
		_loading = false;
	}
}

}
