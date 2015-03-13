using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// The SLTBoard class represents a game board. 
    /// </summary>
    public class SLTBoard
    {

        #region Properties

        /// <summary>
        /// Gets the width of the board in pixels as is in Saltr level editor.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Gets the height of the board in pixels as is in Saltr level editor.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets the layers of the board.
        /// </summary>
        public List<SLTBoardLayer> Layers { get; set; }

        /// <summary>
        /// Gets the properties associated with the board.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

        #region Business Methods

        /// <summary>
        /// Regenerates contents of all layers.
        /// </summary>
        public void Regenerate()
        {
            Layers.ForEach(layer => layer.Regenerate());
        }

        #endregion Business Methods

    }
}
