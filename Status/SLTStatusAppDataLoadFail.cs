using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
   	internal class SLTStatusAppDataLoadFail : SLTStatus
    {
		public SLTStatusAppDataLoadFail(): base(SLTStatusCode.ClientAppDataLoadFail, "[SALTR] Failed to load appData.")
		{
		}
    }
}
