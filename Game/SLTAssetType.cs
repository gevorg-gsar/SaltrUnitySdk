using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class SLTAssetType
    {
        #region Properties

        public string Token { get; set; }

        public Dictionary<string, SLTAssetState> States { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties
    }
}
