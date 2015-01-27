using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTChunkAssetRule
    {
        private string _assetId;
		private IEnumerable<object> _stateIds;
        private ChunkAssetRuleDistributionType _distributionType;
        private float _distributionValue;

        public string AssetId
        {
            get { return _assetId; }
        }

		public IEnumerable<object> StateId
        {
            get { return _stateIds; }
        }

        public ChunkAssetRuleDistributionType DistributionType
        {
            get { return _distributionType; }
        }
        
        public float DistributionValue
        {
            get { return _distributionValue; }
        }

        public SLTChunkAssetRule(string assetId, ChunkAssetRuleDistributionType distributionType, float distributionValue, IEnumerable<object> stateIds)
        {
            _assetId = assetId;
            _distributionType = distributionType;
            _distributionValue = distributionValue;
            _stateIds = stateIds;
        }
    }

    public enum ChunkAssetRuleDistributionType
    {
        Unknown = 0,
        Count,
        Ratio,
        Random
    }
}
