using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
{
	/// <summary>
	/// Internal configuration of SDK.
	/// </summary>
    public class SLTConfig
    {
		
        public static readonly string ACTION_GET_APP_DATA = "getAppData";
        public static readonly string ACTION_ADD_PROPERTIES = "addProperties";
		public static readonly string ACTION_DEV_SYNC_DATA = "sync";
		public static readonly string ACTION_DEV_REGISTER_DEVICE = "registerDevice";
        

        public static readonly string SALTR_API_URL = "https://api.saltr.com/call";
        public static readonly string SALTR_DEVAPI_URL = "https://devapi.saltr.com/call";


        //used to
        public static readonly string APP_DATA_URL_CACHE = "app_data_cache.json";
		/// <summary>
		/// Default path to the local level files.
		/// </summary>
        public static readonly string LOCAL_LEVELPACK_PACKAGE_URL = "saltr/level_packs";
        public static readonly string LOCAL_LEVEL_CONTENT_PACKAGE_URL_TEMPLATE = "saltr/pack_{0}/level_{1}";
        public static readonly string LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE = "pack_{0}_level_{1}.json";

        public static readonly string RESULT_SUCCEED = "SUCCEED";
        public static readonly string RESULT_ERROR = "ERROR";

		public static readonly string DEVICE_TYPE_IPAD = "ipad";
		public static readonly string DEVICE_TYPE_IPHONE = "iphone";
		public static readonly string DEVICE_TYPE_IPOD = "ipod";
		public static readonly string DEVICE_TYPE_ANDROID = "android";
		public static readonly string DEVICE_PLATFORM_ANDROID = "android";
		public static readonly string DEVICE_PLATFORM_IOS = "ios";
    }
}
