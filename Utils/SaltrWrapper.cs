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
        #region Fields

        private SLTUnity _saltr;

        [SerializeField]
        private string _clientKey = string.Empty;

        [SerializeField]
        private string _deviceId = string.Empty;

        [SerializeField]
        private string _socialId = string.Empty;

        [SerializeField]
        private bool _isDevMode = false;

        [SerializeField]
        private bool _isAutoRegisteredDevice = true;

        [SerializeField]
        private bool _useCache = true;

        [SerializeField]
        private bool _useNoLevels = false;

        [SerializeField]
        private bool _useNoFeatures = false;

        [SerializeField]
        private int _requestIdleTimeout = 0;

        [SerializeField]
        private string _localLevelPackage = SLTConstants.LocalLevelPackageUrl; // in Resources

        [SerializeField]
        private FeatureEntry[] _defaultFeatures = null;

        [SerializeField]
        private bool _autoStart = false;

        #endregion Fields

        #region Nested Classes

        [System.Serializable]
        public class FeatureEntry
        {
            public string token = string.Empty;
            public PropertyEntry[] properties = null;
            public bool required = false;
        }

        [System.Serializable]
        public class PropertyEntry
        {
            public string key = string.Empty;
            public string value = string.Empty;
        }

        #endregion Nested Classes

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

        #region Messages

        /// <summary>
        /// Creates and initialises <see cref="saltr.SLTUnity"/> instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (_saltr != null)
                return;

            gameObject.name = SLTUnity.SALTR_GAME_OBJECT_NAME;
            _deviceId = _deviceId != string.Empty ? _deviceId : SystemInfo.deviceUniqueIdentifier;
            _saltr = new SLTUnity(_clientKey, _deviceId, _useCache);
            _saltr.SocialId = _socialId;
            _saltr.IsDevMode = _isDevMode;
            _saltr.IsAutoRegisteredDevice = _isAutoRegisteredDevice;
            _saltr.UseNoLevels = _useNoLevels;
            _saltr.UseNoFeatures = _useNoFeatures;
            _saltr.RequestIdleTimeout = _requestIdleTimeout;
            _saltr.ImportLevels(_localLevelPackage);

            DefineFeatures();

            if (_autoStart)
                _saltr.Start();
        }

        /// <summary>
        /// Defines features that can be specified in the Inspector.
        /// </summary>
        protected virtual void DefineFeatures()
        {
            foreach (FeatureEntry feateure in _defaultFeatures)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                foreach (PropertyEntry property in feateure.properties)
                    properties[property.key] = property.value;
                _saltr.DefineFeature(feateure.token, properties, feateure.required);
            }
        }

        #endregion Messages

        #region TODO @gyln: move everything below to a separate script?

        //TODO @gyln: move everything below to a separate script?
        [SerializeField]
        private GUISkin _GUI_Skin = null;

        private int _mainAreaWidth = 250;
        private int _mainAreaHeight = 150;

        private string _defaultEmail = "example@mail.com";

        //	string _deviceName = "";
        private string _email = string.Empty;
        private string _status = "idle";

        private bool _loading = false;
        private bool _showDeviceRegistationDialog = false;

        private Action<string> _deviceRegistationDialogCallback;

        private void OnGUI()
        {
            if (_showDeviceRegistationDialog)
            {
                GUI.skin = _GUI_Skin;

                GUIUtility.ScaleAroundPivot(Vector2.one * (Screen.height / 512f), new Vector2(Screen.width / 2, Screen.height / 2));

                GUILayout.BeginArea(new Rect((Screen.width - _mainAreaWidth) / 2, (Screen.height - _mainAreaHeight) / 2, _mainAreaWidth, _mainAreaHeight));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Register Device with SALTR");

                GUILayout.BeginHorizontal();
                //					GUILayout.Label("Email");
                GUI.SetNextControlName("email_field");
                _email = GUILayout.TextField(_email, 256, GUILayout.Width(242));
                GUILayout.EndHorizontal();

                //				GUILayout.BeginHorizontal();
                //					GUILayout.Label("Device Name");
                //					_deviceName = GUILayout.TextField(_deviceName, 256,  GUILayout.Width(150));
                //				GUILayout.EndHorizontal();

                if (_status != "idle")
                    GUILayout.Label("Status:  " + _status);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Close") && !_loading)
                {
                    HideDeviceRegistationDialog();
                }
                if (GUILayout.Button("Submit") && !_loading)
                {
                    if (Util.IsValidEmail(_email))
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
                    if (GUI.GetNameOfFocusedControl() == "email_field")
                    {
                        if (_email == _defaultEmail)
                        {
                            _email = string.Empty;
                        }
                    }
                    else
                    {
                        if (_email == string.Empty)
                        {
                            _email = _defaultEmail;
                        }
                    }
                }

                GUI.skin = null;
            }

        }

        public void ShowDeviceRegistationDialog(Action<string> callback)
        {
            _showDeviceRegistationDialog = true;
            _deviceRegistationDialogCallback = callback;
        }

        public void HideDeviceRegistationDialog()
        {
            _showDeviceRegistationDialog = false;
            _loading = false;
        }

        public void SetStatus(string status)
        {
            _status = status;
            _loading = false;
        }

        #endregion TODO @gyln: move everything below to a separate script?

    }

}
