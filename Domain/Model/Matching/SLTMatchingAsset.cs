using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.Game.Matching;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Model.Matching
{
    public class SLTMatchingAsset : SLTAsset
    {
        public SLTMatchingAssetState State { get; set; }
    }
}