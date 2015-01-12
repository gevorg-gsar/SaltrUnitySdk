using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	internal class SLTStatusLevelsParseError : SLTStatus
    {
		public SLTStatusLevelsParseError(): base(SLTStatusCode.ClientLevelsParseError, "[SALTR] Failed to decode Levels.")
		{
		}
    }
}
