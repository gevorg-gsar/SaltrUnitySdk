using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace saltr.game.canvas2d
{
	/// <summary>
	/// Represents a layer of a 2D board.
	/// </summary>
	public class SLT2DBoardLayer : SLTBoardLayer {

		private List<SLT2DAssetInstance> _assetInctances;

		internal SLT2DBoardLayer(string layerId, int layerIndex) : base(layerId,layerIndex)
		{
			_assetInctances = new List<SLT2DAssetInstance>();
		}

		/// <summary>
		/// Gets all the asset instances present on the board.
		/// </summary>
		public List<SLT2DAssetInstance> assetInstances
		{
			get{return _assetInctances;}
		}

		/// <summary>
		/// Adds the asset inctance to the board.
		/// </summary>
		public void addAssetInctance(SLT2DAssetInstance instance)
		{
			_assetInctances.Add(instance);
		}

		/// <summary>
		/// Currently has no effect, since 2D boards do not have any randomized content.
		/// </summary>
		public override void regenerate ()
		{
			// nothing to do here yet
		}
	}
}