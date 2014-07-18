using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLT2DLevel : SLTLevel
    {
        public  SLT2DLevel(string id, int index, int localIndex, int packIndex, string contentUrl, Dictionary<string,object> properties, string version)
            : base(id, index, localIndex, packIndex, contentUrl, properties, version) { }
        public SLT2DBoard getBoard(string id)
        {
            return _boards[id] as SLT2DBoard;
        }

        protected override SLTLevelParser getParser()
        {
            return SLT2DLevelParser.getInstance();
        }
    }
}
