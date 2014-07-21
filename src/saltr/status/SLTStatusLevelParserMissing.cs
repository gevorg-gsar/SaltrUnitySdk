using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
	public class SLTStatusLevelsParserMissing : SLTStatus
	{
		public SLTStatusLevelsParserMissing(): base(SLTStatus.CLIENT_LEVELS_PARSE_ERROR, "[SALTR] Failed to find parser for current level type.")
		{
		}
	}
}
