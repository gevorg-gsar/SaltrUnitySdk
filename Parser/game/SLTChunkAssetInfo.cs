using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTChunkAssetRule
    {
        private string _assetId;
        private string _stateId;
        private string _distributionType;
        private int _distributionValue;

        public string assetId
        {
            get { return _assetId; }
        }

        public string stateId
        {
            get { return _stateId; }
        }

        public string distributionType
        {
            get { return _distributionType; }
        }
        
        public int distributionValue
        {
            get { return _distributionValue; }
        }

        public SLTChunkAssetRule(string assetId, string distributionType, int distributionValue, string stateId)
        {
            _assetId = assetId;
            _distributionType = distributionType;
            _distributionValue = distributionValue;
            _stateId = stateId;
        }
    }
}
