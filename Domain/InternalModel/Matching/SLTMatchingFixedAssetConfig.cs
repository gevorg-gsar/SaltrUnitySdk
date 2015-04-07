using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.InternalModel.Matching
{
    public class SLTMatchingFixedAssetConfig : SLTAssetConfig
    {
        public List<List<int>> Cells { get; set; }
    }
}