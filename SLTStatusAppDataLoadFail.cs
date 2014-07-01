using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
   public class SLTStatusAppDataLoadFail : SLTStatus
    {
		public SLTStatusAppDataLoadFail(): base(SLTStatus.CLIENT_APP_DATA_LOAD_FAIL, "[SALTR] Failed to load appData.")
		{
		}
    }
}
