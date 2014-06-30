using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{
    public static class Deserializer2d
    {

        public static Dictionary<string, object> decod2dBoards(Dictionary<string, object> boardsNode, SLTLevelSettings levelSettings)
        {
            List<SLT2dBoard> boards = new List<SLT2dBoard>();
            foreach (var item in boardsNode)
            {

                Dictionary<string, object> boardDict = item.Value.toDictionaryOrNull();

                if (boardDict == null)
                    continue;
                SLT2dBoard board = new SLT2dBoard();

                board.id = item.Key;
                board.layers = decodeLayers(boardDict, levelSettings);
                board.width = boardDict.getValue("width").ToString();
                board.height = boardDict.getValue("height").ToString();

                IEnumerable<object> position = (IEnumerable<object>)boardDict.getValue("position");


                if (position != null)
                {
                    board.position = new UnityEngine.Vector2(float.Parse(position.ElementAt(0).ToString()), float.Parse(position.ElementAt(1).ToString()));
                }
                board.properties = boardDict.getValue("properties");
                boards.Add(board);
            }

            Dictionary<string, object> dictionartToReturn = new Dictionary<string, object>();

            foreach (var board in boards)
            {
                dictionartToReturn[board.id] = board;

            }
            return dictionartToReturn;
        }


        public static List<SLT2dBoardLayer> decodeLayers(Dictionary<string, object> boardNode, SLTLevelSettings levelSettings)
        {
            IEnumerable<object> layerDictionaryList = null;
            List<SLT2dBoardLayer> layers = new List<SLT2dBoardLayer>();

            if (boardNode.ContainsKey("layers"))
                layerDictionaryList = (IEnumerable<object>)boardNode["layers"];
            int index = 0;

            foreach (var layerDict in layerDictionaryList)
            {

                SLT2dBoardLayer layer = new SLT2dBoardLayer();
                layer._layerId = layerDict.toDictionaryOrNull()["layerId"].ToString();
                layer.assets = decodeAssets(layerDict.toDictionaryOrNull(), levelSettings);
                layer._layerIndex = index;
                layers.Add(layer);
                index++;
            }

            return layers;
        }

        public static List<SLT2dAssetInstance> decodeAssets(Dictionary<string, object> layerNode, SLTLevelSettings levelSettings)
        {
            List<SLT2dAssetInstance> assets = new List<SLT2dAssetInstance>();
            IEnumerable<object> assetsDictList = null;
            assetsDictList = (IEnumerable<object>)layerNode["assets"];

            foreach (var item in assetsDictList)
            {
                Dictionary<string, object> assetsDict = item.toDictionaryOrNull();
                string assetId = assetsDict.getValue("assetId").ToString();
                string stateId = null;

                if (assetsDict.ContainsKey("stateId"))
                {
                    object obj = assetsDict["stateId"];
                    stateId = obj.ToString();
                }

                Vector2 position = Vector2.zero;

                float x;
                float y;

                x = float.Parse(assetsDict.getValue("x").ToString());
                y = float.Parse(assetsDict.getValue("y").ToString());

                position = new Vector2(x, y);

                float rotationAngle = float.Parse(assetsDict.getValue("rotation").ToString());

                SLT2dAsset asset = levelSettings.assetMap.getValue(assetId) as SLT2dAsset;
                string type = "";
                object properties = null;

                string state = null;

                //  if (stateId != null)   For Match3
                //   state = levelSettings.stateMap.getValue(stateId) as String;


                if (stateId != null)
                {
                    Dictionary<string, object> statesDict = asset.states;
                    state = statesDict[stateId].toDictionaryOrNull()["token"].ToString();
                }


                if (asset != null)
                {
                    type = asset.token;
                    properties = asset.properties;

                }
                SLT2dAssetInstance asset2d = new SLT2dAssetInstance(type, state, properties)
                {
                    position = position,
                    rotationAngle = rotationAngle,
                    size = asset.size,
                    pivot = asset.pivot
                   
                };
                assets.Add(asset2d);
            }
            return assets;
        }

    }
}
