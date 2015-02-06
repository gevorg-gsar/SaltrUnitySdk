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
        //TODO @gyln: Group these actions in an enum or a struct?
        public const string ActionGetAppData = "getAppData";
        public const string ActionAddProperties = "addProperties";
        public const string ActionDevSyncData = "sync";
        public const string ActionDevRegisterDevice = "registerDevice";

        public const string SALTR_API_URL = "https://api.saltr.com/call";
        public const string SALTR_DEVAPI_URL = "https://devapi.saltr.com/call";

        //used to
        public const string AppDataUrlCache = "app_data_cache.json";
        /// <summary>
        /// Default path to the local level files.
        /// </summary>
        public const string LocalLevelPackageUrl = "saltr/level_packs";
        public const string LocalLevelContentPackageUrlTemplate = "saltr/pack_{0}/level_{1}";
        public const string LocalLevelContentCacheUrlTemplate = "pack_{0}_level_{1}.json";

        public const string ResultSuccess = "SUCCEED";
        public const string ResultError = "ERROR";

        public const string OS = "os";

        public const string Id = "id";
        public const string Age = "age";
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
        public const string ApiVersion = "apiVersion";
        public const string Unknown = "Unknown";
        public const string Code = "code";
        public const string Type = "type";
        public const string Value = "value";
        public const string Error = "error";
        public const string Token = "token";
        public const string Status = "status";
        public const string Failed = "failed";
        public const string Source = "source";
        public const string Success = "success";
        public const string Message = "message";
        public const string Response = "response";
        public const string LevelType = "levelType";
        public const string ErrorCode = "errorCode";
        public const string Partition = "partition";
        public const string Experiments = "experiments";
        public const string ErrorMessage = "errorMessage";
        public const string ResponseData = "responseData";
        public const string CustomEventList = "customEventList";
        public const string BasicProperties = "basicProperties";
        public const string CustomProperties = "customProperties";
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
