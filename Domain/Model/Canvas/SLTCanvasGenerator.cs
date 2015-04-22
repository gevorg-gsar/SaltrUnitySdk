using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.InternalModel;
using Saltr.UnitySdk.Domain.InternalModel.Canvas;
using Saltr.UnitySdk.Domain.InternalModel.Matching;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Model.Canvas
{
    public static class SLTCanvasGenerator
    {
        public static SLTCanvasBoard RegenerateBoard(this SLTInternalCanvasBoard internalCanvasBoard, Dictionary<string, SLTCanvasAssetType> assetTypes)
        {
            SLTCanvasBoard boardModel = new SLTCanvasBoard();

            boardModel.Width = internalCanvasBoard.Width;
            boardModel.Height = internalCanvasBoard.Height;
            boardModel.AssetsByLayerIndex = new Dictionary<int, List<SLTCanvasAsset>>();
            boardModel.AssetsByLayerToken = new Dictionary<string, List<SLTCanvasAsset>>();

            int index = 0;
            internalCanvasBoard.Layers.ForEach(layer =>
            {
                List<SLTCanvasAsset> canvasAssets = layer.RegenerateLayer(assetTypes, index++);

                boardModel.AssetsByLayerToken.Add(layer.Token, canvasAssets);
                boardModel.AssetsByLayerIndex.Add(layer.Index, canvasAssets);
            });

            boardModel.Properties = internalCanvasBoard.Properties;

            return boardModel;
        }

        public static List<SLTCanvasAsset> RegenerateLayer(this SLTCanvasBoardLayer boardLayer, Dictionary<string, SLTCanvasAssetType> assetTypes, int index)
        {
            boardLayer.Index = index;
            List<SLTCanvasAsset> layerAssets = new List<SLTCanvasAsset>();

            boardLayer.Assets.ForEach(canvasAssetConfig =>
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