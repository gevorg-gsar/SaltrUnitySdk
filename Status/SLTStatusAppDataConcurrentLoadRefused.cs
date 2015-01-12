using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	internal class SLTStatusAppDataConcurrentLoadRefused : SLTStatus
	{
		public SLTStatusAppDataConcurrentLoadRefused(): base(SLTStatusCode.ClientAppDataConcurrentLoadRefused, "[SALTR] appData load refused. Previous load is not complete")
		{
		}
	}
}
