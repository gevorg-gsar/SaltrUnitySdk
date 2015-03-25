using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Saltr.UnitySdk.Game.Matching;

namespace Saltr.UnitySdk.Game
{
	/// <summary>
	/// Represents any kind of a board layer.
	/// </summary>
    public abstract class SLTBoardLayer
    {
        #region Properties
        
        public string Token { get; set; }

        public int? Index { get; set; }
        
        #endregion Properties

    }
}
