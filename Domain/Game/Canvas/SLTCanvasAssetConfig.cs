using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Game.Canvas
{
    public class SLTCanvasAssetConfig : SLTAssetConfig
    {
        public float? X { get; set; }

        public float? Y { get; set; }

        public float? Rotation { get; set; }
    }
}