using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
	class SLTStatusFeaturesParseError : SLTStatus
	{
		public SLTStatusFeaturesParseError(): base(SLTStatus.CLIENT_FEATURES_PARSE_ERROR, "[SALTR] Failed to decode Features.")
		{
		}
	}
}
