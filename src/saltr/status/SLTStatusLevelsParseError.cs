using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusLevelsParseError : SLTStatus
    {
		public SLTStatusLevelsParseError(): base(SLTStatus.Code.ClientLevelsParseError, "[SALTR] Failed to decode Levels.")
		{
		}
    }
}
