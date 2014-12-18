using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.matching
{
	/// <summary>
	/// Represents a layer of a matching board.
	/// </summary>
	public class SLTMatchingBoardLayer : SLTBoardLayer
	{
		private List<SLTChunk> _chunks;

		internal SLTMatchingBoardLayer(string LayerId, int LayerIndex):base(LayerId,LayerIndex)
		{
			_chunks = new List<SLTChunk>();
		}

		/// <summary>
		/// Regenerates contents of all the chunks within the layer.
		/// </summary>
		public override void regenerate ()
		{
			foreach(SLTChunk chunk in _chunks)
			{
				chunk.generateContent();
			}
		}

		// <summary>
		// Adds a chunk to the layer.
		// </summary>
		internal void addChunk(SLTChunk chunk)
		{
			_chunks.Add(chunk);
		}

	}
}
