using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
	/// <summary>
	/// Represents any kind of a board layer.
	/// </summary>
    public class SLTBoardLayer
    {
        #region Properties

        /// <summary>
        /// Gets the token, a unique identifier for the layer within a board.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets the index, layers are ordered by this index within a board.
        /// </summary>
        public int Index { get; set; }

        public string LayerId { get; set; }
        
        #endregion Properties

		/// <summary>
		/// Regenerates layer contents. The behavior depends on the type of the layer. See in derived classes.
		/// </summary>
        public virtual void Regenerate()
        {
            //override
        }
    }
}
