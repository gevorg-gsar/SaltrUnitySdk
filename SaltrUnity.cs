using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Saltr.UnitySdk;
using Saltr.UnitySdk.Utils;
using Plexonic.Core.Network;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Domain;
using Newtonsoft.Json;

namespace Saltr.UnitySdk
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
        private bool _autoStart = false;

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
        private int _timeout = 0;

        [SerializeField]
        private string _localLevelPacksPath = SLTConstants.LocalLevelPacksPath;

        [SerializeField]
        private FeatureEntry[] _defaultFeatures = null;

        #endregion Fields

        #region Properties

        public SaltrConnector SaltrConnector
        {
            get { return _saltrConnector; }
            protected set { _saltrConnector = value; }
        }

        public bool IsStarted { get; private set; }

        public string SocialId
        {
            set { _socialId = value; }
            get { return _socialId; }
        }

        public bool UseNoFeatures
        {
            set { _useNoFeatures = value; }
            get { return _useNoFeatures; }
        }

        public bool UseNoLevels
        {
            set { _useNoLevels = value; }
            get { return _useNoLevels; }
        }

        public bool IsDevMode
        {
            set { _isDevMode = value; }
            get { return _isDevMode; }
        }

        public bool IsAutoRegisteredDevice
        {
            set { _isAutoRegisteredDevice = value; }
            get { return _isAutoRegisteredDevice; }
        }

        //public int RequestIdleTimeout
        //{
        //    set { _requestIdleTimeout = value; }
        //}

        #endregion

        #region Events

        public event Action DeviceRegistrationRequired
        {
            add { _saltrConnector.DeviceRegistrationRequired += value; }
            remove { _saltrConnector.DeviceRegistrationRequired -= value; }
        }

        public event Action<SLTAppData> GetAppDataSuccess
        {
            add { _saltrConnector.GetAppDataSuccess += value; }
            remove { _saltrConnector.GetAppDataSuccess -= value; }
        }

        public event Action<SLTErrorStatus> GetAppDataFail
        {
            add { _saltrConnector.GetAppDataFail += value; }
            remove { _saltrConnector.GetAppDataFail -= value; }
        }

        public event Action<SLTLevel> LoadLevelContentSuccess
        {
            add { _saltrConnector.LoadLevelContentSuccess += value; }
            remove { _saltrConnector.LoadLevelContentSuccess -= value; }
        }

        public event Action<SLTErrorStatus> LoadLevelConnectFail
        {
            add { _saltrConnector.LoadLevelConnectFail += value; }
            remove { _saltrConnector.LoadLevelConnectFail -= value; }
        }

        public event Action RegisterDeviceSuccess
        {
            add { _saltrConnector.RegisterDeviceSuccess += value; }
            remove { _saltrConnector.RegisterDeviceSuccess -= value; }
        }

        public event Action<SLTErrorStatus> RegisterDeviceFail
        {
            add { _saltrConnector.RegisterDeviceFail += value; }
            remove { _saltrConnector.RegisterDeviceFail -= value; }
        }

        public event Action AddPropertiesSuccess
        {
            add { _saltrConnector.AddPropertiesSuccess += value; }
            remove { _saltrConnector.AddPropertiesSuccess -= value; }
        }

        public event Action<SLTErrorStatus> AddPropertiesFail
        {
            add { _saltrConnector.AddPropertiesFail += value; }
            remove { _saltrConnector.AddPropertiesFail -= value; }
        }

        #endregion Events

        #region MonoBehaviour

        protected virtual void Awake()
        {
            if (_saltrConnector != null)
                return;

            gameObject.name = SLTConstants.SaltrGameObjectName;
            gameObject.AddComponent<DownloadManager>();

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
                        
            _saltrConnector.RegisterDeviceSuccess += SaltrConnector_RegisterDeviceSuccess;
            _saltrConnector.DeviceRegistrationRequired += SaltrConnector_OnDeviceRegistrationRequired;

        }
        
        protected virtual void Start()
        {
            if (_autoStart)
            {
                ImportLevels(_localLevelPacksPath);
                DefineDefaultFeatures(_defaultFeatures);

                Init();
            }
            GetAppData();
            //RegisterDevice();
            //_saltrConnector.Sync();
        }

        #endregion Monobehaviour

        #region Public Methods

        public virtual void Init()
        {
            if (_saltrConnector == null)
            {
                throw new Exception(ExceptionConstants.SaltrConnectorShouldBeCreated);
            }

            _saltrConnector.Init();
            IsStarted = true;
        }

        public virtual void ImportLevels(string path)
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

        public virtual void DefineDefaultFeatures(FeatureEntry[] features)
        {
            if (_useNoFeatures)
            {
                return;
            }

            if (!IsStarted)
            {
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

        public virtual void DefineDefaultFeature(string token, Dictionary<string, object> properties, bool isRequired)
        {
            _saltrConnector.DefineDefaultFeature(token, properties, isRequired);
        }

        public virtual void GetAppData()
        {
            _saltrConnector.GetAppData();
        }

        public virtual void LodLevelContent(SLTLevel level, bool useCache = true)
        {
            _saltrConnector.LoadLevelContent(level, useCache);
        }

        public virtual void RegisterDevice()
        {
            if (!IsStarted)
            {
                throw new Exception("Method 'RegisterDevice()' should be called after 'Init()' only.");
            }
            else
            {
                ShowDeviceRegistationDialog(SaltrConnector.RegisterDevice);
            }
        }

        public virtual void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties = null)
        {
            _saltrConnector.AddProperties(basicProperties, customProperties);
        }

        #endregion Public Methods

        #region Event Handlers


        //private void SaltrConnector_OnGetAppDataSuccess(SLTAppData sltAppData)
        //{
        //    _saltrConnector.LoadLevelContent(sltAppData.LevelPacks[0].Levels[0]); //@TODO: GOR Remove when testing is finished.
        //}


        private void SaltrConnector_OnDeviceRegistrationRequired()
        {
            RegisterDevice();
        }

        private void SaltrConnector_RegisterDeviceSuccess()
        {
            HideDeviceRegistationDialog();
        }

        #endregion Event Handlers



        //public List<SLTLevelPack> LevelPacks
        //{
        //    get { return _levelPacks; }
        //}

        /// <summary>
        /// Gets a list all levels.
        /// </summary>
        //public List<SLTLevel> AllLevels
        //{
        //    get
        //    {
        //        List<SLTLevel> allLevels = new List<SLTLevel>();
        //        for (int i = 0; i < _levelPacks.Count; i++)
        //        {
        //            List<SLTLevel> packLevels = _levelPacks[i].Levels;
        //            for (int j = 0; j < packLevels.Count; j++)
        //            {
        //                allLevels.Add(packLevels[j]);
        //            }
        //        }
        //        return allLevels;
        //    }
        //}

        /// <summary>
        /// Gets the count of all levels.
        /// </summary>
        //public uint AllLevelsCount
        //{
        //    get
        //    {
        //        uint count = 0;
        //        for (int i = 0; i < _levelPacks.Count; i++)
        //        {
        //            count += (uint)_levelPacks[i].Levels.Count;
        //        }
        //        return count;
        //    }
        //}

        ///// <summary>
        ///// Gets a list of all experiments.
        ///// </summary>
        //public List<SLTExperiment> Experiments
        //{
        //    get { return _experiments; }
        //}


        /// <summary>
        /// Gets a level by its global index in all levels.
        /// </summary>
        /// <param name="index">Index in all levels.</param>
        //public SLTLevel GetLevelByGlobalIndex(int index)
        //{
        //    int levelSum = 0;
        //    foreach (var levelPack in _levelPacks)
        //    {
        //        int packLenght = levelPack.Levels.Count;
        //        if (index >= levelSum + packLenght)
        //        {
        //            levelSum += packLenght;
        //        }
        //        else
        //        {
        //            int localIndex = index - levelSum;
        //            return levelPack.Levels[localIndex];
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets the level pack that contains the level with given global index in all levels.
        ///// </summary>
        ///// <param name="index">Index of the level in all levels.</param>
        //public SLTLevelPack GetPackByLevelGlobalIndex(int index)
        //{
        //    int levelSum = 0;
        //    foreach (var levelPack in _levelPacks)
        //    {
        //        int packLenght = levelPack.Levels.Count;
        //        if (index >= levelSum + packLenght)
        //        {
        //            levelSum += packLenght;
        //        }

        //        else
        //        {
        //            return levelPack;
        //        }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Gets a list of tokens(unique identifiers) of all features, active in Saltr.
        ///// </summary>
        //public List<string> GetActiveFeatureTokens()
        //{
        //    var tokens = from feature in _activeFeatures.Values
        //                 where feature != null && feature.Token != null
        //                 select feature.Token;

        //    return tokens.ToList<string>();
        //}

        ///// <summary>
        ///// Gets the properties of the feature specified by the token. 
        ///// If a feature is set to be isRequired and is not active in, or cannot be retrieved from, Saltr, 
        ///// the properties will be retrieved from default developer defined features.
        ///// </summary>
        ///// <param name="token">The feature token.</param>
        //public Dictionary<string, object> GetFeatureProperties(string token)
        //{
        //    if (_activeFeatures.ContainsKey(token))
        //    {
        //        return (_activeFeatures[token]).Properties as Dictionary<string, object>;
        //    }
        //    else
        //        if (_developerFeatures.ContainsKey(token))
        //        {
        //            SLTFeature devFeature = _developerFeatures[token];
        //            if (devFeature != null && devFeature.Required)
        //            {
        //                return devFeature.Properties as Dictionary<string, object>;
        //            }
        //        }

        //    return null;
        //}

        

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
