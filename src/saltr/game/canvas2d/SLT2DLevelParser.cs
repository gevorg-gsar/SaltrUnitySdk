using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using saltr.utils;

namespace saltr.game.canvas2d
{
    internal class SLT2DLevelParser : SLTLevelParser
    {

        private static SLT2DLevelParser _instance;

        public static SLT2DLevelParser GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SLT2DLevelParser();
            }
            return _instance;
        }


        public override System.Collections.Generic.Dictionary<string, object> ParseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();
            foreach (var boardId in boardNodes.Keys)
            {
                Dictionary<string, object> boardNode = boardNodes[boardId].ToDictionaryOrNull();
                boards[boardId] = ParseLevelBoard(boardNode, assetMap);

            }
            return boards;
        }


        private SLT2DBoard ParseLevelBoard(Dictionary<string, object> boardNode, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boardProperties = new Dictionary<string, object>();
            if (boardNode.ContainsKey("properties"))
            {
                boardProperties = boardNode["properties"].ToDictionaryOrNull();
            }

            List<SLTBoardLayer> layers = new List<SLTBoardLayer>();
            IEnumerable<object> layerNodes = (IEnumerable<object>)boardNode["layers"];

            for (int i = 0; i < layerNodes.Count(); i++)
            {
                Dictionary<string, object> layerNode = layerNodes.ElementAt(i).ToDictionaryOrNull();
                SLT2DBoardLayer layer = ParseLayer(layerNode, i, assetMap);
                layers.Add(layer);
            }
            float width = boardNode.ContainsKey("width") ? boardNode["width"].ToFloatOrZero() : 0;
            float height = boardNode.ContainsKey("height") ? boardNode["height"].ToFloatOrZero() : 0;

            return new SLT2DBoard(width, height, layers, boardProperties);

        }

        private SLT2DBoardLayer ParseLayer(Dictionary<string, object> layerNode, int layerIndex, Dictionary<string, object> assetMap)
        {
			//temporarily checking for 2 names until "layerId" is removed!
			string token = (layerNode.ContainsKey("token")) ? layerNode.GetValue<string>("token") : layerNode.GetValue<string>("layerId");
			SLT2DBoardLayer layer = new SLT2DBoardLayer(token, layerIndex);
            ParseAssetInstances(layer, (IEnumerable<object>)layerNode["assets"], assetMap);
            return layer;

        }


        private void ParseAssetInstances(SLT2DBoardLayer layer, IEnumerable<object> assetNodes, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < assetNodes.Count(); i++)
            {
                Dictionary<string, object> assetInstanceNode = assetNodes.ElementAt(i).ToDictionaryOrNull();
                float x = assetInstanceNode["x"].ToFloatOrZero();
                float y = assetInstanceNode["y"].ToFloatOrZero();

                float rotation = assetInstanceNode["rotation"].ToFloatOrZero();


                SLTAsset asset = assetMap[assetInstanceNode["assetId"].ToString()] as SLTAsset;


                IEnumerable<object> stateIds = (IEnumerable<object>)assetInstanceNode["states"];

                layer.AddAssetInstance(new SLT2DAssetInstance(asset.Token, asset.GetInstanceStates(stateIds), asset.Properties, x, y, rotation));
            }
        }




        protected override SLTAssetState ParseAssetState(Dictionary<string, object> stateNode)
        {
            string token = stateNode.ContainsKey("token") ? stateNode["token"].ToString() : null;
            Dictionary<string, object> properties = stateNode.ContainsKey("properties") ? stateNode["properties"].ToDictionaryOrNull() : null;

            float pivotX = stateNode.ContainsKey("pivotX") ? stateNode["pivotX"].ToFloatOrZero() : 0;
            float pivotY = stateNode.ContainsKey("pivotY") ? stateNode["pivotY"].ToFloatOrZero() : 0;

            return new SLT2DAssetState(token, properties, pivotX, pivotY);
        }



    }
}
