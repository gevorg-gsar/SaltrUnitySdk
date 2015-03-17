using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class SLTAsset
    {
        #region Properties

        public string AssetId { get; set; }

        public string StateId { get; set; }

        public float? X { get; set; }

        public float? Y { get; set; }

        public float? Rotation { get; set; }

        public List<string> States { get; set; }

        public List<List<int>> Cells { get; set; }        
        
        #endregion Properties
    }
}
