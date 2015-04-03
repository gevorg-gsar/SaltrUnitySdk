using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Canvas
{
    public class SLTCanvasBoard : SLTBoard
    {
        #region Properties

        public List<SLTCanvasBoardLayer> Layers { get; set; }

        public float? Width { get; set; }

        public float? Height { get; set; }

        public Dictionary<string, List<SLTCanvasAsset>> AssetsByLayerToken { get; private set; }

        public Dictionary<string, List<SLTCanvasAsset>> AssetsByLayerIndex { get; private set; }

        #endregion Properties

        public void Regenerate(Dictionary<string, SLTCanvasAssetType> assetTypes)
        {
            AssetsByLayerToken = new Dictionary<string, List<SLTCanvasAsset>>();
            AssetsByLayerIndex = new Dictionary<string, List<SLTCanvasAsset>>();

            int index = 0;
            Layers.ForEach(layer =>
            {
                List<SLTCanvasAsset> canvasAssets = layer.Regenerate(assetTypes, index++);

                AssetsByLayerToken.Add(layer.Token, canvasAssets);
                AssetsByLayerIndex.Add(layer.Index.Value.ToString(), canvasAssets);
            });
        }
    }
}