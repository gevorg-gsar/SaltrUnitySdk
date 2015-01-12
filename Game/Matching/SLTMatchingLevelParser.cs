using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
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


        private static void InitializeCells(SLTCells cells, object boardNode)
        {
            Dictionary<string, object> boardNodeDict = boardNode.ToDictionaryOrNull();

            IEnumerable<object> blockedCells = boardNodeDict.ContainsKey("blockedCells") ? (IEnumerable<object>)boardNodeDict["blockedCells"] : new List<object>();
			IEnumerable<object> cellProperties = boardNodeDict.ContainsKey("cellProperties")? (IEnumerable<object>)boardNodeDict["cellProperties"] : new List<object>();
            int cols = cells.Width;
            int rows = cells.Height;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    SLTCell cell = new SLTCell(i, j);
                    cells.Insert(i, j, cell);
                }
            }

            //assigning cell properties
            for (int p = 0; p < cellProperties.Count(); p++)
            {
                object property = cellProperties.ElementAt(p);
                IEnumerable<object> coords = (IEnumerable<object>)property.ToDictionaryOrNull()["coords"];
                SLTCell cell2 = cells.Retrieve(coords.ElementAt(0).ToIntegerOrZero(), coords.ElementAt(1).ToIntegerOrZero());
                if (cell2 != null)
                    cell2.Properties = property.ToDictionaryOrNull()["value"].ToDictionaryOrNull();
            }


            //blocking cells
            for (int b = 0; b < blockedCells.Count(); b++)
            {
                IEnumerable<object> blokedCell = (IEnumerable<object>)blockedCells.ElementAt(b);
				var cell3 = cells.Retrieve(blokedCell.ElementAt(0).ToIntegerOrZero(), blokedCell.ElementAt(1).ToIntegerOrZero());
                if (cell3 != null)
                {
                    cell3.IsBlocked = true;
                }
            }
        }


        private static void ParseLayerChunks(SLTMatchingBoardLayer layer, IEnumerable<object> chunkNodes, SLTCells cells, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < chunkNodes.Count(); i++)
            {
                Dictionary<string, object> chunkNode = chunkNodes.ElementAt(i).ToDictionaryOrNull();
                IEnumerable<object> cellNodes = new List<object>();
                if (chunkNode != null && chunkNode.ContainsKey("cells"))
                    cellNodes = (IEnumerable<object>)chunkNode["cells"];

                List<SLTCell> chunkCells = new List<SLTCell>();
                foreach (var cellNode in cellNodes)
                {
                    int row = 0;
                    int col = 0;

					row = ((IEnumerable<object>)cellNode).ElementAt(0).ToIntegerOrZero();
					col = ((IEnumerable<object>)cellNode).ElementAt(1).ToIntegerOrZero();

					chunkCells.Add(cells.Retrieve(row, col) as SLTCell);
                }

                IEnumerable<object> assetNodes = (IEnumerable<object>)chunkNode["assets"];
                List<SLTChunkAssetRule> chunkAssetRules = new List<SLTChunkAssetRule>();
                foreach (var assetNode in assetNodes)
                {
                    string assetId = "";
                    string distribytionType = "";
                    float distributionVale = 0;
                    IEnumerable<object> states = new List<object>();

                    if (assetNode.ToDictionaryOrNull() != null && assetNode.ToDictionaryOrNull().ContainsKey("assetId"))
                        assetId = assetNode.ToDictionaryOrNull()["assetId"].ToString();

                    if (assetNode.ToDictionaryOrNull() != null && assetNode.ToDictionaryOrNull().ContainsKey("distributionType"))
                        distribytionType = assetNode.ToDictionaryOrNull()["distributionType"].ToString();

                    if (assetNode.ToDictionaryOrNull() != null && assetNode.ToDictionaryOrNull().ContainsKey("distributionValue"))
                        distributionVale = assetNode.ToDictionaryOrNull()["distributionValue"].ToFloatOrZero();

                    if (assetNode.ToDictionaryOrNull() != null && assetNode.ToDictionaryOrNull().ContainsKey("states"))
                        states = (IEnumerable<object>)assetNode.ToDictionaryOrNull()["states"];

                    chunkAssetRules.Add(new SLTChunkAssetRule(assetId, distribytionType, distributionVale, states));
                }
                layer.AddChunk(new SLTChunk(layer.Token, layer.Index, chunkCells, chunkAssetRules, assetMap));
            }
        }


        private SLTMatchingBoard ParseLevelBoard(Dictionary<string, object> boardNode, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boardProperties = new Dictionary<string, object>();
            if (boardNode.ContainsKey("properties"))
            {
                boardProperties = boardNode["properties"].ToDictionaryOrNull();
            }

            SLTCells cells = new SLTCells(boardNode["cols"].ToIntegerOrZero(), boardNode["rows"].ToIntegerOrZero());
            InitializeCells(cells, boardNode);
            List<SLTBoardLayer> layers = new List<SLTBoardLayer>();
            IEnumerable<object> layerNodes = (IEnumerable<object>)boardNode["layers"];
            for (int i = 0; i < layerNodes.Count(); i++)
            {
                Dictionary<string, object> layerNode = layerNodes.ElementAt(i).ToDictionaryOrNull();
                SLTMatchingBoardLayer layer = ParseLayer(layerNode, i, cells, assetMap);
                layers.Add(layer);
            }

            return new SLTMatchingBoard(cells, layers, boardProperties);
        }


        private SLTMatchingBoardLayer ParseLayer(Dictionary<string, object> layerNode, int layerIndex, SLTCells cells, Dictionary<string, object> assetMap)
        {
			//temporarily checking for 2 names until "layerId" is removed!
			string token = (layerNode.ContainsKey("token")) ? layerNode.GetValue<string>("token") : layerNode.GetValue<string>("layerId");
            SLTMatchingBoardLayer layer = new SLTMatchingBoardLayer(token, layerIndex);

            ParseFixedAssets(layer, (IEnumerable<object>)layerNode["fixedAssets"], cells, assetMap);
            ParseLayerChunks(layer, (IEnumerable<object>)layerNode["chunks"], cells, assetMap);

            return layer;
        }


        private void ParseFixedAssets(SLTMatchingBoardLayer layer, IEnumerable<object> assetNodes, SLTCells cells, Dictionary<string, object> assetMap)
        {
            for (int i = 0; i < assetNodes.Count(); i++)
            {
                Dictionary<string, object> assetInstanceNode = assetNodes.ElementAt(i).ToDictionaryOrNull();
                SLTAsset asset = assetMap[assetInstanceNode["assetId"].ToString()] as SLTAsset;
                IEnumerable<object> stateIds = (IEnumerable<object>)assetInstanceNode["states"];
                IEnumerable<object> cellPositions = (IEnumerable<object>)assetInstanceNode["cells"];

                for (int j = 0; j < cellPositions.Count(); j++)
                {
                    IEnumerable<object> position = (IEnumerable<object>)cellPositions.ElementAt(j);
                    SLTCell cell = cells.Retrieve(position.ElementAt(0).ToIntegerOrZero(), position.ElementAt(1).ToIntegerOrZero());
                    cell.SetAssetInstance(layer.Token, layer.Index, new SLTAssetInstance(asset.Token, asset.GetInstanceStates(stateIds), asset.Properties));
                }
            }
        }



        public override Dictionary<string, object> ParseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();

            foreach (var boardId in boardNodes.Keys)
            {
                Dictionary<string, object> boardNode = boardNodes[boardId].ToDictionaryOrNull();
                boards[boardId] = ParseLevelBoard(boardNode, assetMap);
            }
            return boards;
        }


    }
}