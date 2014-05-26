using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevelSettings
    {
        private Dictionary<string, object> _assetMap;

        public Dictionary<string, object> assetMap
        {
            get { return _assetMap; }
        }

        private Dictionary<string, object> _stateMap;

        public Dictionary<string, object> stateMap
        {
            get { return _stateMap; }
        }

        public SLTLevelSettings(Dictionary<string, object> assetMap, Dictionary<string, object> stateMap)
        {
            _assetMap = assetMap;
            _stateMap = stateMap;
        }
    }
}
