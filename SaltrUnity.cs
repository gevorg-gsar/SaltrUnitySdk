using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Saltr.UnitySdk;
using Saltr.UnitySdk.Utils;
using Saltr.UnitySdk.Domain.InternalModel;
using Saltr.UnitySdk.Domain;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain.Model;
using Saltr.UnitySdk.Network;

namespace Saltr.UnitySdk
{

    /// <summary>
    /// Unity Saltr.
    /// </summary>
    public class SaltrUnity : MonoBehaviour
    {
        #region Fields

        private Coroutine _heartBeat;
        private SaltrConnector _saltrConnector;

        [SerializeField]
        private SLTLevelType _levelType = SLTLevelType.NoLevels; /// Specifies the type of the game: Matching, Canvas2D, etc...

        [SerializeField]
        private string _clientKey = string.Empty; /// Client key provided by Saltr.

        [SerializeField]
        private string _deviceId = string.Empty; /// Device Identifier: Provided device identifier is used otherwise gets current device identifier.

        [SerializeField]
        private string _socialId = string.Empty; /// Social Identifier: FacebookId, GoogleId, etc...

        [SerializeField]
        private bool _autoStart = false; /// If is set to true ImportLevels, DefineDefaultFeatures and Init methods are called during application start.

        [SerializeField]
        private bool _isDevMode = false; /// Indicates if the game is in development mode. If set true, developer features are synced with the saltr after app data is gotten. 

        [SerializeField]
        private bool _isAutoRegisteredDevice = true; /// If set to true, device registration dialog is shown.

        [SerializeField]
        private bool _useCache = true; /// Specifies if the level contents should be cached locally.

        [SerializeField]
        private bool _useNoLevels = false; /// Specifies if the game uses levels or no. If set to true, ImportLevels should be called before Init.

        [SerializeField]
        private bool _useNoFeatures = false; /// Specifies if the game uses features or no. If set to true, DefineDefaultFeatures should be called before Init.

        //[SerializeField]
        //private int _timeout = 0;

        [SerializeField]
        private string _localLevelPacksPath = SLTConstants.LocalLevelPacksPath; /// Specifies Path to local level_packs gotten from Saltr.

        [SerializeField]
        private FeatureEntry[] _defaultFeatures = null; /// Specifies Default Features for the game. This features will be synced with Saltr.

        #endregion Fields

        #region Properties

        public bool IsStarted { get; private set; } /// Indicates if Init method is called.

        public string SocialId
        {
            set { _socialId = value; }
            get { return _socialId; }
        }

        public bool UseNoFeatures /// Specifies if the game uses features or no. If set to true, DefineDefaultFeatures should be called before Init.
        {
            set { _useNoFeatures = value; }
            get { return _useNoFeatures; }
        }

        public bool UseNoLevels /// Specifies if the game uses levels or no. If set to true, ImportLevels should be called before Init.
        {
            set { _useNoLevels = value; }
            get { return _useNoLevels; }
        }

        public bool IsDevMode /// Indicates if the game is in development mode. If set true, developer features are synced with the saltr after app data is gotten. 
        {
            set { _isDevMode = value; }
            get { return _isDevMode; }
        }

        public bool IsAutoRegisteredDevice
        {
            set { _isAutoRegisteredDevice = value; }
            get { return _isAutoRegisteredDevice; }
        } /// If set to true, device registration dialog is shown.

        //public int RequestIdleTimeout
        //{
        //    set { _requestIdleTimeout = value; }
        //}

        #endregion

        #region Saltr Connector Methods/Properties

        public List<SLTLevelPack> LevelPacks /// Level Packs: Each level pack contains a list of levels.
        {
            get { return _saltrConnector.LevelPacks; }
        }

        public List<SLTExperiment> Experiments /// Experiments which should be configured in Saltr.
        {
            get { return _saltrConnector.Experiments; }
        }

        private Dictionary<string, SLTFeature> ActiveFeatures /// Represents current active features. Default Features are used when there's no internet connection available otherwise syncs with Saltr.
        {
            get { return _saltrConnector.ActiveFeatures; }
        }

        private Dictionary<string, SLTFeature> DefaultFeatures /// Represents default features which are used locally when there's no internet connection.
        {
            get { return _saltrConnector.DefaultFeatures; }
        }

        public List<SLTLevel> AllLevels /// Represents a all levels in a raw list.
        {
            get
            {
                return LevelPacks.SelectMany(lp => lp.Levels).ToList<SLTLevel>();
            }
        }

        public int AllLevelsCount /// All levels count.
        {
            get
            {
                return LevelPacks.Sum(lp => lp.Levels.Count);
            }
        }

        public SLTLevel GetLevelByGlobalIndex(int index) ///Gets level by global index.
        {
            return AllLevels.ElementAt<SLTLevel>(index);
        }

