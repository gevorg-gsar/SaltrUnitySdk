using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.matching
{
    internal class SLTCompositeAsset : SLTAsset
    {
        public IEnumerable<object> _cellInfos { get; set; }

        public SLTCompositeAsset(string token, IEnumerable<object> cellInfos, object properties, Dictionary<string,object> states)
            : base(token, properties,states)
        {
            _cellInfos = cellInfos;
        }
    
    
    SLTAssetInstance GetInstance(IEnumerable<object> stateIds)
        {
            return new SLTCompositeInstance (Token, GetInstanceStates(stateIds),
                Properties, _cellInfos);
        }

    }
}
