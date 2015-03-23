using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game
{
    public class SLTLevelContent
    {
        #region Properties

        public Dictionary<string, SLTBoard> Boards { get; set; }

        public Dictionary<string, SLTAssetType> Assets { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties
    }
    
}