        public SLTLevelPack GetPackByLevelGlobalIndex(int index) /// Gets level pack of the level with index.
        {
            SLTLevel levelByIndex = GetLevelByGlobalIndex(index);
            return LevelPacks.FirstOrDefault<SLTLevelPack>(lp => lp.Levels.Contains<SLTLevel>(levelByIndex));
        }

        public Dictionary<string, object> GetFeatureProperties(string token) /// Gets feature properties with specified token.
        {
            if (ActiveFeatures.ContainsKey(token))
            {
                return (ActiveFeatures[token]).Properties as Dictionary<string, object>;
            }
            else if (DefaultFeatures.ContainsKey(token) && DefaultFeatures[token].IsRequired)
            {
                return (DefaultFeatures[token]).Properties as Dictionary<string, object>;
            }

            return null;
        }

        #endregion  Saltr Connector Methods/Properties

        #region Events

        public event Action GetAppDataSuccess /// Event fires when app data is gotten.
        {
            add { _saltrConnector.GetAppDataSuccess += value; }
            remove { _saltrConnector.GetAppDataSuccess -= value; }
        }

        public event Action<SLTErrorStatus> GetAppDataFail /// Event fires when app data retrival is failed. Error status contains fail information.
        {
            add { _saltrConnector.GetAppDataFail += value; }
            remove { _saltrConnector.GetAppDataFail -= value; }
        }

        public event Action<SLTLevel> LoadLevelContentSuccess /// Event fires after successfuly level content load.
        {
            add { _saltrConnector.LoadLevelContentSuccess += value; }
            remove { _saltrConnector.LoadLevelContentSuccess -= value; }
        }

        public event Action<SLTErrorStatus> LoadLevelConnectFail /// Event fires when level load is failed. Error status contains fail information.
        {
            add { _saltrConnector.LoadLevelConnectFail += value; }
            remove { _saltrConnector.LoadLevelConnectFail -= value; }
        }

        public event Action DeviceRegistrationRequired /// Event fires when device registration is needed. Same time device registration dialog is shown.
        {
            add { _saltrConnector.DeviceRegistrationRequired += value; }
            remove { _saltrConnector.DeviceRegistrationRequired -= value; }
        }

        public event Action RegisterDeviceSuccess /// Event fires after successful device registration.
        {
            add { _saltrConnector.RegisterDeviceSuccess += value; }
            remove { _saltrConnector.RegisterDeviceSuccess -= value; }
        }

        public event Action<SLTErrorStatus> RegisterDeviceFail /// Event fires after failed device registration. Error status contains fail information.
        {
            add { _saltrConnector.RegisterDeviceFail += value; }
            remove { _saltrConnector.RegisterDeviceFail -= value; }
        }

        public event Action AddPropertiesSuccess /// Event fires after successful AddProperties call.
        {
            add { _saltrConnector.AddPropertiesSuccess += value; }
            remove { _saltrConnector.AddPropertiesSuccess -= value; }
        }

        public event Action<SLTErrorStatus> AddPropertiesFail  /// Event fires after failed AddProperties call. Error status contains fail information.
        {
            add { _saltrConnector.AddPropertiesFail += value; }
            remove { _saltrConnector.AddPropertiesFail -= value; }
        }

        #endregion Events

        #region MonoBehaviour

        protected virtual void Awake() /// Used to initialize SaltrUnity. Is called automatically after script is added to a game object.
        {
            if (_saltrConnector != null)
                return;

            BoardConverter.LevelType = _levelType;
            SLTAssetTypeConverter.LevelType = _levelType;

            DontDestroyOnLoad(gameObject);
            gameObject.name = SLTConstants.SaltrGameObjectName;
            gameObject.AddComponent<SLTDownloadManager>();

            if (string.IsNullOrEmpty(_deviceId))
            {
                _deviceId = SystemInfo.deviceUniqueIdentifier;
            }

            _saltrConnector = new SaltrConnector(_clientKey, _deviceId, _useCache);

            _saltrConnector.IsDevMode = _isDevMode;
            _saltrConnector.SocialId = _socialId;

            _saltrConnector.IsAutoRegisteredDevice = _isAutoRegisteredDevice;
            //_saltrConnector.RequestIdleTimeout = _requestIdleTimeout;

            _saltrConnector.UseNoLevels = _useNoLevels;
            _saltrConnector.UseNoFeatures = _useNoFeatures;

            _saltrConnector.GetAppDataSuccess -= SaltrConnector_GetAppDataSuccess;
            _saltrConnector.GetAppDataSuccess += SaltrConnector_GetAppDataSuccess;
            _saltrConnector.RegisterDeviceSuccess -= SaltrConnector_RegisterDeviceSuccess;
            _saltrConnector.RegisterDeviceSuccess += SaltrConnector_RegisterDeviceSuccess;
            _saltrConnector.LoadLevelContentSuccess -= SaltrConnector_LoadLevelContentSuccess;
            _saltrConnector.LoadLevelContentSuccess += SaltrConnector_LoadLevelContentSuccess;
            _saltrConnector.DeviceRegistrationRequired -= SaltrConnector_OnDeviceRegistrationRequired;
            _saltrConnector.DeviceRegistrationRequired += SaltrConnector_OnDeviceRegistrationRequired;

        }

