using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.status
{
	internal class SLTStatusExperimentsParseError : SLTStatus
	{
		public SLTStatusExperimentsParseError() : base(SLTStatus.Code.ClientExperimentsParseError, "[SALTR] Failed to decode Experiments.")
		{
		}
	}
}
