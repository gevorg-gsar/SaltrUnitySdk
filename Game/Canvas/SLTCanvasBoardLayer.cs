using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Canvas
{
    public class SLTCanvasBoardLayer : SLTBoardLayer
    {
        public List<SLTCanvasAssetConfig> Assets { get; set; }

        public virtual List<SLTCanvasAsset> Regenerate(Dictionary<string, SLTCanvasAssetType> assetTypes, int index)
        {
            Index = index;
            List<SLTCanvasAsset> layerAssets = new List<SLTCanvasAsset>();

            Assets.ForEach(canvasAssetConfig =>
            {
                 SLTCanvasAssetType assetType = assetTypes[canvasAssetConfig.AssetId];
                 SLTCanvasAsset asset = new SLTCanvasAsset()
                 {
                     Token = assetType.Token, 
                     State = assetType.States[canvasAssetConfig.StateId], 
                     Properties = assetType.Properties, 
                     X = canvasAssetConfig.X, 
                     Y = canvasAssetConfig.Y, 
                     Rotation = canvasAssetConfig.Rotation 
                 };

                 layerAssets.Add(asset);                
            });

            return layerAssets;
        }
    }
}