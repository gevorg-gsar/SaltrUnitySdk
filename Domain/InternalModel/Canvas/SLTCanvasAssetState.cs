using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.InternalModel.Canvas
{
    public class SLTCanvasAssetState : SLTAssetState
    {
        public float? PivotX { get; set; }

        public float? PivotY { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }
    }
}