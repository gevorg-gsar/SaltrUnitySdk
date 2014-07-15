using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{

    public class SLTLevelParser
    {
        public static Dictionary<string, object> parseLevelBoards(Dictionary<string, object> boardNodes, SLTLevelSettings levelSettings)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();

            if (boardNodes == null)
                return boards;

            foreach (string boardID in boardNodes.Keys)
            {
                Dictionary<string, object> boardNod = (Dictionary<string, object>)boardNodes[boardID];
                boards[boardID] = parseLevelBoard(boardNod, levelSettings);
            }
            return boards;
        }


        public static SLTLevelBoard parseLevelBoard(Dictionary<string, object> boardNode, SLTLevelSettings levelSettings)
        {
            Dictionary<string, object> boardProperties = new Dictionary<string, object>();
            if (boardNode.ContainsKey("properties") && boardNode["properties"].toDictionaryOrNull() != null &&
                boardNode["properties"].toDictionaryOrNull().ContainsKey("board"))
                boardProperties = (boardNode["properties"].toDictionaryOrNull()["board"]).toDictionaryOrNull();

            int width = 0;
            int heihgt = 0;

            if (boardNode.ContainsKey("cols"))
                width = boardNode["cols"].toIntegerOrZero();

            if (boardNode.ContainsKey("rows"))
                heihgt = boardNode["rows"].toIntegerOrZero();

            SLTCells cells = new SLTCells(width, heihgt);
            initializeCells(cells, boardNode);

            Dictionary<string, object> layers = new Dictionary<string, object>();
            IEnumerable<object> layersNodes = new List<object>();
            if (boardNode.ContainsKey("layers"))
                layersNodes = (IEnumerable<object>)boardNode["layers"];

            for (int i = 0; i < layersNodes.Count(); i++)
            {
                Dictionary<string, object> layerNod = layersNodes.ElementAt(i).toDictionaryOrNull();

                string layerId = "";
                if (layerNod.ContainsKey("layerId"))
                    layerId = layerNod["layerId"].ToString();

                IEnumerable<object> fixedAssets = new List<object>();
                if (layerNod.ContainsKey("fixedAssets"))
                    fixedAssets = (IEnumerable<object>)layerNod["fixedAssets"];

                IEnumerable<object> chunks = new List<object>();
                if (layerNod.ContainsKey("chunks"))
                    chunks = (IEnumerable<object>)layerNod["chunks"];

                IEnumerable<object> composites = new List<object>();
                if (layerNod.ContainsKey("composites"))
                    composites = (IEnumerable<object>)layerNod["composites"];

                SLTBoardLayer layer = new SLTBoardLayer(layerId, i, fixedAssets, chunks, composites);
                parseLayer(layer, cells, levelSettings);
                layers[layer.layerId] = layer;
            }
            return new SLTLevelBoard(cells, layers, boardProperties);
        }


        private static void parseLayer(SLTBoardLayer layer, SLTCells cells, SLTLevelSettings levelSettings)
        {
            parseFixedAssets(layer, cells, levelSettings);
            parseLayerChunks(layer, cells, levelSettings);
            parseComposites(layer, cells, levelSettings);
        }


        private static void parseComposites(SLTBoardLayer layer, SLTCells cellMatrix, SLTLevelSettings levelSettings)
        {
            IEnumerable<object> compositeNodes = layer.compositeNodes;
            for (int i = 0; i < compositeNodes.Count(); i++)
            {
                Dictionary<string, object> compositeNode = compositeNodes.ElementAt(i).toDictionaryOrNull();
                IEnumerable<object> cells = new List<object>();
                IEnumerable<object> cellPosition = new List<object>();

                if (compositeNodes.Contains("cell"))
                {
                    cellPosition = (IEnumerable<object>)compositeNode["cell"];

                    string assetId = "";
                    string stateId = "";
                    if (compositeNode.ContainsKey("assetId"))
                        assetId = compositeNode["assetId"].ToString();

                    if (compositeNode.ContainsKey("stateId"))
                        stateId = compositeNode["stateId"].ToString();


                    new SLTComposite(layer, assetId, stateId, (cellMatrix.retrieve(cellPosition.ElementAt(0).toIntegerOrZero(), cellPosition.ElementAt(1).toIntegerOrZero())) as SLTCell, levelSettings);
                }
            }
        }



        private static void parseFixedAssets(SLTBoardLayer layer, SLTCells cells, SLTLevelSettings levelSettings)
        {
            IEnumerable<object> fixedAssetsNode = layer.fixedAssetsNodes;
            Dictionary<string, object> assetMap = levelSettings.assetMap;
            Dictionary<string, object> stateMap = levelSettings.stateMap;

            for (int i = 0; i < fixedAssetsNode.Count(); i++)
            {
                object fixedAsset = fixedAssetsNode.ElementAt(i);
                SLTAsset asset = assetMap[fixedAsset.toDictionaryOrNull()["assetId"].ToString()] as SLTAsset;
                string state = stateMap[fixedAsset.toDictionaryOrNull()["stateId"].ToString()].ToString();
                IEnumerable<object> cellPosition = (IEnumerable<object>)fixedAsset.toDictionaryOrNull()["cells"];

                for (int j = 0; j < cellPosition.Count(); j++)
                {
                    IEnumerable<object> position = (IEnumerable<object>)cellPosition.ElementAt(j);
                    SLTCell cell = cells.retrieve(position.ElementAt(0).toIntegerOrZero(), position.ElementAt(1).toIntegerOrZero());
                    //cell.setAssetInstance(layer.layerId, layer.layerIndex, new SLTAssetInstance(asset.token, state, asset.properties));
                }
            }
        }


		//complete
		public abstract Dictionary<string,object> parseLevelContent(Dictionary<string,object> boardNodes, Dictionary<string,object> assetMap);

        //complete
        public  Dictionary<string,object> parseLevelSettings(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            if (rootNode.ContainsKey("assets"))
                assetMap = parseLevelAssets(rootNode["assets"].toDictionaryOrNull());

            return assetMap;
        }


        //complete
        protected  SLTAssetState parseAssetState(Dictionary<string, object> stateNode)
        {
            string token = String.Empty;
            Dictionary<string, object> properties = new Dictionary<string, object>();

            if (stateNode.ContainsKey("token"))
            {
                token = stateNode["token"].ToString();
            }

            if (stateNode.ContainsKey("properties"))
            {
                properties = stateNode["properties"].toDictionaryOrNull();
            }

            return new SLTAssetState(token, properties);
        }


        //complete
        private  Dictionary<string, object> parseAssetStates(Dictionary<string, object> states)
        {
            Dictionary<string, object> statesMap = new Dictionary<string, object>();
            foreach (var stateId in states.Keys)
            {
                statesMap[stateId.ToString()] = parseAssetState(statesMap[stateId.ToString()].toDictionaryOrNull());
            }

            return statesMap;
        }




        //complete
        private  Dictionary<string, object> parseLevelAssets(Dictionary<string, object> assetNodes)
        {
            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            foreach (var assetId in assetNodes.Keys)
            {
                assetMap[assetId.ToString()] = parseAsset(assetNodes[assetId.ToString()].toDictionaryOrNull());
            }
            return assetMap;
        }


        //complete
        private  SLTAsset parseAsset(Dictionary<string, object> assetNode)
        {
            string token = "";
            object properties = null;
            Dictionary<string, object> States = new Dictionary<string, object>();

            if (assetNode.ContainsKey("properties"))

                properties = assetNode["properties"];

            if (assetNode.ContainsKey("states"))
            {
                States = assetNode["states"].toDictionaryOrNull();
            }

            if (assetNode.ContainsKey("token"))
            {
                token = assetNode["token"].ToString();
            }

            return new SLTAsset(token, properties, States);
        }
    }
}