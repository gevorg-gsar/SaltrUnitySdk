using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTChunkAssetRule
    {
        #region Properties

        public string AssetId
        {
            get;
            private set;
        }

        public IEnumerable<object> StateIds
        {
            get;
            private set;
        }

        public float DistributionValue
        {
            get;
            private set;
        }

        public ChunkAssetRuleDistributionType DistributionType
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTChunkAssetRule(string assetId, ChunkAssetRuleDistributionType distributionType, float distributionValue, IEnumerable<object> stateIds)
        {
            AssetId = assetId;
            DistributionType = distributionType;
            DistributionValue = distributionValue;
            StateIds = stateIds;
        }

        #endregion Ctor
        
    }

    public enum ChunkAssetRuleDistributionType
    {
        Unknown = 0,
        Count,
        Ratio,
        Random
    }
}
