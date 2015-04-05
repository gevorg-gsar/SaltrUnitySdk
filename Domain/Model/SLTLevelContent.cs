using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.Model.Canvas;
using Saltr.UnitySdk.Domain.Model.Matching;

namespace Saltr.UnitySdk.Domain.Model
{
    public class SLTLevelContent
    {
        public Dictionary<string, SLTCanvasBoard> CanvasBoards { get; set; }

        public Dictionary<string, SLTMatchingBoard> MatchingBoards { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public SLTLevelContent()
        {
            CanvasBoards = new Dictionary<string, SLTCanvasBoard>();
            MatchingBoards = new Dictionary<string, SLTMatchingBoard>();
        }
    }
}
