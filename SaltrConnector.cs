using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Saltr.UnitySdk.Repository;
using Saltr.UnitySdk.Domain.InternalModel;
using Saltr.UnitySdk.Utils;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using Saltr.UnitySdk.Domain.Model;
using Saltr.UnitySdk.Network;
using Newtonsoft.Json.Converters;

namespace Saltr.UnitySdk
{
    //TODO:: @daal add some flushCache method.
    public class SaltrConnector
    {
        #region Constants

        public const string Client = "Unity";
        public const string ApiVersion = "1.0.0"; //"0.9.0";
        private const string LevelContentUrlFormat = @"{0}?_time_{1}";

        #endregion Constants

        #region Fields

        private string _deviceId;
        private string _clientKey;

        private bool _useCache;
        private bool _isSynced;
        private bool _isLoading;
        private bool _isAppDataGotten;

        private bool _isHeartBeatEnabled;

        private ISLTRepository _repository;
        private List<SLTLevelPack> _levelPacks;
        private List<SLTExperiment> _experiments;
        private Dictionary<string, SLTFeature> _activeFeatures;
        private Dictionary<string, SLTFeature> _defaultFeatures;

        #endregion Fields

        #region Properties

        public SLTAppData AppData { get; private set; }

        public bool IsDevMode { get; set; }

        public string SocialId { get; set; }

        public bool UseNoLevels { get; set; }

        public bool UseNoFeatures { get; set; }

        public bool IsAutoRegisteredDevice { get; set; }

        public List<SLTLevelPack> LevelPacks { get { return _levelPacks; } }

        public List<SLTExperiment> Experiments { get { return _experiments; } }

        public Dictionary<string, SLTFeature> DefaultFeatures { get { return _defaultFeatures; } }

        public Dictionary<string, SLTFeature> ActiveFeatures { get { return _activeFeatures; } }

        #endregion Properties

        #region Events

        public event Action DeviceRegistrationRequired = delegate() { };

        public event Action<SLTAppData> GetAppDataSuccess = delegate(SLTAppData appData) { };
        public event Action<SLTErrorStatus> GetAppDataFail = delegate(SLTErrorStatus errorStatus) { };

        public event Action<SLTLevel> LoadLevelContentSuccess = delegate(SLTLevel sltLevel) { };
        public event Action<SLTErrorStatus> LoadLevelConnectFail = delegate(SLTErrorStatus errorStatus) { };

        public event Action RegisterDeviceSuccess = delegate() { };
        public event Action<SLTErrorStatus> RegisterDeviceFail = delegate(SLTErrorStatus errorStatus) { };

        public event Action AddPropertiesSuccess = delegate() { };
        public event Action<SLTErrorStatus> AddPropertiesFail = delegate(SLTErrorStatus errorStatus) { };

        #endregion Events

        #region Ctor

        public SaltrConnector(string clientKey, string deviceId, bool useCache = true)
        {
            _useCache = useCache;

            _clientKey = clientKey;
            _deviceId = deviceId;
            _isLoading = false;
            _isAppDataGotten = false;

            _isHeartBeatEnabled = false;

            _levelPacks = new List<SLTLevelPack>();
            _experiments = new List<SLTExperiment>();
            _activeFeatures = new Dictionary<string, SLTFeature>();
            _defaultFeatures = new Dictionary<string, SLTFeature>();

            if (useCache)
            {
                _repository = new SLTMobileRepository();
            }
            else
            {
                _repository = new SLTDummyRepository();
            }
        }

        #endregion Ctor

        #region Public Methods

        public void ImportLevels(string path)
        {
            path = string.IsNullOrEmpty(path) ? SLTConstants.LocalLevelPacksPath : path;
            AppData = _repository.GetObjectFromApplication<SLTAppData>(path);

            _levelPacks = AppData.LevelPacks;
        }

