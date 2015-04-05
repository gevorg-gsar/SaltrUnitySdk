using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.Game.Matching;


namespace Saltr.UnitySdk.Domain.Game
{
    public abstract class SLTInternalBoard
    {
        #region Properties
        
        public List<int> Position { get; set; }
        
        public Dictionary<string, object> Properties { get; set; }


        #endregion Properties
        
    }
}
