using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTCompositeAsset : SLTAsset
    {
        #region Properties

        public IEnumerable<object> CellInfos { get; set; }

        #endregion Properties

        #region Business Methods

        public SLTAssetInstance GetInstance(IEnumerable<object> stateIds)
        {
            return new SLTCompositeInstance(Token, GetInstanceStates(stateIds),
                Properties, CellInfos);
        }

        #endregion Business Methods

    }
}
