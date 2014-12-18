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
		
        internal static readonly string ACTION_GET_APP_DATA = "getAppData";
		internal static readonly string ACTION_ADD_PROPERTIES = "addProperties";
		internal static readonly string ACTION_DEV_SYNC_DATA = "sync";
		internal static readonly string ACTION_DEV_REGISTER_DEVICE = "registerDevice";
        

		internal static readonly string SALTR_API_URL = "https://api.saltr.com/call";
		internal static readonly string SALTR_DEVAPI_URL = "https://devapi.saltr.com/call";


        //used to
		internal static readonly string APP_DATA_URL_CACHE = "app_data_cache.json";
		/// <summary>
		/// Default path to the local level files.
		/// </summary>
        public static readonly string LOCAL_LEVELPACK_PACKAGE_URL = "saltr/level_packs";
		internal static readonly string LOCAL_LEVEL_CONTENT_PACKAGE_URL_TEMPLATE = "saltr/pack_{0}/level_{1}";
		internal static readonly string LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE = "pack_{0}_level_{1}.json";

		internal static readonly string RESULT_SUCCEED = "SUCCEED";
		internal static readonly string RESULT_ERROR = "ERROR";
    }
}
