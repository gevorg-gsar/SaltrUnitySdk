using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Model.Canvas
{
    public class SLTCanvasBoard
    {
        public float? Width { get; set; }

        public float? Height { get; set; }

        public Dictionary<string, List<SLTCanvasAsset>> AssetsByLayerToken { get; set; }

        public Dictionary<string, List<SLTCanvasAsset>> AssetsByLayerIndex { get; set; }
    }
}
