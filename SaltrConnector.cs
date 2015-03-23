using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Saltr.UnitySdk.Repository;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Utils;
using Plexonic.Core.Network;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain;

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

        private bool _isSynced;
        private bool _isLoading;
        private bool _isAppDataGotten;

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

        #endregion Properties

        #region Events

        public event Action DeviceRegistrationRequired = delegate() { };

        public event Action<SLTAppData> GetAppDataSuccess = delegate(SLTAppData appData) { };
        public event Action<SLTErrorStatus> GetAppDataFail = delegate(SLTErrorStatus errorStatus) { };

        public event Action<SLTLevel> LoadLevelContentSuccess = delegate(SLTLevel sltLevel) { };
        public event Action<SLTErrorStatus> LoadLevelConnectFail = delegate(SLTErrorStatus errorStatus) { };

        public event Action RegisterDeviceSuccess = delegate() { };
        public event Action<SLTErrorStatus> RegisterDeviceFail = delegate(SLTErrorStatus errorStatus) { };

        #endregion Events

        #region Ctor

        public SaltrConnector(string clientKey, string deviceId, bool useCache = true)
        {
            _clientKey = clientKey;
            _deviceId = deviceId;
            _isLoading = false;
            _isAppDataGotten = false;

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
            if (!string.IsNullOrEmpty(featureToken))
            {
                _defaultFeatures[featureToken] = new SLTFeature() { Token = featureToken, Properties = properties, IsRequired = isRequired };
            }
        }

        public void Init()
        {
            if (_deviceId == null)
            {
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            //if (AppData == null)
            //{
            //    throw new Exception(ExceptionConstants.AppDataShouldBeInitialized);
            //}

            if (_levelPacks.Count == 0 && !UseNoLevels)
            {
                throw new Exception(ExceptionConstants.LevelsShouldBeImported);
            }

            if (_defaultFeatures.Count == 0 && !UseNoFeatures)
            {
                throw new Exception(ExceptionConstants.FeaturesShouldBeDefined);
            }

            SLTAppData cachedAppData = _repository.GetObjectFromCache<SLTAppData>(SLTConstants.AppDataCacheFileName);
            _experiments.Clear();
            _activeFeatures.Clear();

            if (cachedAppData == null)
            {
                foreach (var item in _defaultFeatures)
                {
                    _activeFeatures.Add(item.Key, item.Value);
                }
            }
            else
            {
                if (!cachedAppData.Features.IsNullOrEmpty<SLTFeature>())
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
                OnGetAppDataFail(new DownloadResult(ExceptionConstants.SaltrAppDataLoadRefused));
                return;
            }

            _isLoading = true;

            var urlVars = PrepareAppDataRequestParameters(basicProperties, customProperties);
            var url = FillRequestPrameters(SLTConstants.SaltrApiUrl, urlVars);

            DownloadManager.Instance.AddDownload(url, OnAppDataGotten);
        }

        public void LoadLevelContent(SLTLevel level, bool useCache = true)
        {
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
        }

        public void Sync()
        {
            var urlVars = PrepareSyncRequestParameters();
            var url = FillRequestPrameters(SLTConstants.SaltrDevApiUrl, urlVars);

            DownloadManager.Instance.AddDownload(url, OnSync);
        }

        public void RegisterDevice(string email)
        {
            var urlVars = PrepareRegisterDeviceRequestParameters(email);
            var url = FillRequestPrameters(SLTConstants.SaltrDevApiUrl, urlVars);

            DownloadManager.Instance.AddDownload(url, OnRegisterDevice);
        }

        public void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties = null)
        {
            if (basicProperties == null && customProperties == null)
            {
                return;
            }

            var urlVars = PrepareAddPropertiesRequestParameters(basicProperties, customProperties);
            var url = FillRequestPrameters(SLTConstants.SaltrApiUrl, urlVars);

            DownloadManager.Instance.AddDownload(url, OnAddProperties);
        }

        #endregion Public Methods

        #region Private Methods

        private void LoadLevelContentFromSaltr(SLTLevel level)
        {
            string levelContentUrl = string.Format(LevelContentUrlFormat, level.Url, DateTime.Now.ToShortTimeString());

            DownloadManager.Instance.AddDownload(levelContentUrl, OnLevelContentLoad);
        }

        private SLTLevelContent GetLevelContentLocally(SLTLevel level)
        {
            SLTLevelContent levelContent = GetLevelContentFromCache(level);
            if (levelContent == null)
            {
                levelContent = GetLevelContentFromApplication(level);
            }
            return levelContent;
        }

        private void CacheLevelContent(SLTLevel level)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            _repository.CacheObject<SLTLevelContent>(cachedFileName, level.Content, level.Version.ToString());
        }

        private SLTLevelContent GetLevelContentFromCache(SLTLevel level)
        {
            string path = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromCache<SLTLevelContent>(path);
        }

        private SLTLevelContent GetLevelContentFromApplication(SLTLevel level)
        {
            string path = string.Format(SLTConstants.LocalLevelContentPathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectFromApplication<SLTLevelContent>(path);
        }

        private string GetCachedLevelVersion(SLTLevel level)
        {
            string cachedFileName = string.Format(SLTConstants.LocalLevelContentCachePathFormat, level.PackIndex.ToString(), level.LocalIndex.ToString());
            return _repository.GetObjectVersion(cachedFileName);
        }

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
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
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

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData);

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
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            if (_defaultFeatures != null)
            {
                args.DeveloperFeatures = _defaultFeatures.Values.ToList<SLTFeature>();
            }

            urlVars[SLTConstants.UrlParamDevMode] = IsDevMode.ToString();
            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData);

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
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
            }

            if (!string.IsNullOrEmpty(email))
            {
                args.Email = email;
            }
            else
            {
                throw new Exception(ExceptionConstants.EmailIsRequired);
            }

            string deviceModel = SLTConstants.Unknown;
            string os = SLTConstants.Unknown;

            //@TODO: Gor check if os value is retrived correctly.
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                deviceModel = Util.GetHumanReadableIOSDeviceModel(SystemInfo.deviceModel);
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

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData);

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
                throw new Exception(ExceptionConstants.DeviceIdIsRequired);
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

            urlVars[SLTConstants.UrlParamArguments] = JsonConvert.SerializeObject(args.RawData);

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

        #endregion Private Methods

        #region Handlers

        private void OnAppDataGotten(DownloadResult result)
        {
            SLTAppData sltAppData = null;

            if (result == null || string.IsNullOrEmpty(result.Text))
            {
                OnGetAppDataFail(result);
            }

            SLTResponse<SLTAppData> response = JsonConvert.DeserializeObject<SLTResponse<SLTAppData>>(result.Text);

            if (response != null && !response.Response.IsNullOrEmpty<SLTAppData>())
            {
                sltAppData = response.Response.FirstOrDefault<SLTAppData>();

                if (sltAppData != null && sltAppData.Success.HasValue && sltAppData.Success.Value)
                {
                    _isAppDataGotten = true;
                    _repository.CacheObject(SLTConstants.AppDataCacheFileName, sltAppData);

                    Debug.Log("[SALTR] AppData load success.");

                    if (IsDevMode && !_isSynced)
                    {
                        Sync();
                    }

                    if (GetAppDataSuccess != null)
                    {
                        GetAppDataSuccess(sltAppData);
                    }

                    //// if developer didn't announce use without levels, and levelType in returned JSON is not "noLevels",
                    //// then - parse levels
                    //if (!_useNoLevels && _levelType != SLTLevelType.NoLevels)
                    //{
                    //    List<SLTLevelPack> newLevelPacks = null;
                    //    try
                    //    {
                    //        newLevelPacks = SLTDeserializer.DecodeLevels(response);
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Debug.Log(e.Message);
                    //        _onConnectFail(new SLTStatusLevelsParseError());
                    //        return;
                    //    }

                    //    // if new levels are received and parsed, then only dispose old ones and assign new ones.
                    //    if (newLevelPacks != null)
                    //    {
                    //        DisposeLevelPacks();
                    //        _levelPacks = newLevelPacks;
                    //    }
                    //}

                }
                else
                {
                    //if (responseData.ContainsKey(SLTConstants.Error))
                    //{
                    //OnConnectFail(new SLTStatus(int.Parse(response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Code)), response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Message)));
                    //}
                    //else
                    //{
                    //int errorCode;
                    //int.TryParse(response.GetValue<string>(SLTConstants.ErrorCode), out errorCode);

                    //OnConnectFail(new SLTStatus(errorCode, response.GetValue<string>(SLTConstants.ErrorMessage)));
                    //}
                }
            }
        }

        private void OnGetAppDataFail(DownloadResult result)
        {
            if (GetAppDataFail != null)
            {
                //GetAppDataFail(new SLTErrorStatus(SLTErrorStatusCode.UnknownError, result.Error)); //implement correct fail mechanism with correct data.
            }
        }

        private void OnSync(DownloadResult result)
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

        private void OnRegisterDevice(DownloadResult result)
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

        private void OnAddProperties(DownloadResult result)
        {

        }

        private void OnLevelContentLoad(DownloadResult result)
        {
            SLTLevel sltLevel = null;

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                sltLevel = (result.StateObject as SLTLevel) ?? new SLTLevel();
                sltLevel.Content = JsonConvert.DeserializeObject<SLTLevelContent>(result.Text);
            }

            if (sltLevel == null)
            {
                OnLevelContentLoadFail(result);
            }

            if (sltLevel.Content != null)
            {
                CacheLevelContent(sltLevel);
            }
            else
            {
                sltLevel.Content = GetLevelContentLocally(sltLevel);
            }

            if (sltLevel.Content != null)
            {
                LoadLevelContentSuccess(sltLevel);
                return;
            }
            else
            {
                OnLevelContentLoadFail(result);
            }
        }

        private void OnLevelContentLoadFail(DownloadResult result)
        {
            throw new NotImplementedException();
        }

        //private void AppDataLoadSuccessCallback(Dictionary<string, object> resource)
        //{
        //    Dictionary<string, object> data = resource.Data;

        //    if (data == null)
        //    {
        //        _onConnectFail(new SLTStatusAppDataLoadFail());
        //        resource.Dispose();
        //        return;
        //    }

        //    bool isSuccess = false;
        //    Dictionary<string, object> response = new Dictionary<string, object>();

        //    if (data.ContainsKey(SLTConstants.Response))
        //    {
        //        IEnumerable<object> res = (IEnumerable<object>)data[SLTConstants.Response];
        //        response = res.FirstOrDefault() as Dictionary<string, object>;
        //        isSuccess = (bool)response[SLTConstants.Success]; //.ToString().ToLower() == "true";
        //    }
        //    else
        //    {
        //        //TODO @GSAR: remove later when API is versioned!
        //        if (data.ContainsKey(SLTConstants.ResponseData))
        //        {
        //            response = data[SLTConstants.ResponseData] as Dictionary<string, object>;
        //        }

        //        isSuccess = (data.ContainsKey(SLTConstants.Status) && data[SLTConstants.Status].ToString() == SLTConstants.ResultSuccess);
        //    }

        //    _isLoading = false;

        //    if (isSuccess)
        //    {
        //        if (_isDevMode && !_isSynced)
        //        {
        //            Sync();
        //        }

        //        if (response.ContainsKey(SLTConstants.LevelType))
        //        {
        //            _levelType = (SLTLevelType)Enum.Parse(typeof(SLTLevelType), response[SLTConstants.LevelType].ToString(), true);
        //        }

        //        Dictionary<string, SLTFeature> saltrFeatures = new Dictionary<string, SLTFeature>();
        //        try
        //        {
        //            saltrFeatures = SLTDeserializer.DecodeFeatures(response);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log(e.Message);
        //            _onConnectFail(new SLTStatusFeaturesParseError());
        //            return;
        //        }

        //        try
        //        {
        //            _experiments = SLTDeserializer.DecodeExperiments(response);
        //        }
        //        catch
        //        {
        //            _onConnectFail(new SLTStatusExperimentsParseError());
        //            return;
        //        }

        //        // if developer didn't announce use without levels, and levelType in returned JSON is not "noLevels",
        //        // then - parse levels
        //        if (!_useNoLevels && _levelType != SLTLevelType.NoLevels)
        //        {
        //            List<SLTLevelPack> newLevelPacks = null;
        //            try
        //            {
        //                newLevelPacks = SLTDeserializer.DecodeLevels(response);
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.Log(e.Message);
        //                _onConnectFail(new SLTStatusLevelsParseError());
        //                return;
        //            }

        //            // if new levels are received and parsed, then only dispose old ones and assign new ones.
        //            if (newLevelPacks != null)
        //            {
        //                DisposeLevelPacks();
        //                _levelPacks = newLevelPacks;
        //            }
        //        }

        //        _isConected = true;
        //        _repository.CacheObject(SLTConstants.AppDataUrlCache, "0", response);

        //        _activeFeatures = saltrFeatures;
        //        _onConnectSuccess();

        //        Debug.Log("[SALTR] AppData load success. LevelPacks loaded: " + _levelPacks.Count);
        //        //TODO @GSAR: later we need to report the feature set differences by an event or a callback to client;
        //    }
        //    else
        //    {
        //        if (response.ContainsKey(SLTConstants.Error))
        //        {
        //            _onConnectFail(new SLTStatus(int.Parse(response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Code)), response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Message)));
        //        }
        //        else
        //        {
        //            int errorCode;
        //            int.TryParse(response.GetValue<string>(SLTConstants.ErrorCode), out errorCode);

        //            _onConnectFail(new SLTStatus(errorCode, response.GetValue<string>(SLTConstants.ErrorMessage)));
        //        }

        //    }
        //    resource.Dispose();
        //}

        //private void AppDataLoadFailCallback(Dictionary<string, object> resource)
        //{
        //    resource.Dispose();
        //    _isLoading = false;
        //    _onConnectFail(new SLTStatusAppDataLoadFail());
        //}

        #endregion Handlers

    }

}