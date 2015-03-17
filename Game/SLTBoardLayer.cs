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
    public class SLTBoardLayer
    {
        #region Properties

        public List<SLTChunk> Chunks { get; set; }

        //[JsonProperty("fixedAssets")]
        public List<SLTAsset> FixedAssets { get; set; }

        /// <summary>
        /// Gets the token, a unique identifier for the layer within a board.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets the index, layers are ordered by this index within a board.
        /// </summary>
        public int? Index { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        
        
        #endregion Properties

        /// <summary>
        /// Regenerates contents of all the chunks within the layer.
        /// </summary>
        public virtual void Regenerate(SLTCell[,] boardCells)
        {
            Chunks.ForEach(chunk => chunk.GenerateContent(Token, Index.Value, boardCells));
        }
    }
}
