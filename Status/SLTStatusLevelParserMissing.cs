using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Status
{
	public class SLTStatusLevelsParserMissing : SLTStatus
	{
		public SLTStatusLevelsParserMissing()
            : base(SLTStatusCode.ClientLevelsParseError, ExceptionConstants.SaltrFailedFindParserForLevelType)
		{
		}
	}
}
