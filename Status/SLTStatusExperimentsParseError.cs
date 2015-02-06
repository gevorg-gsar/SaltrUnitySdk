using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	public class SLTStatusExperimentsParseError : SLTStatus
	{
		public SLTStatusExperimentsParseError()
            : base(SLTStatusCode.ClientExperimentsParseError, ExceptionConstants.SaltrFailedDecodeExperiments)
		{
		}
	}
}
