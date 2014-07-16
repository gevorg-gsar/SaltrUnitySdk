using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{
    public static class Deserializer2d
    {

        //public static Dictionary<string, object> decod2dBoards(Dictionary<string, object> boardsNode, SLTLevelSettings levelSettings)
        //{
        //    List<SLT2DBoard> boards = new List<SLT2DBoard>();
        //    foreach (var item in boardsNode)
        //    {

        //        Dictionary<string, object> boardDict = item.Value.toDictionaryOrNull();

        //        if (boardDict == null)
        //            continue;
        //        SLT2DBoard board = new SLT2DBoard();

        //        board.id = item.Key;
        //        board.layers = decodeLayers(boardDict, levelSettings);
        //        board.width = boardDict.getValue("width").ToString();
        //        board.height = boardDict.getValue("height").ToString();

        //        IEnumerable<object> position = (IEnumerable<object>)boardDict.getValue("position");


        //        if (position != null)
        //        {
        //            board.position = new UnityEngine.Vector2(float.Parse(position.ElementAt(0).ToString()), float.Parse(position.ElementAt(1).ToString()));
        //        }
        //        board.properties = boardDict.getValue("properties");
        //        boards.Add(board);
        //    }

        //    Dictionary<string, object> dictionartToReturn = new Dictionary<string, object>();

        //    foreach (var board in boards)
        //    {
        //        dictionartToReturn[board.id] = board;

        //    }
        //    return dictionartToReturn;
        //}


        //public static List<SLT2DBoardLayer> decodeLayers(Dictionary<string, object> boardNode, SLTLevelSettings levelSettings)
        //{
        //    IEnumerable<object> layerDictionaryList = null;
        //    List<SLT2DBoardLayer> layers = new List<SLT2DBoardLayer>();

        //    if (boardNode.ContainsKey("layers"))
        //        layerDictionaryList = (IEnumerable<object>)boardNode["layers"];
        //    int index = 0;

        //    foreach (var layerDict in layerDictionaryList)
        //    {

        //        SLT2DBoardLayer layer = new SLT2DBoardLayer();
        //        layer._layerId = layerDict.toDictionaryOrNull()["layerId"].ToString();
        //        layer.assets = decodeAssets(layerDict.toDictionaryOrNull(), levelSettings);
        //        layer._layerIndex = index;
        //        layers.Add(layer);
        //        index++;
        //    }

        //    return layers;
        //}

        //public static List<SLT2DAssetInstance> decodeAssets(Dictionary<string, object> layerNode, SLTLevelSettings levelSettings)
        //{
        //    List<SLT2DAssetInstance> assets = new List<SLT2DAssetInstance>();
        //    IEnumerable<object> assetsDictList = null;
        //    assetsDictList = (IEnumerable<object>)layerNode["assets"];

        //    foreach (var item in assetsDictList)
        //    {
        //        Dictionary<string, object> assetsDict = item.toDictionaryOrNull();
        //        string assetId = assetsDict.getValue("assetId").ToString();
        //        string stateId = null;

        //        if (assetsDict.ContainsKey("stateId"))
        //        {
        //            object obj = assetsDict["stateId"];
        //            stateId = obj.ToString();
        //        }

        //        Vector2 position = Vector2.zero;

        //        float x;
        //        float y;

        //        x = float.Parse(assetsDict.getValue("x").ToString());
        //        y = float.Parse(assetsDict.getValue("y").ToString());

        //        position = new Vector2(x, y);

        //        float rotationAngle = float.Parse(assetsDict.getValue("rotation").ToString());

        //        SLT2DAssetState asset = levelSettings.assetMap.getValue(assetId) as SLT2DAssetState;
        //        string type = "";
        //        object properties = null;

        //        List<SLTAssetState> states =  new List<SLTAssetState>();

        //        //  if (stateId != null)   For Match3
        //        //   state = levelSettings.stateMap.getValue(stateId) as String;


        //        if (stateId != null)
        //        {
        //            Dictionary<string, object> statesDict = asset.states;

        //            if (item.toDictionaryOrNull().ContainsKey("states"))
        //            {
        //                foreach (var stateItem in (IEnumerable<object>)item.toDictionaryOrNull()["states"])
        //                {
        //                    if (statesDict.ContainsKey(stateItem.ToString()))
        //                    {
        //                        Dictionary<string, object> tempStateDict = statesDict[stateItem.ToString()].toDictionaryOrNull();
        //                        string token = "";
        //                        Dictionary<string, object> Properties = new Dictionary<string, object>();

        //                        if (tempStateDict.ContainsKey("properties"))
        //                        {
        //                            Properties = tempStateDict["properties"].toDictionaryOrNull();
        //                        }

        //                        if (tempStateDict.ContainsKey("token"))
        //                        {
        //                            token = tempStateDict["token"].ToString();
        //                        }

        //                        states.Add(new SLTAssetState(token, Properties));
        //                    }

        //                }
        //            }
        //        }


        //        if (asset != null)
        //        {
        //            type = asset.token;
        //            properties = asset.properties;

        //        }
        //        SLT2DAssetInstance asset2d = new SLT2DAssetInstance(type, states, properties,position.x,position.y,rotationAngle);
                
        //        assets.Add(asset2d);
        //    }
        //    return assets;
        //}

    }
}