        public void DefineDefaultFeature(string featureToken, Dictionary<string, object> properties, bool isRequired)
        {
            Regex regex = new Regex(SLTConstants.RegexPatternFeatureToken);
            Match match = regex.Match(featureToken);

            if (match.Success)
            {
                _defaultFeatures[featureToken] = new SLTFeature() { Token = featureToken, Properties = properties, IsRequired = isRequired };
            }
            else
            {
                throw new Exception(SLTExceptionConstants.InvalidToken);
            }
        }

        public void Init()
        {
            if (_deviceId == null)
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            //if (AppData == null)
            //{
            //    throw new Exception(ExceptionConstants.AppDataShouldBeInitialized);
            //}

            if (_levelPacks.IsNullOrEmpty<SLTLevelPack>() && !UseNoLevels)
            {
                throw new Exception(SLTExceptionConstants.LevelsShouldBeImported);
            }

            if ((_defaultFeatures == null || _defaultFeatures.Count == 0) && !UseNoFeatures)
            {
                throw new Exception(SLTExceptionConstants.FeaturesShouldBeDefined);
            }

            SLTAppData cachedAppData = _repository.GetObjectFromCache<SLTAppData>(SLTConstants.AppDataCacheFileName);
            _experiments.Clear();
            _activeFeatures.Clear();

            if (cachedAppData == null)
            {
                if (!UseNoFeatures)
                {
                    foreach (var item in _defaultFeatures)
                    {
                        _activeFeatures.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                if (!UseNoFeatures && !cachedAppData.Features.IsNullOrEmpty<SLTFeature>())
                {
                    foreach (var feature in cachedAppData.Features)
                    {
                        _activeFeatures.Add(feature.Token, feature);
                    }
                }

                if (!cachedAppData.Experiments.IsNullOrEmpty<SLTExperiment>())
                {
                    _experiments = cachedAppData.Experiments;
                }
            }
        }

        public void GetAppData(SLTBasicProperties basicProperties = null, Dictionary<string, object> customProperties = null)
        {
            if (_isLoading)
            {
                GetAppDataFail(new SLTErrorStatus { Message = SLTExceptionConstants.SaltrAppDataLoadRefused });
                return;
            }

            _isLoading = true;

            var urlVars = PrepareAppDataRequestParameters(basicProperties, customProperties);
            var url = FillRequestPrameters(SLTConstants.SaltrApiUrl, urlVars);

            SLTDownloadManager.Instance.AddDownload(url, OnAppDataGotten);
        }

        public void LoadLevelContent(SLTLevel level, bool useCache = true)
        {
            if (_isAppDataGotten &&
                (!useCache || (level != null && level.Version != null && level.Version.ToString() != GetCachedLevelVersion(level))))
            {
                LoadLevelContentFromSaltr(level);
            }
            else
            {
                level.InternalLevelContent = LoadLevelContentLocally(level, useCache);

                LoadLevelContentSuccess(level);
            }
        }

        public void Sync()
        {
            var urlVars = PrepareSyncRequestParameters();
            var url = FillRequestPrameters(SLTConstants.SaltrDevApiUrl, urlVars);

            SLTDownloadManager.Instance.AddDownload(url, OnSync);
        }

        public void RegisterDevice(string email)
        {
            var urlVars = PrepareRegisterDeviceRequestParameters(email);
            var url = FillRequestPrameters(SLTConstants.SaltrDevApiUrl, urlVars);

            SLTDownloadManager.Instance.AddDownload(url, OnRegisterDevice);
        }

        public void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties = null)
        {
            if (basicProperties == null && customProperties == null)
            {
                return;
            }

            var urlVars = PrepareAddPropertiesRequestParameters(basicProperties, customProperties);
            var url = FillRequestPrameters(SLTConstants.SaltrApiUrl, urlVars);

            SLTDownloadManager.Instance.AddDownload(url, OnAddProperties);
        }

        public IEnumerator StartHeartBeat(int seconds = 2)
        {
            _isHeartBeatEnabled = true;

            while (_isHeartBeatEnabled)
            {
                yield return new WaitForSeconds(seconds);

                var urlVars = PrepareHeartBeatRequestParameters();
                var url = FillRequestPrameters(SLTConstants.SaltrApiUrl, urlVars);

                SLTDownloadManager.Instance.AddDownload(url, OnHeartBeat);
            }
        }

        #endregion Public Methods

        #region Private Methods

        #region LevelContent

        private void CacheLevelContent(SLTLevel level)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            _repository.CacheObject<SLTInternalLevelContent>(cachedFileName, level.InternalLevelContent, level.Version.ToString());
        }

