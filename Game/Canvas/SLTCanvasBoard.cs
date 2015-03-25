using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Canvas
{
    public class SLTCanvasBoard : SLTBoard
    {
        public List<SLTCanvasBoardLayer> Layers { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }
    }
}