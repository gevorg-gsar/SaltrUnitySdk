using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTMatchingLevel : SLTLevel
    {
        public SLTMatchingLevel(string id, int index, int localIndex, int packIndex, string contentUrl, Dictionary<string, object> properties, string version) :
            base(id, index, localIndex, packIndex, contentUrl, properties, version)
        {
        }

        SLTMatchingBoard getBoard(string id)
        {
            return _boards[id] as SLTMatchingBoard;
        }

        protected override SLTLevelParser getParser()
        {
            return SLTMatchingLevelParser.getInstance();
        }

    }
}
