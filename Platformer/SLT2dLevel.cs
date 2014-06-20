using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLT2dLevel : SLTLevel
    {
        public List<SLT2dBoard> boards { get; set; }

        public SLT2dLevel(string id, int index, string contentDataUrl, object properties, string version)
            : base(id, index, contentDataUrl, properties, version)
        {

        }
    }
}
