using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCompositeAsset : SLTAsset
    {
        public IEnumerable<object> _cellInfos { get; set; }

        public SLTCompositeAsset(string token, IEnumerable<object> cellInfos, object properties)
            : base(token, properties)
        {
            _cellInfos = cellInfos;
        }
    }
}
