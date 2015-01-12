using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk
{
	/// <summary>
	/// Internal configuration of SDK.
	/// </summary>
    public class SLTConfig
    {
		//TODO @gyln: Group these actions in an enum or a struct?
        internal static readonly string ActionGetAppData = "getAppData";
		internal static readonly string ActionAddProperties = "addProperties";
		internal static readonly string ActionDevSyncData = "sync";
		internal static readonly string ActionDevRegisterDevice = "registerDevice";
        

		internal static readonly string SALTR_API_URL = "https://api.saltr.com/call";
		internal static readonly string SALTR_DEVAPI_URL = "https://devapi.saltr.com/call";


        //used to
		internal static readonly string AppDataUrlCache = "app_data_cache.json";
		/// <summary>
		/// Default path to the local level files.
		/// </summary>
        public static readonly string LocalLevelPackageUrl = "saltr/level_packs";
		internal static readonly string LocalLevelContentPackageUrlTemplate = "saltr/pack_{0}/level_{1}";
		internal static readonly string LocalLevelContentCacheUrlTemplate = "pack_{0}_level_{1}.json";

		internal static readonly string ResultSuccess = "SUCCEED";
		internal static readonly string ResultError = "ERROR";
    }
}
