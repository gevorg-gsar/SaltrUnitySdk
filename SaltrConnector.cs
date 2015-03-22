using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Saltr.UnitySdk.Repository;
using Saltr.UnitySdk.Resource;
using Saltr.UnitySdk.Game;
using Saltr.UnitySdk.Status;
using Saltr.UnitySdk.Utils;
using Plexonic.Core.Network;
using Newtonsoft.Json;

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

        private SLTAppData _appData;
        private ISLTRepository _repository;
        private Dictionary<string, SLTFeature> _defaultFeatures;

        #endregion Fields

        #region Properties

        public bool IsDevMode { get; set; }

        public string SocialId { get; set; }

        #endregion Properties

        #region Events

        public event Action<SLTAppData> AppDataGotten;
        public event Action<SLTStatus> GetAppDataFail;

        public event Action<SLTLevel> LevelContentLoadSuccess;
        public event Action<SLTStatus> LevelConnectLoadFail;

        #endregion Events

        #region Ctor

        public SaltrConnector(string clientKey, string deviceId, bool useCache = true)
        {
            _clientKey = clientKey;
            _deviceId = deviceId;
            _isLoading = false;
            _isAppDataGotten = false;

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

        public void DefineDefaultFeature(string featureToken, Dictionary<string, object> properties, bool isRequired)
        {
            if (!string.IsNullOrEmpty(featureToken))
            {
                _defaultFeatures[featureToken] = new SLTFeature() { Token = featureToken, Properties = properties, IsRequired = isRequired };
            }
        }

        public void ImportLevels(string path)
        {
            path = string.IsNullOrEmpty(path) ? SLTConstants.LocalLevelPacksPath : path;
            _appData = _repository.GetObjectFromApplication<SLTAppData>(path);
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

        public void LoadLevelContentFromSaltr(SLTLevel level)
        {
            string levelContentUrl = string.Format(LevelContentUrlFormat, level.Url, DateTime.Now.ToShortTimeString());

            DownloadManager.Instance.AddDownload(levelContentUrl, OnLevelContentLoad);
        }

        private void Sync()
        {
            var urlVars = PrepareSyncRequestParameters();
            var url = FillRequestPrameters(SLTConstants.SaltrDevApiUrl, urlVars);

            DownloadManager.Instance.AddDownload(url, OnSync);
        }
                
        ///// <summary>
        ///// Associates some properties with this client, that are used to assign it to a certain user group in Saltr.
        ///// </summary>
        ///// <param name="basicProperties">Basic properties. Standard set of client properties</param>
        ///// <param name="customProperties">(Optional)Custom properties.</param>
        //public void AddProperties(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        //{
        //    if (basicProperties == null && customProperties == null)
        //    {
        //        return;
        //    }

        //    Dictionary<string, string> urlVars = new Dictionary<string, string>();
        //    urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionAddProperties; //TODO @GSAR: remove later
        //    urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionAddProperties;

        //    SLTRequestArguments args = new SLTRequestArguments()
        //    {
        //        ApiVersion = ApiVersion,
        //        ClientKey = _clientKey,
        //        Client = Client
        //    };

        //    if (_deviceId != null)
        //    {
        //        args.DeviceId = _deviceId;
        //    }
        //    else
        //    {
        //        throw new Exception(ExceptionConstants.DeviceIdIsRequired);
        //    }

        //    if (_socialId != null)
        //    {
        //        args.SocialId = _socialId;
        //    }

        //    if (basicProperties != null)
        //    {
        //        args.BasicProperties = basicProperties;
        //    }

        //    if (customProperties != null)
        //    {
        //        args.CustomProperties = customProperties;
        //    }

        //    Action<SLTResource> propertyAddSuccess = delegate(SLTResource res)
        //    {
        //        Debug.Log(SLTConstants.Success);
        //        Dictionary<string, object> data = res.Data;
        //        res.Dispose();
        //    };

        //    Action<SLTResource> propertyAddFail = delegate(SLTResource res)
        //    {
        //        Debug.Log(SLTConstants.Error);
        //        res.Dispose();
        //    };

        //    urlVars[SLTConstants.UrlParamArguments] = Json.Serialize(args.RawData);

        //    SLTResourceTicket ticket = GetTicket(SLTConstants.SALTR_API_URL, urlVars, _requestIdleTimeout);
        //    SLTResource resource = new SLTResource(SLTConstants.ResourceIdProperty, ticket, propertyAddSuccess, propertyAddFail);
        //    resource.Load();
        //}

        ///// <summary>
        ///// See <see cref="saltr.SLTUnity.AddProperties"/>.
        ///// </summary>
        //public void AddProperties(SLTBasicProperties basicProperties)
        //{
        //    AddProperties(basicProperties, null);
        //}
        
        #endregion Public Methods

        #region Internal Methods

        private Dictionary<string, string> PrepareAppDataRequestParameters(SLTBasicProperties basicProperties, Dictionary<string, object> customProperties)
        {
            Dictionary<string, string> urlVars = new Dictionary<string, string>();

            urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionGetAppData; //TODO @GSAR: remove later
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
            urlVars[SLTConstants.UrlParamCommand] = SLTConstants.ActionDevSync; //TODO @GSAR: remove later
            urlVars[SLTConstants.UrlParamAction] = SLTConstants.ActionDevSync;

            SLTRequestArguments args = new SLTRequestArguments();
            args.ApiVersion = ApiVersion;
            args.Client = Client;
            args.ClientKey = _clientKey;

            args.IsDevMode = IsDevMode;

            if (SocialId != null)
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

        #endregion Internal Methods

        #region Handlers

        private void OnAppDataGotten(DownloadResult result)
        {
            SLTAppData sltAppData = null;

            if (result == null || string.IsNullOrEmpty(result.Text))
            {
                OnGetAppDataFail(result);
            }

            Dictionary<string, List<SLTAppData>> responseData = JsonConvert.DeserializeObject<Dictionary<string, List<SLTAppData>>>(result.Text);

            if (responseData != null && responseData.ContainsKey(SLTConstants.Response))
            {
                sltAppData = responseData[SLTConstants.Response].FirstOrDefault<SLTAppData>();

                if (sltAppData != null && sltAppData.Success.HasValue && sltAppData.Success.Value)
                {
                    _isAppDataGotten = true;
                    _repository.CacheObject(SLTConstants.AppDataCacheFileName, sltAppData);

                    Debug.Log("[SALTR] AppData load success.");

                    if (IsDevMode && !_isSynced)
                    {
                        Sync();
                    }

                    if (AppDataGotten != null)
                    {
                        AppDataGotten(sltAppData);
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
                    if (responseData.ContainsKey(SLTConstants.Error))
                    {
                        //OnConnectFail(new SLTStatus(int.Parse(response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Code)), response.GetValue<Dictionary<string, object>>(SLTConstants.Error).GetValue<string>(SLTConstants.Message)));
                    }
                    else
                    {
                        //int errorCode;
                        //int.TryParse(response.GetValue<string>(SLTConstants.ErrorCode), out errorCode);

                        //OnConnectFail(new SLTStatus(errorCode, response.GetValue<string>(SLTConstants.ErrorMessage)));
                    }

                }
            }
        }

        private void OnGetAppDataFail(DownloadResult result)
        {
            if (GetAppDataFail != null)
            {
                GetAppDataFail(new SLTStatus(SLTStatusCode.UnknownError, result.Error)); //implement correct fail mechanism with correct data.
            }
        }

        private void OnSync(DownloadResult result)
        {

        }

        private void OnLevelContentLoad(DownloadResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                SLTLevel sltLevel = (result.StateObject as SLTLevel) ?? new SLTLevel();
                sltLevel.Content = JsonConvert.DeserializeObject<SLTLevelContent>(result.Text);

                if (LevelContentLoadSuccess != null)
                {
                    LevelContentLoadSuccess(sltLevel);
                    return;
                }
            }

            OnLevelContentLoadFail(result);

            //object contentData = result.Text;
            //if (contentData != null)
            //{
            //    CacheLevelContent(level, contentData);
            //}
            //else
            //{
            //    contentData = LoadLevelContentInternally(level);
            //}

            //if (contentData != null)
            //{
            //    LevelContentLoadSuccessHandler(level, contentData);
            //}
            //else
            //{
            //    LevelContentLoadFailHandler();
            //}
        }

        private void OnLevelContentLoadFail(DownloadResult result)
        {
            throw new NotImplementedException();
        }

        //private void LevelContentLoadSuccessHandler(SLTLevel level, object content)
        //{
        //    level.UpdateContent(content as Dictionary<string, object>);
        //    _onLevelContentLoadSuccess();
        //}

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

        //private void SyncSuccessHandler(Dictionary<string, object> resource)
        //{
        //    object data = resource.Data;

        //    if (data == null)
        //    {
        //        Debug.Log("[Saltr] Dev feature Sync's data is null.");
        //        return;
        //    }

        //    var dataDict = data as Dictionary<string, object>;
        //    if (dataDict != null)
        //    {
        //        IEnumerable<object> response = (IEnumerable<object>)dataDict.GetValue(SLTConstants.Response);
        //        if (response == null)
        //        {
        //            Debug.Log("[Saltr] Dev feature Sync's response is null.");
        //            return;
        //        }

        //        if (response.Count() <= 0)
        //        {
        //            Debug.Log("[Saltr] Dev feature Sync response's length is <= 0.");
        //            return;
        //        }

        //        Dictionary<string, object> responseObject = response.ElementAt(0) as Dictionary<string, object>;

        //        if ((bool)responseObject.GetValue(SLTConstants.Success) == false)
        //        {

        //            Dictionary<string, object> errorDict = responseObject.GetValue(SLTConstants.Error) as Dictionary<string, object>;
        //            if (errorDict != null)
        //            {
        //                int errorCode;
        //                int.TryParse(errorDict.GetValue(SLTConstants.Code).ToString(), out errorCode);
        //                //TODO: change below casting to use Enum.Parse() method.
        //                if ((SLTStatusCode)errorCode == SLTStatusCode.RegistrationRequired && _isAutoRegisteredDevice)
        //                {
        //                    RegisterDevice();
        //                }

        //                Debug.Log("[Saltr] Sync error: " + errorDict.GetValue<string>(SLTConstants.Message));
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("[Saltr] Dev feature Sync is complete.");
        //            _isSynced = true;
        //        }
        //    }
        //}

        //private void SyncFailHandler(Dictionary<string, object> resource)
        //{
        //    Debug.Log("[Saltr] Dev feature Sync has failed.");
        //}

        //private void AddDeviceSuccessHandler(Dictionary<string, object> resource)
        //{
        //    Debug.Log("[Saltr] Dev adding new device is complete.");
        //    Dictionary<string, object> data = resource.Data as Dictionary<string, object>;
        //    bool isSuccess = false;
        //    Dictionary<string, object> response;
        //    if (data.ContainsKey(SLTConstants.Response))
        //    {
        //        response = ((IEnumerable<object>)(data[SLTConstants.Response])).ElementAt(0) as Dictionary<string, object>;
        //        isSuccess = (bool)response.GetValue(SLTConstants.Success);
        //        if (isSuccess)
        //        {
        //            //_wrapper.SetStatus(SLTConstants.Success);
        //            Sync();
        //        }
        //        else
        //        {
        //            Dictionary<string, object> errorDict = response.GetValue(SLTConstants.Error) as Dictionary<string, object>;
        //            if (errorDict != null)
        //            {
        //                //_wrapper.SetStatus(errorDict.GetValue<string>(SLTConstants.Message));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        AddDeviceFailHandler(resource);
        //    }
        //}

        //private void AddDeviceFailHandler(Dictionary<string, object> resource)
        //{
        //    Debug.Log("[Saltr] Dev adding new device has failed.");
        //    //_wrapper.SetStatus(SLTConstants.Failed);
        //}

        //private void AppDataLoadFailCallback(Dictionary<string, object> resource)
        //{
        //    resource.Dispose();
        //    _isLoading = false;
        //    _onConnectFail(new SLTStatusAppDataLoadFail());
        //}

        //private void LevelContentLoadFailHandler()
        //{
        //    _onLevelContentLoadFail(new SLTStatusLevelContentLoadFail());
        //}

        #endregion Handlers


    }

}