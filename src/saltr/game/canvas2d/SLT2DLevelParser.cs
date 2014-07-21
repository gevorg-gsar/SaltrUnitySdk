using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using saltr.utils;

namespace saltr.game.canvas2d
{
    public class SLT2DLevelParser : SLTLevelParser
    {

        private static SLT2DLevelParser INSTANCE;

        public static SLT2DLevelParser getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new SLT2DLevelParser();
            }
            return INSTANCE;
        }


        public override System.Collections.Generic.Dictionary<string, object> parseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();
            foreach (var boardId in boardNodes.Keys)
            {
                Dictionary<string, object> boardNode = boardNodes[boardId].toDictionaryOrNull();
                boards[boardId] = parseLevelBoard(boardNode, assetMap);

            }
            return boards;
        }


        private SLT2DBoard parseLevelBoard(Dictionary<string, object> boardNode, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boardProperties = new Dictionary<string, object>();
            if (boardNode.ContainsKey("properties") && boardNode["properties"].toDictionaryOrNull().ContainsKey("board"))
            {
                boardProperties = boardNode["properties"].toDictionaryOrNull()["board"].toDictionaryOrNull();
            }

            List<SLTBoardLayer> layers = new List<SLTBoardLayer>();
            IEnumerable<object> layerNodes = (IEnumerable<object>)boardNode["layers"];

            for (int i = 0; i < layerNodes.Count(); i++)
            {
                Dictionary<string, object> layerNode = layerNodes.ElementAt(i).toDictionaryOrNull();
                SLT2DBoardLayer layer = parseLayer(layerNode, i, assetMap);
                layers.Add(layer);
            }
            float width = boardNode.ContainsKey("width") ? boardNode["width"].toFloatOrZero() : 0;
            float height = boardNode.ContainsKey("height") ? boardNode["height"].toFloatOrZero() : 0;

            return new SLT2DBoard(width, height, layers, boardProperties);

        }

        private SLT2DBoardLayer parseLayer(Dictionary<string, object> layerNode, int LayerIndex, Dictionary<string, object> assetMap)
        {
            string layerId = layerNode["layerId"].ToString();
            SLT2DBoardLayer layer = new SLT2DBoardLayer(layerId, LayerIndex);
            parseAssetInstances(layer, (IEnumerable<object>)layerNode["assets"], assetMap);
            return layer;

        }


        private void parseAssetInstances(SLT2DBoardLayer layer, IEnumerable<object> assetNodes, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < assetNodes.Count(); i++)
            {
                Dictionary<string, object> assetInstanceNode = assetNodes.ElementAt(i).toDictionaryOrNull();
                float x = assetInstanceNode["x"].toFloatOrZero();
                float y = assetInstanceNode["y"].toFloatOrZero();

                float rotation = assetInstanceNode["rotation"].toFloatOrZero();


                SLTAsset asset = assetMap[assetInstanceNode["assetId"].ToString()] as SLTAsset;


                IEnumerable<object> stateIds = (IEnumerable<object>)assetInstanceNode["states"];

                layer.addAssetInctance(new SLT2DAssetInstance(asset.token, asset.getInstanceStates(stateIds), asset.properties, x, y, rotation));
            }
        }




        protected override SLTAssetState parseAssetState(Dictionary<string, object> stateNode)
        {
            string token = stateNode.ContainsKey("token") ? stateNode["token"].ToString() : null;
            Dictionary<string, object> properties = stateNode.ContainsKey("properties") ? stateNode["properties"].toDictionaryOrNull() : null;

            float pivotX = stateNode.ContainsKey("pivotX") ? stateNode["pivotX"].toFloatOrZero() : 0;
            float pivotY = stateNode.ContainsKey("pivotY") ? stateNode["pivotY"].toFloatOrZero() : 0;

            return new SLT2DAssetState(token, properties, pivotX, pivotY);
        }



    }
}