        protected virtual void Start() /// Start is called after awake automatically. When _autoStart is set to true then ImportLevels, DefineDefaultFeatures and Init are called.
        {
            if (_autoStart)
            {
                ImportLevels(_localLevelPacksPath);
                DefineDefaultFeatures(_defaultFeatures);

                Init();
            }
        }

        #endregion Monobehaviour

        #region Public Methods

        public virtual void Init() /// Initializes Saltr engine.
        {
            if (_saltrConnector == null)
            {
                throw new Exception(SLTExceptionConstants.SaltrConnectorShouldBeCreated);
            }

            _saltrConnector.Init();
            IsStarted = true;
        }

        public virtual void ImportLevels(string path) /// Imports local levels from application. Should be called before Init call if useNoLevels is set to false otherwide no need to call this method.
        {
            if (_useNoLevels)
            {
                return;
            }

            if (!IsStarted)
            {
                _saltrConnector.ImportLevels(path);
            }
            else
            {
                throw new Exception("Method 'ImportLevels()' should be called before 'Init()' only.");
            }
        }

        public virtual void DefineDefaultFeatures(FeatureEntry[] features = null) /// Defines default features. If the feature is required then it's synced with Saltr. Should be called before Init call if useNoFeatures is set to false otherwise no need to call this method.
        {
            if (_useNoFeatures)
            {
                return;
            }

            if (!IsStarted)
            {
                _defaultFeatures = features ?? _defaultFeatures;

                foreach (FeatureEntry featureEntry in _defaultFeatures)
                {
                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    foreach (PropertyEntry property in featureEntry.properties)
                    {
                        properties[property.key] = property.value;
                    }

                    DefineDefaultFeature(featureEntry.token, properties, featureEntry.required);
                }
            }
            else
            {
                throw new Exception("Method 'DefineFeatures()' should be called before 'Init()' only.");
            }
        }

        public virtual void DefineDefaultFeature(string token, Dictionary<string, object> properties, bool isRequired) /// Define default feature. Also called within DefineDefaultFeatures method.
        {
            _saltrConnector.DefineDefaultFeature(token, properties, isRequired);
        }

        public virtual void GetAppData() /// Loads application data from server or uses cached data.
        {
            _saltrConnector.GetAppData();
        }

        public virtual void LodLevelContent(SLTLevel level) /// Loads specified level content from Saltr or from cached data.
        {
            _saltrConnector.LoadLevelContent(level, _useCache);
        }

        public virtual void RegisterDevice() /// Device registration dialog is shown. Should be called after Init call.
        {
            if (!IsStarted)
            {
                throw new Exception("Method 'RegisterDevice()' should be called after 'Init()' only.");
            }
            else
            {
                ShowDeviceRegistationDialog(_saltrConnector.RegisterDevice);
            }
        }

        public virtual void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties = null) /// Add properties to saltr.
        {
            _saltrConnector.AddProperties(basicProperties, customProperties);
        }

        #endregion Public Methods

        #region Event Handlers

        private void SaltrConnector_LoadLevelContentSuccess(SLTLevel level)
        {
            level.RegenerateBoards();
        }
        
        private void SaltrConnector_OnDeviceRegistrationRequired()
        {
            RegisterDevice();
        }

        private void SaltrConnector_RegisterDeviceSuccess()
        {
            HideDeviceRegistationDialog();
        }

        private void SaltrConnector_GetAppDataSuccess()
        {
            if (_heartBeat != null)
            {
                StopCoroutine(_heartBeat);
            }

            _heartBeat = StartCoroutine(_saltrConnector.StartHeartBeat());
        }

        #endregion Event Handlers
       
        #region Nested Classes

        [System.Serializable]
        public class FeatureEntry /// Used to represent feature
        {
            public string token = string.Empty;
            public PropertyEntry[] properties = null;
            public bool required = false;
        }

        [System.Serializable]
        public class PropertyEntry /// Used to represent feature properties
        {
            public string key = string.Empty;
            public string value = string.Empty;
        }

        #endregion Nested Classes

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
                    if (SLTUtil.IsValidEmail(_email))
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
