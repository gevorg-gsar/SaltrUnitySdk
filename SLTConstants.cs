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
    }
}
