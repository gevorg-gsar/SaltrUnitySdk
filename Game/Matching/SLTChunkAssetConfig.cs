using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTChunkAssetConfig : SLTAssetConfig
    {
        #region Properties

        public int? DistributionValue { get; set; }

        public ChunkAssetDistributionType? DistributionType { get; set; }

        #endregion Properties
    }

    public enum ChunkAssetDistributionType
    {
        Count,
        Ratio,
        Random
    }
}
