using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	public class SLTStatusFeaturesParseError : SLTStatus
	{
		public SLTStatusFeaturesParseError()
            : base(SLTStatusCode.ClientFeaturesParseError, ExceptionConstants.SaltrFailedDecodeFeatures)
		{
		}
	}
}
