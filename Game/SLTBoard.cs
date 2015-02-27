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
        /// Gets the layers of the board.
        /// </summary>
        public List<SLTBoardLayer> Layers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the properties associated with the board.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLTBoard(List<SLTBoardLayer> layers, Dictionary<string, object> properties)
        {
            Layers = layers;
            Properties = properties;
        }

        #endregion

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
