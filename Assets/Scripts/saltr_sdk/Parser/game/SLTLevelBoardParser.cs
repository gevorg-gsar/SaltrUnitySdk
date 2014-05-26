using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevelBoardParser
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

            SLTCellMatrix cells = new SLTCellMatrix(width, heihgt);
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

                SLTLevelBoardLayer layer = new SLTLevelBoardLayer(layerId, i, fixedAssets, chunks, composites);
                parseLayer(layer, cells, levelSettings);
                layers[layer.layerId] = layer;
            }
            return new SLTLevelBoard(cells, layers, boardProperties);
        }


        private static void parseLayer(SLTLevelBoardLayer layer, SLTCellMatrix cells, SLTLevelSettings levelSettings)
        {
            parseFixedAssets(layer, cells, levelSettings);
            parseChunks(layer, cells, levelSettings);
            parseComposites(layer, cells, levelSettings);
        }


        private static void parseComposites(SLTLevelBoardLayer layer, SLTCellMatrix cellMatrix, SLTLevelSettings levelSettings)
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



        private static void parseFixedAssets(SLTLevelBoardLayer layer, SLTCellMatrix cells, SLTLevelSettings levelSettings)
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
                    cell.setAssetInstance(layer.layerId, layer.layerIndex, new SLTAssetInstance(asset.token, state, asset.properties));
                }
            }
        }

        private static void initializeCells(SLTCellMatrix cells, object boardNode)
        {
            Dictionary<string, object> boardNodeDict = boardNode.toDictionaryOrNull();

            IEnumerable<object> blockedCells = boardNodeDict.ContainsKey("blockedCells") ? (IEnumerable<object>)boardNodeDict["blockedCells"] : new List<object>();
            IEnumerable<object> cellProperties = boardNodeDict.ContainsKey("properties") && boardNodeDict["properties"].toDictionaryOrNull().ContainsKey("cell") ? (IEnumerable<object>)boardNodeDict["properties"].toDictionaryOrNull()["cell"] : new List<object>();
            int cols = cells.width;
            int rows = cells.height;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    SLTCell cell = new SLTCell(i, j);
                    cells.insert(i, j, cell);
                }
            }

            //assigning cell properties
            for (int p = 0; p < cellProperties.Count(); p++)
            {
                object property = cellProperties.ElementAt(p);
                IEnumerable<object> coords = (IEnumerable<object>)property.toDictionaryOrNull()["coords"];
                SLTCell cell2 = cells.retrieve(coords.ElementAt(0).toIntegerOrZero(), coords.ElementAt(1).toIntegerOrZero());
                if (cell2 != null)
                    cell2.properties = property.toDictionaryOrNull()["value"];
            }


            //blocking cells
            for (int b = 0; b < blockedCells.Count(); b++)
            {
                IEnumerable<object> blokedCell = (IEnumerable<object>)blockedCells.ElementAt(b);
                var cell3 = cells.retrieve(blockedCells.ElementAt(0).toIntegerOrZero(), blockedCells.ElementAt(1).toIntegerOrZero());
                if (cell3 != null)
                {
                    cell3.isBlocked = true;
                }
            }
        }


        private static void parseChunks(SLTLevelBoardLayer layer, SLTCellMatrix cellMatrix, SLTLevelSettings levelSettings)
        {
            IEnumerable<object> chunkNodes = layer.chunkNodes;
            for (int i = 0; i < chunkNodes.Count(); i++)
            {
                object chunkNode = chunkNodes.ElementAt(i);
                IEnumerable<object> cellNodes = new List<object>();
                if (chunkNode.toDictionaryOrNull() != null && chunkNode.toDictionaryOrNull().ContainsKey("cells"))
                    cellNodes = (IEnumerable<object>)chunkNode.toDictionaryOrNull()["cells"];

                List<SLTCell> chunkCells = new List<SLTCell>();
                foreach (var cellNode in cellNodes)
                {
                    int width = 0;
                    int height = 0;

                    width = ((IEnumerable<object>)cellNode).ElementAt(0).toIntegerOrZero();
                    height = ((IEnumerable<object>)cellNode).ElementAt(1).toIntegerOrZero();

                    chunkCells.Add(cellMatrix.retrieve(width, height) as SLTCell);
                }

                IEnumerable<object> assetNodes = (IEnumerable<object>)chunkNode.toDictionaryOrNull()["assets"];
                List<SLTChunkAssetRule> chunkAssetRules = new List<SLTChunkAssetRule>();
                foreach (var assetNode in assetNodes)
                {
                    string assetId = "";
                    string distribytionType = "";
                    int distributionVale = 0;
                    string stateId = "";

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("assetId"))
                        assetId = assetNode.toDictionaryOrNull()["assetId"].ToString();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionType"))
                        distribytionType = assetNode.toDictionaryOrNull()["distributionType"].ToString();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionValue"))
                        distributionVale = assetNode.toDictionaryOrNull()["distributionValue"].toIntegerOrZero();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("stateId"))
                        stateId = assetNode.toDictionaryOrNull()["stateId"].ToString();

                    chunkAssetRules.Add(new SLTChunkAssetRule(assetId, distribytionType, distributionVale, stateId));
                }

                new SLTChunk(layer, chunkCells, chunkAssetRules, levelSettings);
            }
        }



        public static SLTLevelSettings parseLevelSettings(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            if (rootNode.ContainsKey("assets"))
                assetMap = parseBoardAssets(rootNode["assets"].toDictionaryOrNull());

            Dictionary<string, object> stateMap = new Dictionary<string, object>();
            if (rootNode.ContainsKey("assetStates"))
                stateMap = parseAssetStates(rootNode["assetStates"].toDictionaryOrNull());

            return new SLTLevelSettings(assetMap, stateMap);
        }


        private static Dictionary<string, object> parseAssetStates(Dictionary<string, object> states)
        {
            Dictionary<string, object> statesMap = new Dictionary<string, object>();
            foreach (var item in states.Keys)
            {
                statesMap[item.ToString()] = states[item.ToString()];
            }
            return statesMap;
        }


        private static Dictionary<string, object> parseBoardAssets(Dictionary<string, object> assetNodes)
        {
            Dictionary<string, object> assetMap = new Dictionary<string, object>();
            foreach (var assetId in assetNodes.Keys)
            {
                assetMap[assetId.ToString()] = parseAsset(assetNodes[assetId.ToString()].toDictionaryOrNull());
            }
            return assetMap;
        }


        private static SLTAsset parseAsset(Dictionary<string, object> assetNode)
        {
            string token = "";
            object properties = null;
            
            if(assetNode.ContainsKey("properties"))

            properties = assetNode["properties"];

            if (assetNode.ContainsKey("token"))
            {
                token = assetNode["token"].ToString();
            }
            else
            {
                if (assetNode.ContainsKey("type"))
                {
                    token = assetNode["type"].ToString();
                }
            }
            //if asset is complete asset!
            if (assetNode.ContainsKey("cells") && assetNode["cells"] != null || assetNode.ContainsKey("cellInfos") && assetNode["cellInfos"] != null)
            {
                return new SLTCompositeAsset(token, (IEnumerable<object>)assetNode["cellInfos"], properties);
            }
            return new SLTAsset(token, properties);
        }
    }
}