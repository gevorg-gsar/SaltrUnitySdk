using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusLevelContentLoadFail : SLTStatus
    {
		public SLTStatusLevelContentLoadFail():base(SLTStatus.Code.ClientLevelContentLoadFail, "Level content load has failed.")
		{
		}
    }
}
