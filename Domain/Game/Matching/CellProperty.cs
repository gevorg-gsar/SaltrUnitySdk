using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Game.Matching
{
    public class CellProperty
    {
        public List<int> Coords { get; set; }

        public Dictionary<string, object> Value { get; set; }
    }
}