        private void LoadLevelContentFromSaltr(SLTLevel level)
        {
            string levelContentUrl = string.Format(LevelContentUrlFormat, level.Url, DateTime.Now.ToShortTimeString());

            SLTDownloadManager.Instance.AddDownload(new SLTDownloadRequest(levelContentUrl, OnLoadLevelContentFromSaltr) { StateObject = level });
        }

        private SLTInternalLevelContent LoadLevelContentLocally(SLTLevel level, bool useCache = true)
        {
            SLTInternalLevelContent levelContent = null;

            if (useCache)
            {
                levelContent = LoadLevelContentFromCache(level);
            }

            if (levelContent == null)
            {
                levelContent = LoadLevelContentFromApplication(level);
            }

            return levelContent;
        }

        private SLTInternalLevelContent LoadLevelContentFromCache(SLTLevel level)
        {
            string path = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromCache<SLTInternalLevelContent>(path);
        }

        private SLTInternalLevelContent LoadLevelContentFromApplication(SLTLevel level)
        {
            string path = string.Format(SLTConstants.LocalLevelContentPathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromApplication<SLTInternalLevelContent>(path);
        }

        private string GetCachedLevelVersion(SLTLevel level)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectVersion(cachedFileName);
        }

        #endregion LevelContent

        #region Request Parameters

        private Dictionary<string, string> PrepareAppDataRequestParameters(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionGetAppData;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.ClientKey = _clientKey;
            args.Client = Client;

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            if (SocialId != null)
            {
                args.SocialId = SocialId;
            }

            if (basicProperties != null)
            {
                args.BasicProperties = basicProperties;
            }

            if (customProperties != null)
            {
                args.CustomProperties = customProperties;
            }

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
            };
            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData, Formatting.None, settings);

