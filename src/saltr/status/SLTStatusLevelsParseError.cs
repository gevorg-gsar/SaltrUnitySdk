using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusLevelsParseError : SLTStatus
    {
		public SLTStatusLevelsParseError(): base(SLTStatus.Code.CLIENT_LEVELS_PARSE_ERROR, "[SALTR] Failed to decode Levels.")
		{
		}
    }
}
