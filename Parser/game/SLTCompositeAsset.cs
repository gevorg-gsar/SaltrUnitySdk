using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCompositeAsset : SLTAsset
    {
        public IEnumerable<object> _cellInfos { get; set; }

        public SLTCompositeAsset(string token, IEnumerable<object> cellInfos, object properties, Dictionary<string,object> states)
            : base(token, properties,states)
        {
            _cellInfos = cellInfos;
        }
    }
}
