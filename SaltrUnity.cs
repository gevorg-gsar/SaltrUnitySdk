using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Saltr.UnitySdk;
using Saltr.UnitySdk.Status;
using Saltr.UnitySdk.Utils;
using Plexonic.Core.Network;
using Saltr.UnitySdk.Game;

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
        private int _requestIdleTimeout = 0;

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
            gameObject.AddComponent<DownloadManager>();

            if (string.IsNullOrEmpty(_deviceId))
            {
                _deviceId = SystemInfo.deviceUniqueIdentifier;
            }

            _saltrConnector = new SaltrConnector(_clientKey, _deviceId, _useCache);

            _saltrConnector.IsDevMode = _isDevMode;
            _saltrConnector.SocialId = _socialId;

            _saltrConnector.AppDataGotten += SaltrConnector_OnConnectSuccess;
            _saltrConnector.LevelContentLoadSuccess += SaltrConnector_LevelContentLoadSuccess;

            

            //_saltrConnector.IsAutoRegisteredDevice = _isAutoRegisteredDevice;
            //_saltrConnector.UseNoLevels = _useNoLevels;
            //_saltrConnector.UseNoFeatures = _useNoFeatures;
            //_saltrConnector.RequestIdleTimeout = _requestIdleTimeout;

            if (_autoStart)
            {
                ImportLevels();
                DefineDefaultFeatures();

                RegisterDevice();

                //_GetAppData();           
            }
        }

        #endregion Messages

        #region Public Methods

        public void ImportLevels()
        {
            if (_useNoLevels)
            {
                return;
            }

            //if (!_isStarted)
            //{
            string path = !string.IsNullOrEmpty(_localLevelPacksPath) ? _localLevelPacksPath : SLTConstants.LocalLevelPacksPath;

            _saltrConnector.ImportLevels(path);
            //}
            //else
            //{
            //    throw new Exception("Method 'importLevels()' should be called before 'Start()' only.");
            //}
        }

        /// <summary>
        /// Defines features that can be specified in the Inspector.
        /// </summary>
        public virtual void DefineDefaultFeatures()
        {
            if (_useNoFeatures)
            {
                return;
            }

            //if (_isStarted == false)
            //{
            foreach (FeatureEntry featureEntry in _defaultFeatures)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                foreach (PropertyEntry property in featureEntry.properties)
                {
                    properties[property.key] = property.value;
                }

                _saltrConnector.DefineDefaultFeature(featureEntry.token, properties, featureEntry.required);
            }
            //}
            //else
            //{
            //    throw new Exception("Method 'defineFeature()' should be called before 'Start()' only.");
            //}
        }

        public void RegisterDevice()
        {
            //if (!_isStarted)
            //{
            //    throw new Exception("Method 'RegisterDevice()' should be called after 'Start()' only.");
            //}

            ShowDeviceRegistationDialog(SaltrConnector.RegisterDevice);
        }

        public void GetAppData()
        {
            _saltrConnector.GetAppData();
        }

        public void LodLevelContent()
        {
            //_saltrConnector.LoadLevelContent();
        }
        
        #endregion Public Methods

        #region Event Handlers

        private void SaltrConnector_LevelContentLoadSuccess(SLTLevel sltLevel)
        {

        }

        private void SaltrConnector_OnConnectSuccess(SLTAppData sltAppData)
        {
            _saltrConnector.LoadLevelContentFromSaltr(sltAppData.LevelPacks[0].Levels[0]); //@TODO: GOR Remove when testing is finished.
        }

        #endregion Event Handlers

        //public void LoadLevelContent(SLTLevel level, bool useCache = true)
        //{
        //    object content = null;
        //    if (_isConected == false)
        //    {
        //        if (useCache)
        //        {
        //            content = LoadLevelContentInternally(level);
        //        }
        //        else
        //        {
        //            content = LoadLevelContentFromDisk(level);
        //        }

        //        LevelContentLoadSuccessHandler(level, content);
        //    }
        //    else
        //    {
        //        if (!useCache || level.Version != GetCachedLevelVersion(level))
        //        {
        //            LoadLevelContentFromSaltr(level);
        //        }
        //        else
        //        {
        //            content = LoadLevelContentFromCache(level);
        //            LevelContentLoadSuccessHandler(level, content);
        //        }
        //    }
        //}

        //private void LoadLevelContentFromSaltr(SLTLevel level)
        //{
        //    string dataUrl = level.ContentUrl + "?_time_=" + DateTime.Now.ToShortTimeString();

        //    // some ugly stuff here came from AS3... you don't do like this in normal languages
        //    Action<SLTResource> loadFromSaltrSuccessCallback = delegate(SLTResource res)
        //    {
        //        object contentData = res.Data;
        //        if (contentData != null)
        //        {
        //            CacheLevelContent(level, contentData);
        //        }
        //        else
        //        {
        //            contentData = LoadLevelContentInternally(level);
        //        }
        //        if (contentData != null)
        //        {
        //            LevelContentLoadSuccessHandler(level, contentData);
        //        }
        //        else
        //        {
        //            LevelContentLoadFailHandler();
        //        }
        //        res.Dispose();
        //    };
        //}

        //private object LoadLevelContentInternally(SLTLevel level)
        //{
        //    object contentData = LoadLevelContentFromCache(level);
        //    if (contentData == null)
        //    {
        //        contentData = LoadLevelContentFromDisk(level);
        //    }
        //    return contentData;
        //}


        //private object LoadLevelContentFromCache(SLTLevel level)
        //{
        //    string url = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectFromCache(url);
        //}

        //private object LoadLevelContentFromDisk(SLTLevel level)
        //{
        //    string url = string.Format(SLTConstants.LocalLevelContentPackageUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectFromApplication(url);
        //}

        //private string GetCachedLevelVersion(SLTLevel level)
        //{
        //    string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectVersion(cachedFileName);
        //}

        //private void CacheLevelContent(SLTLevel level, object content)
        //{
        //    string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    _repository.CacheObject(cachedFileName, level.Version.ToString(), content);
        //}











        //private bool _isDevMode;
        //private bool _isStarted;
        //private bool _isSynced;
        //private bool _useNoLevels;
        //private bool _useNoFeatures;
        //private bool _isAutoRegisteredDevice;

        //private int _requestIdleTimeout;

        //private SLTLevelType? _levelType;
        //private ISLTRepository _repository;

        //private Dictionary<string, SLTFeature> _activeFeatures;
        //private Dictionary<string, SLTFeature> _developerFeatures;

        /// <summary>
        /// Checks if everything is initialized properly and starts the instance.
        /// After this call you can access application data (levels, features), and establish connection with the server.
        /// </summary>
        public void Start()
        {
            //if (_deviceId == null)
            //{
            //    throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            //}

            //if (_developerFeatures.Count == 0 && !_useNoFeatures)
            //{
            //    throw new Exception("Features should be defined.");
            //}

            //if (_levelPacks.Count == 0 && !_useNoLevels)
            //{
            //    throw new Exception("Levels should be imported.");
            //}

            //object cachedAppData = _repository.GetObjectFromCache(SLTConstants.AppDataUrlCache);
            //if (cachedAppData == null)
            //{
            //    foreach (var item in _developerFeatures.Keys)
            //    {
            //        _activeFeatures[item] = _developerFeatures[item];
            //    }
            //}
            //else
            //{
            //    _activeFeatures = SLTDeserializer.DeserializeFeatures(cachedAppData[SLTConstants.Features] as IEnumerable<object>);
            //    _experiments = SLTDeserializer.DeserializeExperiments(cachedAppData as Dictionary<string, object>);
            //}
            //_isStarted = true;
        }



        /// <summary>
        /// Gets the level packs.
        /// </summary>
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




        ///// <summary>
        ///// Sets the social identifier of the user.
        ///// </summary>
        //public string SocialId
        //{
        //    set { _socialId = value; }
        //}

        ///// <summary>
        ///// Sets a value indicating whether the application does not use features.
        ///// </summary>
        ///// <value><c>true</c> if features are not used; otherwise, <c>false</c>.</value>
        //public bool UseNoFeatures
        //{
        //    set { _useNoFeatures = value; }
        //}

        ///// <summary>
        ///// Sets a value indicating whether the application does not use levels.
        ///// </summary>
        ///// <value><c>true</c> if levels are not used; otherwise, <c>false</c>.</value>
        //public bool UseNoLevels
        //{
        //    set { _useNoLevels = value; }
        //}

        ///// <summary>
        ///// Sets a value indicating weather this <see cref="saltr.SLTUnity"/> should operate in dev(developer) mode.
        ///// In this mode client data(e.g. developer defined features) will be synced with Saltr, once, after successful <see cref="saltr.SLTUnity.Connect"/> call.
        ///// </summary>
        ///// <value><c>true</c> to enable; <c>false</c> to disable.</value>
        //public bool IsDevMode
        //{
        //    set { _isDevMode = value; }
        //}

        ///// <summary>
        ///// Sets a value indicating whether device registratioon dialog should be automaticaly shown in dev mode(<see cref="saltr.SLTUnity.IsDevMode"/>),
        ///// after successful <see cref="saltr.SLTUnity.connect"/> call, if the device was not registered already.
        ///// </summary>
        ///// <value><c>true</c> to enable; <c>false</c> to disable. By default is set to <c>true</c></value>
        //public bool IsAutoRegisteredDevice
        //{
        //    set { _isAutoRegisteredDevice = value; }
        //}

        ///// <summary>
        ///// Sets the request idle timeout. If a URL request takes more than timeout to complete, it would be canceled.
        ///// </summary>
        ///// <value>The request idle timeout in milliseconds.</value>
        //public int RequestIdleTimeout
        //{
        //    set { _requestIdleTimeout = value; }
        //}

        ///// <summary>
        ///// Sets the repository used by this instance. An appropriate repository is already set by a constructor,
        ///// so you will need this only if you want to implement and use your own custom repository (<see cref="saltr.Repository.ISLTRepository"/>).
        ///// </summary>        
        //internal ISLTRepository Repository
        //{
        //    set { _repository = value; }
        //}



        // If you want to have a feature synced with SALTR you should call define before getAppData call.
        /// <summary>
        /// Defines a feature (<see cref="saltr.SLTFeature"/>).
        /// </summary>
        /// <param name="token">Token - a unique identifier for the feature.</param>
        /// <param name="properties">A dictionary of properties, that should be of "JSON friendly" datatypes 
        /// (string, int, double, Dictionary, List, etc.). To represent color use standard HTML format: <c>"#RRGGBB"</c> </param>
        /// <param name="isRequired">If set to <c>true</c> feature is isRequired(see <see cref="saltr.SLTUnity.GetFeatureProperties"/>). <c>false</c> by default.</param>
        //public void DefineFeature(string token, Dictionary<string, object> properties, bool isRequired)
        //{
        //    if (_useNoFeatures)
        //    {
        //        return;
        //    }

        //    if (_isStarted == false)
        //    {
        //        _developerFeatures[token] = new SLTFeature() { Token = token, Properties = properties, IsRequired = isRequired };
        //    }
        //    else
        //    {
        //        throw new Exception("Method 'defineFeature()' should be called before 'Start()' only.");
        //    }
        //}

        ///// <summary>
        ///// See <see cref="saltr.SLTUnity.DefineFeature"/>.
        ///// </summary>
        //public void DefineFeature(string token, Dictionary<string, object> properties)
        //{
        //    DefineFeature(token, properties, false);
        //}

        

        /// <summary>
        /// Imports the level data from local files, that can be downloaded from Saltr. 
        /// If your application is using levels, this must be called before calling Start(), otherwise has no effect.
        /// </summary>
        /// <param name="path">
        /// The path to level packs in Resources folder. 
        /// If not specified the <see cref="saltr.SLTConfig.LocalLevelPackageUrl"/> will be used.
        /// </param>



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

        ///// <summary>
        ///// See <see cref="saltr.SLTUnity.ImportLevels"/>.
        ///// </summary>
        //public void ImportLevels()
        //{
        //    ImportLevels(null);
        //}







        //private object LoadLevelContentInternally(SLTLevel level)
        //{
        //    object contentData = LoadLevelContentFromCache(level);
        //    if (contentData == null)
        //    {
        //        contentData = LoadLevelContentFromDisk(level);
        //    }
        //    return contentData;
        //}

        //private object LoadLevelContentFromDisk(SLTLevel level)
        //{
        //    string url = string.Format(SLTConstants.LocalLevelContentPackageUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectFromApplication(url);
        //}

        //private object LoadLevelContentFromCache(SLTLevel level)
        //{

        //    string url = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectFromCache(url);
        //}

        //private string GetCachedLevelVersion(SLTLevel level)
        //{
        //    string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    return _repository.GetObjectVersion(cachedFileName);
        //}

        //private void CacheLevelContent(SLTLevel level, object content)
        //{
        //    string cachedFileName = string.Format(SLTConstants.LocalLevelContentCacheUrlTemplate, level.PackIndex.ToString(), level.LocalIndex.ToString());
        //    _repository.CacheObject(cachedFileName, level.Version.ToString(), content);
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
