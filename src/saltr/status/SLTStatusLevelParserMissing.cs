using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusLevelsParserMissing : SLTStatus
	{
		public SLTStatusLevelsParserMissing(): base(SLTStatus.Code.ClientLevelsParseError, "[SALTR] Failed to find parser for current level type.")
		{
		}
	}
}
