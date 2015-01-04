using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusFeaturesParseError : SLTStatus
	{
		public SLTStatusFeaturesParseError(): base(SLTStatus.Code.ClientFeaturesParseError, "[SALTR] Failed to decode Features.")
		{
		}
	}
}
