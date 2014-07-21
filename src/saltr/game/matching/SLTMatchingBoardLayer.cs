using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game.matching
{
	public class SLTMatchingBoardLayer : SLTBoardLayer
	{
		private List<SLTChunk> _chunks;

		public SLTMatchingBoardLayer(string LayerId, int LayerIndex):base(LayerId,LayerIndex)
		{
			_chunks = new List<SLTChunk>();
		}

		public override void regenerate ()
		{
			foreach(SLTChunk chunk in _chunks)
			{
				chunk.generateContent();
			}
		}

		public void addChunk(SLTChunk chunk)
		{
			_chunks.Add(chunk);
		}

	}
}
