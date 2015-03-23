using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain
{
    public class SLTBaseEntity
    {
        public bool? Success { get; set; }

        public SLTErrorStatus Error { get; set; }
    }
}
