using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.InternalModel.Canvas
{
    public class SLTCanvasAssetType : SLTAssetType
    {
        public Dictionary<string, SLTCanvasAssetState> States { get; set; }
    }
}