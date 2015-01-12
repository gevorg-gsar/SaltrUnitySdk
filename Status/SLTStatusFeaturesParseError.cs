using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	internal class SLTStatusFeaturesParseError : SLTStatus
	{
		public SLTStatusFeaturesParseError(): base(SLTStatusCode.ClientFeaturesParseError, "[SALTR] Failed to decode Features.")
		{
		}
	}
}
