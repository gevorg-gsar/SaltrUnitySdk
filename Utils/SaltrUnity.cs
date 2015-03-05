using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Saltr.UnitySdk;
using Saltr.UnitySdk.Status;
using Saltr.UnitySdk.Utils;
using Plexonic.Core.Network;

namespace Saltr.UnitySdk.Utils
{

    /// <summary>
    /// Unity Saltr.
    /// </summary>
    public class SaltrUnity : MonoBehaviour
    {
        #region Fields

        private SaltrConnector _saltrConnector;

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

        #region Properties

        public SaltrConnector SaltrConnector
        {
            get { return _saltrConnector; }
            internal set { _saltrConnector = value; }
        }

        #endregion

        #region Messages

        /// <summary>
        /// Creates and initialises <see cref="saltr.SLTUnity"/> instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (_saltrConnector != null)
                return;

            gameObject.name = SLTConstants.SaltrGameObjectName;
            gameObject.AddComponent<HttpWrapper>();

            _deviceId = _deviceId != string.Empty ? _deviceId : SystemInfo.deviceUniqueIdentifier;

            _saltrConnector = new SaltrConnector(_clientKey, _deviceId, _useCache);

            _saltrConnector.SocialId = _socialId;
            _saltrConnector.IsDevMode = _isDevMode;
            _saltrConnector.IsAutoRegisteredDevice = _isAutoRegisteredDevice;
            _saltrConnector.UseNoLevels = _useNoLevels;
            _saltrConnector.UseNoFeatures = _useNoFeatures;
            _saltrConnector.RequestIdleTimeout = _requestIdleTimeout;

            _saltrConnector.ImportLevels(_localLevelPackage);

            DefineFeatures();

            if (_autoStart)
            { 
                _saltrConnector.Start();
            }
        }

        /// <summary>
        /// Defines features that can be specified in the Inspector.
        /// </summary>
        protected virtual void DefineFeatures()
        {
            foreach (FeatureEntry feature in _defaultFeatures)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                foreach (PropertyEntry property in feature.properties)
                {
                    properties[property.key] = property.value;
                }

                _saltrConnector.DefineFeature(feature.token, properties, feature.required);
            }
        }

        #endregion Messages


        #region Business Methods



        #endregion Business Methods











        #region TODO @gyln: move everything below to a separate script?

        //TODO @gyln: move everything below to a separate script?
        [SerializeField]
        private GUISkin _GUI_Skin = null;

        private int _mainAreaWidth = 250;
        private int _mainAreaHeight = 150;

        private string _defaultEmail = "example@mail.com";

        //	string _deviceName = "";
        private string _email = string.Empty;
        private string _status = SLTConstants.StatusIdle;

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

                GUILayout.BeginVertical(SLTConstants.GuiBox);
                GUILayout.Label(SLTConstants.GuiRegisterDeviceWithSaltr);

                GUILayout.BeginHorizontal();
                //					GUILayout.Label("Email");
                GUI.SetNextControlName(SLTConstants.GuiEmailField);
                _email = GUILayout.TextField(_email, 256, GUILayout.Width(242));
                GUILayout.EndHorizontal();

                //				GUILayout.BeginHorizontal();
                //					GUILayout.Label("Device Name");
                //					_deviceName = GUILayout.TextField(_deviceName, 256,  GUILayout.Width(150));
                //				GUILayout.EndHorizontal();

                if (_status != SLTConstants.StatusIdle)
                    GUILayout.Label("Status:  " + _status);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(SLTConstants.GuiClose) && !_loading)
                {
                    HideDeviceRegistationDialog();
                }
                if (GUILayout.Button(SLTConstants.GuiSubmit) && !_loading)
                {
                    if (Util.IsValidEmail(_email))
                    {
                        _deviceRegistationDialogCallback(_email);
                        _status = SLTConstants.StatusLoading;
                        _loading = true;
                    }
                    else
                    {
                        _status = SLTConstants.StatusInvalidEmail;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.EndArea();

                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    if (GUI.GetNameOfFocusedControl() == SLTConstants.GuiEmailField)
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
