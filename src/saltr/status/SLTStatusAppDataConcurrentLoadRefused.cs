using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusAppDataConcurrentLoadRefused : SLTStatus
	{
		public SLTStatusAppDataConcurrentLoadRefused(): base(SLTStatus.Code.ClientAppDataConcurrentLoadRefused, "[SALTR] appData load refused. Previous load is not complete")
		{
		}
	}
}
