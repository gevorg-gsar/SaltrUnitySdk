using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
   	public class SLTStatusExperimentsParseError : SLTStatus
	{
		public SLTStatusExperimentsParseError() : base(SLTStatus.CLIENT_EXPERIMENTS_PARSE_ERROR, "[SALTR] Failed to decode Experiments.")
		{
		}
	}
}
