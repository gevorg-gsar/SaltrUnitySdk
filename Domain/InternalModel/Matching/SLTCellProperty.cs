using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.InternalModel.Matching
{
    public class SLTCellProperty
    {
        public List<int> Coords { get; set; }

        public Dictionary<string, object> Value { get; set; }
    }
}
