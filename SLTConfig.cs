using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTConfig
    {


        public static readonly string ACTION_GET_APP_DATA = "getAppData";
        public static readonly string ACTION_ADD_PROPERTIES = "addProperties";
        public static readonly string ACTION_DEV_SYNC_FEATURES = "syncFeatures";

        

        public static readonly string SALTR_API_URL = "https://api.saltr.com/call";
        //public static readonly string SALTR_API_URL = "https://saltapi.includiv.com/call";
        public static readonly string SALTR_DEVAPI_URL = "https://devapi.saltr.com/call";
        //public static readonly string SALTR_DEVAPI_URL = "https://saltadmin.includiv.com/call";


        //used to
        public static readonly string APP_DATA_URL_CACHE = "app_data_cache.json";
        public static readonly string LOCAL_LEVELPACK_PACKAGE_URL = "saltr/level_packs";
        public static readonly string LOCAL_LEVEL_CONTENT_PACKAGE_URL_TEMPLATE = "saltr/pack_{0}/level_{1}";
        public static readonly string LOCAL_LEVEL_CONTENT_CACHE_URL_TEMPLATE = "pack_{0}_level_{1}.json";

        public static readonly string PROPERTY_OPERATIONS_INCREMENT = "inc";
        public static readonly string PROPERTY_OPERATIONS_SET = "set";

        public static readonly string RESULT_SUCCEED = "SUCCEED";
        public static readonly string RESULT_ERROR = "ERROR";
    }
}
