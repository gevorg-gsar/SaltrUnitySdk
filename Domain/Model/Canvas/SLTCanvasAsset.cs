using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.InternalModel.Canvas;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Model.Canvas
{
    public class SLTCanvasAsset : SLTAsset
    {
        public SLTCanvasAssetState State { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Rotation { get; set; }
    }
}