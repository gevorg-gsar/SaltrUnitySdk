using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	public class SLTStatusAppDataConcurrentLoadRefused : SLTStatus
	{
		public SLTStatusAppDataConcurrentLoadRefused()
            : base(SLTStatusCode.ClientAppDataConcurrentLoadRefused, ExceptionConstants.SaltrAppDataLoadRefused)
		{
		}
	}
}
