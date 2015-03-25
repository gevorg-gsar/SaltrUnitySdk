using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Game.Matching;


namespace Saltr.UnitySdk.Game
{
    public abstract class SLTBoard
    {
        #region Properties
        
        public List<int> Position { get; set; }
        
        public Dictionary<string, object> Properties { get; set; }


        #endregion Properties
        
    }
}
