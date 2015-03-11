using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Game.Canvas2D;
using Saltr.UnitySdk.Utils;
using Saltr.UnitySdk.Status;

namespace Saltr.UnitySdk.Game
{
    public class SLTLevelContent
    {
        #region Properties

        public Dictionary<string, SLTBoard> Boards { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties
    }
    
}