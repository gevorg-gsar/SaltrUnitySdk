using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	internal class SLTStatusLevelsParserMissing : SLTStatus
	{
		public SLTStatusLevelsParserMissing(): base(SLTStatusCode.ClientLevelsParseError, "[SALTR] Failed to find parser for current level type.")
		{
		}
	}
}
