using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	public class SLTStatusLevelContentLoadFail : SLTStatus
    {
		public SLTStatusLevelContentLoadFail()
            : base(SLTStatusCode.ClientLevelContentLoadFail, ExceptionConstants.SaltrFailedLoadContent)
		{
		}
    }
}
