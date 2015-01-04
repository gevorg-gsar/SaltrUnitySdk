using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.matching
{
    internal class SLTCompositeInstance : SLTAssetInstance
    {
        private IEnumerable<object> _cells;

        public IEnumerable<object> Cells
        {
            get { return _cells; }
        }

        public SLTCompositeInstance(string token, List<SLTAssetState> stateIds, object properties, IEnumerable<object> cells)
            : base(token, stateIds, properties)
        {
            _cells = cells;
        }
    }
}
