using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using saltr.utils;

namespace saltr.game.matching
{

    internal class SLTMatchingLevelParser : SLTLevelParser
    {
        private static SLTMatchingLevelParser INSTANCE = null;

        private SLTMatchingLevelParser() { }

        public static SLTMatchingLevelParser getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new SLTMatchingLevelParser();
            }
            return INSTANCE;
        }


        private static void initializeCells(SLTCells cells, object boardNode)
        {
            Dictionary<string, object> boardNodeDict = boardNode.toDictionaryOrNull();

            IEnumerable<object> blockedCells = boardNodeDict.ContainsKey("blockedCells") ? (IEnumerable<object>)boardNodeDict["blockedCells"] : new List<object>();
			IEnumerable<object> cellProperties = boardNodeDict.ContainsKey("cellProperties")? (IEnumerable<object>)boardNodeDict["cellProperties"] : new List<object>();
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
                    cell2.properties = property.toDictionaryOrNull()["value"].toDictionaryOrNull();
            }


            //blocking cells
            for (int b = 0; b < blockedCells.Count(); b++)
            {
                IEnumerable<object> blokedCell = (IEnumerable<object>)blockedCells.ElementAt(b);
				var cell3 = cells.retrieve(blokedCell.ElementAt(0).toIntegerOrZero(), blokedCell.ElementAt(1).toIntegerOrZero());
                if (cell3 != null)
                {
                    cell3.isBlocked = true;
                }
            }
        }


        private static void parseLayerChunks(SLTMatchingBoardLayer layer, IEnumerable<object> chunkNodes, SLTCells cells, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < chunkNodes.Count(); i++)
            {
                Dictionary<string, object> chunkNode = chunkNodes.ElementAt(i).toDictionaryOrNull();
                IEnumerable<object> cellNodes = new List<object>();
                if (chunkNode != null && chunkNode.ContainsKey("cells"))
                    cellNodes = (IEnumerable<object>)chunkNode["cells"];

                List<SLTCell> chunkCells = new List<SLTCell>();
                foreach (var cellNode in cellNodes)
                {
                    int row = 0;
                    int col = 0;

					row = ((IEnumerable<object>)cellNode).ElementAt(0).toIntegerOrZero();
					col = ((IEnumerable<object>)cellNode).ElementAt(1).toIntegerOrZero();

					chunkCells.Add(cells.retrieve(row, col) as SLTCell);
                }

                IEnumerable<object> assetNodes = (IEnumerable<object>)chunkNode["assets"];
                List<SLTChunkAssetRule> chunkAssetRules = new List<SLTChunkAssetRule>();
                foreach (var assetNode in assetNodes)
                {
                    string assetId = "";
                    string distribytionType = "";
                    float distributionVale = 0;
                    IEnumerable<object> states = new List<object>();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("assetId"))
                        assetId = assetNode.toDictionaryOrNull()["assetId"].ToString();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionType"))
                        distribytionType = assetNode.toDictionaryOrNull()["distributionType"].ToString();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionValue"))
                        distributionVale = assetNode.toDictionaryOrNull()["distributionValue"].toFloatOrZero();

                    if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("states"))
                        states = (IEnumerable<object>)assetNode.toDictionaryOrNull()["states"];

                    chunkAssetRules.Add(new SLTChunkAssetRule(assetId, distribytionType, distributionVale, states));
                }
                layer.addChunk(new SLTChunk(layer.token, layer.index, chunkCells, chunkAssetRules, assetMap));
            }
        }


        private SLTMatchingBoard parseLevelBoard(Dictionary<string, object> boardNode, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boardProperties = new Dictionary<string, object>();
            if (boardNode.ContainsKey("properties"))
            {
                boardProperties = boardNode["properties"].toDictionaryOrNull();
            }

            SLTCells cells = new SLTCells(boardNode["cols"].toIntegerOrZero(), boardNode["rows"].toIntegerOrZero());
            initializeCells(cells, boardNode);
            List<SLTBoardLayer> layers = new List<SLTBoardLayer>();
            IEnumerable<object> layerNodes = (IEnumerable<object>)boardNode["layers"];
            for (int i = 0; i < layerNodes.Count(); i++)
            {
                Dictionary<string, object> layerNode = layerNodes.ElementAt(i).toDictionaryOrNull();
                SLTMatchingBoardLayer layer = parseLayer(layerNode, i, cells, assetMap);
                layers.Add(layer);
            }

            return new SLTMatchingBoard(cells, layers, boardProperties);
        }


        private SLTMatchingBoardLayer parseLayer(Dictionary<string, object> layerNode, int layerIndex, SLTCells cells, Dictionary<string, object> assetMap)
        {
			//temporarily checking for 2 names until "layerId" is removed!
			string token = (layerNode.ContainsKey("token")) ? layerNode.getValue<string>("token") : layerNode.getValue<string>("layerId");
            SLTMatchingBoardLayer layer = new SLTMatchingBoardLayer(token, layerIndex);

            parseFixedAssets(layer, (IEnumerable<object>)layerNode["fixedAssets"], cells, assetMap);
            parseLayerChunks(layer, (IEnumerable<object>)layerNode["chunks"], cells, assetMap);

            return layer;
        }


        private void parseFixedAssets(SLTMatchingBoardLayer layer, IEnumerable<object> assetNodes, SLTCells cells, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < assetNodes.Count(); i++)
            {
                Dictionary<string, object> assetInstanceNode = assetNodes.ElementAt(i).toDictionaryOrNull();
                SLTAsset asset = assetMap[assetInstanceNode["assetId"].ToString()] as SLTAsset;
                IEnumerable<object> stateIds = (IEnumerable<object>)assetInstanceNode["states"];
                IEnumerable<object> cellPositions = (IEnumerable<object>)assetInstanceNode["cells"];

                for (int j = 0; j < cellPositions.Count(); j++)
                {
                    IEnumerable<object> position = (IEnumerable<object>)cellPositions.ElementAt(j);
                    SLTCell cell = cells.retrieve(position.ElementAt(0).toIntegerOrZero(), position.ElementAt(1).toIntegerOrZero());
                    cell.setAssetInstance(layer.token, layer.index, new SLTAssetInstance(asset.token, asset.getInstanceStates(stateIds), asset.properties));
                }
            }
        }



        public override Dictionary<string, object> parseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();

            foreach (var boardId in boardNodes.Keys)
            {
                Dictionary<string, object> boardNode = boardNodes[boardId].toDictionaryOrNull();
                boards[boardId] = parseLevelBoard(boardNode, assetMap);
            }
            return boards;
        }


    }
}