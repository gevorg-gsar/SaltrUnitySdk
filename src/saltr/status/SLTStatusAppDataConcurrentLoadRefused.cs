using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	public class SLTStatusAppDataConcurrentLoadRefused : SLTStatus
	{
		public SLTStatusAppDataConcurrentLoadRefused(): base(SLTStatus.CLIENT_APP_DATA_CONCURRENT_LOAD_REFUSED, "[SALTR] appData load refused. Previous load is not complete")
		{
		}
	}
}