            return urlVars;
        }

        private Dictionary<string, string> PrepareSyncRequestParameters()
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionDevSync;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.Client = Client;
            args.ClientKey = _clientKey;

            args.IsDevMode = IsDevMode;

            if (!string.IsNullOrEmpty(SocialId))
            {
                args.SocialId = SocialId;
            }

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
                urlVars[SLTConstants.DeviceId] = _deviceId;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            if (_defaultFeatures != null)
            {
                args.DeveloperFeatures = _defaultFeatures.Values.ToList<SLTFeature>();
            }

            urlVars[SLTConstants.UrlParamDevMode] = IsDevMode.ToString();

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
            };

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData, Formatting.None, settings);

            return urlVars;
        }

        private Dictionary<string, string> PrepareRegisterDeviceRequestParameters(string email)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionDevRegisterDevice;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.ClientKey = _clientKey;

            args.IsDevMode = IsDevMode;

            if (!string.IsNullOrEmpty(_deviceId))
            {
                args.Id = _deviceId;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            if (!string.IsNullOrEmpty(email))
            {
                args.Email = email;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.EmailIsRequired);
            }

            string deviceModel = SLTConstants.Unknown;
            string os = SLTConstants.Unknown;

            //@TODO: Gor check if os value is retrived correctly.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                deviceModel = SLTUtil.GetHumanReadableIOSDeviceModel(SystemInfo.deviceModel);
                os = SystemInfo.operatingSystem.Replace("Phone ", string.Empty);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                deviceModel = SystemInfo.deviceModel;

                string androidVersion = string.Empty;

                try
                {
                    androidVersion = SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.IndexOf("API-") + 4, 2).ToString();
                }
                catch
                {
                    androidVersion = SLTConstants.Unknown;
                }

                os = "Android " + androidVersion;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                deviceModel = "PC";
                os = SystemInfo.operatingSystem;
            }
            else
            {
                deviceModel = SystemInfo.deviceModel;
                os = SystemInfo.operatingSystem;
            }

            args.Source = deviceModel;
            args.OS = os;

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
            };

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData, Formatting.None, settings);

            return urlVars;
        }

        private Dictionary<string, string> PrepareAddPropertiesRequestParameters(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties = null)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionAddProperties;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.Client = Client;
            args.ClientKey = _clientKey;

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            if (SocialId != null)
            {
                args.SocialId = SocialId;
            }

            if (basicProperties != null)
            {
                args.BasicProperties = basicProperties;
            }

            if (customProperties != null)
            {
                args.CustomProperties = customProperties;
            }

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
            };

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData, Formatting.None, settings);

            return urlVars;
        }

        private Dictionary<string, string> PrepareHeartBeatRequestParameters()
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionHeartBeat;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.Client = Client;
            args.ClientKey = _clientKey;

            if (!string.IsNullOrEmpty(SocialId))
            {
                args.SocialId = SocialId;
            }

            if (_deviceId != null)
            {
                args.DeviceId = _deviceId;
                urlVars[SLTConstants.DeviceId] = _deviceId;
            }
            else
            {
                throw new Exception(SLTExceptionConstants.DeviceIdIsRequired);
            }

            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesExceptDictionaryKeysContractResolver()
            };

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData, Formatting.None, settings);

            return urlVars;
        }

        private string FillRequestPrameters(string url, Dictionary<string, string> parameters)
        {
            if (parameters != null)
            {
                char seperator = '?';
                foreach (var parameter in parameters)
                {
                    url += seperator;
                    url += parameter.Key + "=" + WWW.EscapeURL(parameter.Value);
                    if ('?' == seperator)
                    {
                        seperator = '&';
                    }
                }
            }

            return url;
        }

        #endregion Request Parameters

        #endregion Private Methods

        #region Handlers

        private void OnAppDataGotten(SLTDownloadResult result)
        {
            SLTAppData sltAppData = null;

            SLTResponse<SLTAppData> response = JsonConvert.DeserializeObject<SLTResponse<SLTAppData>>(result.Text, new DictionaryConverter());

            if (response != null && !response.Response.IsNullOrEmpty<SLTAppData>())
            {
                sltAppData = response.Response.FirstOrDefault<SLTAppData>();

                if (sltAppData != null && sltAppData.Success.HasValue && sltAppData.Success.Value)
                {
                    if (IsDevMode && !_isSynced && !UseNoFeatures)
                    {
                        Sync();
                    }

                    if (!UseNoLevels && !sltAppData.LevelPacks.IsNullOrEmpty<SLTLevelPack>())
                    {
                        _levelPacks = sltAppData.LevelPacks;
                    }
                    else
                    {
                        sltAppData.LevelPacks = null;
                    }

                    if (!UseNoFeatures && !sltAppData.Features.IsNullOrEmpty<SLTFeature>())
                    {
                        _activeFeatures.Clear();
                        foreach (var feature in sltAppData.Features)
                        {
                            _activeFeatures.Add(feature.Token, feature);
                        }
                    }
                    else
                    {
                        sltAppData.Features = null;
                    }

                    _experiments = sltAppData.Experiments;

                    _isAppDataGotten = true;
                    _repository.CacheObject<SLTAppData>(SLTConstants.AppDataCacheFileName, sltAppData);

                    GetAppDataSuccess(sltAppData);

                    Debug.Log("[SALTR] AppData load success.");

                }
                else if (sltAppData.Error != null)
                {
                    GetAppDataFail(sltAppData.Error);
                }
                else
                {
                    GetAppDataFail(new SLTErrorStatus() { Message = result.Text });
                }
            }
            else
            {
                GetAppDataFail(new SLTErrorStatus() { Message = result.Text });
            }

            _isLoading = false;
        }

        private void OnLoadLevelContentFromSaltr(SLTDownloadResult result)
        {
            SLTLevel level = result.StateObject as SLTLevel;

            level.InternalLevelContent = JsonConvert.DeserializeObject<SLTInternalLevelContent>(result.Text, new BoardConverter(), new SLTAssetTypeConverter(), new DictionaryConverter());

            if (level.InternalLevelContent != null)
            {
                CacheLevelContent(level);
            }
            else
            {
                level.InternalLevelContent = LoadLevelContentLocally(level);
            }

            if (level.InternalLevelContent != null)
            {
                LoadLevelContentSuccess(level);
                return;
            }
            else
            {
                LoadLevelConnectFail(new SLTErrorStatus() { Message = result.Text });
            }
        }

        private void OnSync(SLTDownloadResult result)
        {
            SLTResponse<SLTBaseEntity> response = JsonConvert.DeserializeObject<SLTResponse<SLTBaseEntity>>(result.Text);

            if (response != null && !response.Response.IsNullOrEmpty<SLTBaseEntity>())
            {
                SLTBaseEntity sltStatusEntity = response.Response.FirstOrDefault<SLTBaseEntity>();

                if (sltStatusEntity.Success.HasValue && !sltStatusEntity.Success.Value)
                {
                    if (IsAutoRegisteredDevice
                        && sltStatusEntity.Error != null
                        && sltStatusEntity.Error.Code == SLTErrorStatusCode.RegistrationRequired)
                    {
                        DeviceRegistrationRequired();
                    }
                }
                else
                {
                    _isSynced = true;
                }
            }
        }

        private void OnHeartBeat(SLTDownloadResult result)
        {
            SLTResponse<SLTBaseEntity> response = JsonConvert.DeserializeObject<SLTResponse<SLTBaseEntity>>(result.Text);

            if (response != null && !response.Response.IsNullOrEmpty<SLTBaseEntity>())
            {
                SLTBaseEntity sltStatusEntity = response.Response.FirstOrDefault<SLTBaseEntity>();

                if (!sltStatusEntity.Success.HasValue || (sltStatusEntity.Success.HasValue && !sltStatusEntity.Success.Value))
                {
                    _isHeartBeatEnabled = false;
                }
                else
                {
                    _isHeartBeatEnabled = true;
                }
            }
        }

        private void OnRegisterDevice(SLTDownloadResult result)
        {
            SLTResponse<SLTBaseEntity> response = JsonConvert.DeserializeObject<SLTResponse<SLTBaseEntity>>(result.Text);

            Debug.Log("[Saltr] Dev register new device is complete.");

            if (response != null && !response.Response.IsNullOrEmpty<SLTBaseEntity>())
            {
                SLTBaseEntity sltStatusEntity = response.Response.FirstOrDefault<SLTBaseEntity>();

                if (sltStatusEntity.Success.HasValue && sltStatusEntity.Success.Value)
                {
                    RegisterDeviceSuccess();

                    Sync();
                }
                else if (sltStatusEntity.Error != null)
                {
                    RegisterDeviceFail(sltStatusEntity.Error);
                }
            }
            else
            {
                RegisterDeviceFail(new SLTErrorStatus() { Code = SLTErrorStatusCode.UnknownError, Message = result.Text });
            }
        }

        private void OnAddProperties(SLTDownloadResult result)
        {
            SLTResponse<SLTBaseEntity> response = JsonConvert.DeserializeObject<SLTResponse<SLTBaseEntity>>(result.Text, new DictionaryConverter());

            if (response != null && !response.Response.IsNullOrEmpty<SLTBaseEntity>())
            {
                SLTBaseEntity sltStatusEntity = response.Response.FirstOrDefault<SLTBaseEntity>();

                if (sltStatusEntity.Success.HasValue && sltStatusEntity.Success.Value)
                {
                    Debug.Log("[SALTR] Success: Add Properties");
                    AddPropertiesSuccess();
                }
                else
                {
                    if (sltStatusEntity.Error != null)
                    {
                        AddPropertiesFail(sltStatusEntity.Error);
                    }
                    else
                    {
                        Debug.Log("[SALTR] Fail: Add Properties");
                        AddPropertiesFail(new SLTErrorStatus() { Message = result.Text });
                    }
                }

            }
        }

        #endregion Handlers

    }

}