using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    /// <summary>
    /// Represents a layer of a matching board.
    /// </summary>
    public class SLTMatchingBoardLayer : SLTBoardLayer
    {
        #region Fields

        private List<SLTChunk> _chunks = new List<SLTChunk>();

        #endregion Fields

        #region Business Methods

        // <summary>
        // Adds a chunk to the layer.
        // </summary>
        public void AddChunk(SLTChunk chunk)
        {
            _chunks.Add(chunk);
        }

        /// <summary>
        /// Regenerates contents of all the chunks within the layer.
        /// </summary>
        public override void Regenerate()
        {
            _chunks.ForEach(chunk => chunk.GenerateContent());
        }

        #endregion Business Methods

    }
}
