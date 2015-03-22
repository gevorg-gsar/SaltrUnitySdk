using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk
{
    /// <summary>
    /// Internal configuration of SDK.
    /// </summary>
    public class SLTConstants
    {
        public const string SaltrGameObjectName = "Saltr";

        //TODO @gyln: Group these actions in an enum or a struct?
        public const string ActionDevSync = "sync";
        public const string ActionGetAppData = "getAppData";
        public const string ActionAddProperties = "addProperties";
        public const string ActionDevRegisterDevice = "registerDevice";

        public const string SaltrApiUrl = "https://api.saltr.com/call";
        public const string SaltrDevApiUrl = "https://devapi.saltr.com/call";

        //used to
        public const string AppDataCacheFileName = "app_data_cache.json";
        /// <summary>
        /// Default path to the local level files.
        /// </summary>
        public const string LocalLevelPacksPath = "Saltr/level_packs.json";
        public const string LocalLevelContentPathFormat = "Saltr/pack_{0}/level_{1}.json";
        public const string LocalLevelContentCachePathFormat = "pack_{0}_level_{1}.json";

        public const string ResultSuccess = "SUCCEED";
        public const string ResultError = "ERROR";

        public const string OS = "os";

        public const string Id = "id";
        public const string Age = "age";
        public const string Url = "url";
        public const string Gender = "gender";
        public const string Locale = "locale";
        public const string Region = "region";
        public const string City = "city";
        public const string Email = "email";
        public const string Country = "country";
        public const string Location = "location";
        public const string AppVersion = "appVersion";
        public const string DeviceName = "deviceName";
        public const string DeviceType = "deviceType";
        public const string SystemName = "systemName";
        public const string SystemVersion = "systemVersion";
        public const string BrowserName = "browserName";
        public const string BrowserVersion = "browserVersion";

        public const string HttpMethodGet = "get";
        public const string HttpMethodPost = "post";

        public const string DeviceId = "deviceId";
        public const string SocialId = "socialId";
        public const string AssetId = "assetId";
        public const string ApiVersion = "apiVersion";
        public const string Unknown = "Unknown";
        public const string Assets = "assets";
        public const string X = "x";
        public const string Y = "y";
        public const string PivotX = "pivotX";
        public const string PivotY = "pivotY";
        public const string Rotation = "rotation";
        public const string Code = "code";
        public const string Type = "type";
        public const string Cols = "cols";
        public const string Rows = "rows";
        public const string Name = "name";
        public const string Cells = "cells";
        public const string Value = "value";
        public const string Error = "error";
        public const string Token = "token";
        public const string Index = "index";
        public const string Levels = "levels";
        public const string Layers = "layers";
        public const string Boards = "boards";
        public const string Status = "status";
        public const string States = "states";
        public const string Failed = "failed";
        public const string Source = "source";
        public const string Coords = "coords";
        public const string Version = "version";
        public const string Success = "success";
        public const string Message = "message";
        public const string Response = "response";
        public const string Features = "features";
        public const string LevelType = "levelType";
        public const string ErrorCode = "errorCode";
        public const string Partition = "partition";
        public const string LevelPacks = "levelPacks";
        public const string LocalIndex = "localIndex";
        public const string Properties = "properties";
        public const string FeatureType = "featureType";
        public const string Experiments = "experiments";
        public const string VariationId = "variationId";
        public const string ErrorMessage = "errorMessage";
        public const string ResponseData = "responseData";
        public const string BlockedCells = "blockedCells";
        public const string CellProperties = "cellProperties";
        public const string CustomEventList = "customEventList";
        public const string BasicProperties = "basicProperties";
        public const string VariationVersion = "variationVersion";
        public const string CustomProperties = "customProperties";
        public const string DistributionType = "distributionType";
        public const string DistributionValue = "distributionValue";
        public const string DeveloperFeatures = "developerFeatures";
        
        public const string UrlParamCommand = "cmd";
        public const string UrlParamAction = "action";
        public const string UrlParamArguments = "args";
        public const string UrlParamDevMode = "devMode";
        public const string UrlParamClientKey = "clientKey";

        public const string ResourceIdSaltr = "saltr";
        public const string ResourceIdProperty = "property";
        public const string ResourceIdAddDevice = "addDevice";
        public const string ResourceIdSyncFeatures = "syncFeatures";
        public const string ResourceIdSaltrAppConfig = "saltAppConfig";

        public const string StatusIdle = "idle";
        public const string StatusLoading = "Loading...";
        public const string StatusInvalidEmail = "Invalid email!";

        public const string GuiBox = "box";
        public const string GuiClose = "Close";
        public const string GuiSubmit = "Submit";
        public const string GuiEmailField = "email_field";

        public const string GuiRegisterDeviceWithSaltr = "Register Device with SALTR";
    }
}
