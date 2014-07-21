using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
   public class SLTStatusLevelsParseError : SLTStatus
    {
		public SLTStatusLevelsParseError(): base(SLTStatus.CLIENT_LEVELS_PARSE_ERROR, "[SALTR] Failed to decode Levels.")
		{
		}
    }
}
