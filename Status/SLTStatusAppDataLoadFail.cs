using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
   	public class SLTStatusAppDataLoadFail : SLTStatus
    {
		public SLTStatusAppDataLoadFail()
            : base(SLTStatusCode.ClientAppDataLoadFail, "[SALTR] Failed to load appData.")
		{
		}
    }
}
