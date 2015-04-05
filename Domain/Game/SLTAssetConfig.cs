using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Game
{
    public abstract class SLTAssetConfig
    {
        #region Properties

        public string AssetId { get; set; }

        public string StateId { get; set; }

        public List<string> States { get; set; }
                
        #endregion Properties
    }
}
