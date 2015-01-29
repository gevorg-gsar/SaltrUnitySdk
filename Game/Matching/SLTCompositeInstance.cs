using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTCompositeInstance : SLTAssetInstance
    {
        #region Properties

        public IEnumerable<object> Cells
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTCompositeInstance(string token, List<SLTAssetState> stateIds, object properties, IEnumerable<object> cells)
            : base(token, stateIds, properties)
        {
            Cells = cells;
        }

        #endregion Ctor
        
    }
}
