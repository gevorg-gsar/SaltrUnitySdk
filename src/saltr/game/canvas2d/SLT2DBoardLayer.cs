using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace saltr.game.canvas2d
{
	public class SLT2DBoardLayer : SLTBoardLayer {

		private List<SLT2DAssetInstance> _assetInctances;

		public SLT2DBoardLayer(string layerId, int layerIndex) : base(layerId,layerIndex)
		{
			_assetInctances = new List<SLT2DAssetInstance>();
		}

		public List<SLT2DAssetInstance> assetInstances
		{
			get{return _assetInctances;}
		}

		public void addAssetInctance(SLT2DAssetInstance instance)
		{
			_assetInctances.Add(instance);
		}
	}
}