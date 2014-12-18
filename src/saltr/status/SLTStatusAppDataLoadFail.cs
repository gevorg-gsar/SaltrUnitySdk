using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
   	internal class SLTStatusAppDataLoadFail : SLTStatus
    {
		public SLTStatusAppDataLoadFail(): base(SLTStatus.Code.CLIENT_APP_DATA_LOAD_FAIL, "[SALTR] Failed to load appData.")
		{
		}
    }
}
