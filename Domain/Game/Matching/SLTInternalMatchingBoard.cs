using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Game.Matching
{
    public class SLTInternalMatchingBoard : SLTInternalBoard
    {
        public List<SLTMatchingBoardLayer> Layers { get; set; }
        
        public int? Rows { get; set; }

        public int? Cols { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        public List<int> CellSize { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SLTMatchingBoardOrientation? Orientation { get; set; }

        public List<CellProperty> CellProperties { get; set; }

        public List<List<int>> BlockedCells { get; set; }

    }

    public enum SLTMatchingBoardOrientation
    {
        TOP_LEFT,
        BOTTOM_LEFT
    }
}