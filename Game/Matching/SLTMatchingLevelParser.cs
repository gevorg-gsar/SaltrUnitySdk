using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Saltr.UnitySdk.Utils;
using System;

namespace Saltr.UnitySdk.Game.Matching
{

    internal class SLTMatchingLevelParser : SLTLevelParser
    {
        private static SLTMatchingLevelParser INSTANCE = null;

        private SLTMatchingLevelParser() { }

        public static SLTMatchingLevelParser Instance
        {
            get
            {
                if (INSTANCE == null)
                {
                    INSTANCE = new SLTMatchingLevelParser();
                }
                return INSTANCE;
            }
        }


        private static void InitializeCells(SLTCells cells, object boardNode)
        {
            Dictionary<string, object> boardNodeDict = boardNode as Dictionary<string, object>;

            IEnumerable<object> blockedCells = boardNodeDict.ContainsKey("blockedCells") ? (IEnumerable<object>)boardNodeDict["blockedCells"] : new List<object>();
            IEnumerable<object> cellProperties = boardNodeDict.ContainsKey("cellProperties") ? (IEnumerable<object>)boardNodeDict["cellProperties"] : new List<object>();
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

                Dictionary<string, object> propertyDict = property as Dictionary<string, object>;

                if (propertyDict != null)
                {

                    IEnumerable<object> coords = (IEnumerable<object>)propertyDict["coords"];
                    int cord1, cord2;
                    int.TryParse(coords.ElementAt(0).ToString(), out cord1);
                    int.TryParse(coords.ElementAt(1).ToString(), out cord2);

                    SLTCell cell2 = cells.Retrieve(cord1, cord2);
                    if (cell2 != null)
                        cell2.Properties = propertyDict["value"] as Dictionary<string, object>;
                }
            }


            //blocking cells
            for (int b = 0; b < blockedCells.Count(); b++)
            {
                IEnumerable<object> blokedCell = (IEnumerable<object>)blockedCells.ElementAt(b);
                int col, row;
                int.TryParse(blokedCell.ElementAt(0).ToString(), out col);
                int.TryParse(blokedCell.ElementAt(1).ToString(), out row);

                var cell3 = cells.Retrieve(col, row);
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
                Dictionary<string, object> chunkNode = chunkNodes.ElementAt(i) as Dictionary<string, object>;
                IEnumerable<object> cellNodes = new List<object>();
                if (chunkNode != null && chunkNode.ContainsKey("cells"))
                    cellNodes = (IEnumerable<object>)chunkNode["cells"];

                List<SLTCell> chunkCells = new List<SLTCell>();
                foreach (var cellNode in cellNodes)
                {
                    int col = 0;
                    int row = 0;

                    int.TryParse(((IEnumerable<object>)cellNode).ElementAt(0).ToString(), out col);
                    int.TryParse(((IEnumerable<object>)cellNode).ElementAt(1).ToString(), out row);

                    chunkCells.Add(cells.Retrieve(col, row) as SLTCell);
                }

                IEnumerable<object> assetNodes = (IEnumerable<object>)chunkNode["assets"];
                List<SLTChunkAssetRule> chunkAssetRules = new List<SLTChunkAssetRule>();
                foreach (var assetNodeObj in assetNodes)
                {
                    string assetId = "";
                    float distributionVale = 0;
                    IEnumerable<object> states = new List<object>();
                    ChunkAssetRuleDistributionType distribytionType = ChunkAssetRuleDistributionType.Unknown;

                    var assetNode = assetNodeObj as Dictionary<string, object>;

                    if (assetNode != null)
                    {

                        if (assetNode.ContainsKey("assetId"))
                        {
                            assetId = assetNode["assetId"].ToString();
                        }

                        if (assetNode.ContainsKey("distributionType"))
                        {
                            distribytionType = (ChunkAssetRuleDistributionType)Enum.Parse(typeof(ChunkAssetRuleDistributionType), assetNode.ToString(), true);
                        }

                        if (assetNode.ContainsKey("distributionValue"))
                        {
                            float.TryParse(assetNode["distributionValue"].ToString(), out distributionVale);
                        }

                        if (assetNode.ContainsKey("states"))
                        {
                            states = (IEnumerable<object>)assetNode["states"];
                        }
                    }

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
                boardProperties = boardNode["properties"] as Dictionary<string, object>;
            }

            int width, height;

            int.TryParse(boardNode["cols"].ToString(), out width);
            int.TryParse(boardNode["rows"].ToString(), out height);

            SLTCells cells = new SLTCells(width, height);
            InitializeCells(cells, boardNode);
            List<SLTBoardLayer> layers = new List<SLTBoardLayer>();
            IEnumerable<object> layerNodes = (IEnumerable<object>)boardNode["layers"];
            for (int i = 0; i < layerNodes.Count(); i++)
            {
                Dictionary<string, object> layerNode = layerNodes.ElementAt(i) as Dictionary<string, object>;
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
                Dictionary<string, object> assetInstanceNode = assetNodes.ElementAt(i) as Dictionary<string, object>;
                SLTAsset asset = assetMap[assetInstanceNode["assetId"].ToString()] as SLTAsset;
                IEnumerable<object> stateIds = (IEnumerable<object>)assetInstanceNode["states"];
                IEnumerable<object> cellPositions = (IEnumerable<object>)assetInstanceNode["cells"];

                for (int j = 0; j < cellPositions.Count(); j++)
                {
                    IEnumerable<object> position = (IEnumerable<object>)cellPositions.ElementAt(j);
                    int col, row;

                    int.TryParse(position.ElementAt(0).ToString(), out col);
                    int.TryParse(position.ElementAt(1).ToString(), out row);

                    SLTCell cell = cells.Retrieve(col, row);
                    cell.SetAssetInstance(layer.Token, layer.Index, new SLTAssetInstance(asset.Token, asset.GetInstanceStates(stateIds), asset.Properties));
                }
            }
        }
        
        public override Dictionary<string, object> ParseLevelContent(Dictionary<string, object> boardNodes, Dictionary<string, object> assetMap)
        {
            Dictionary<string, object> boards = new Dictionary<string, object>();
            try
            {
                foreach (var boardId in boardNodes.Keys)
                {
                    Dictionary<string, object> boardNode = boardNodes[boardId] as Dictionary<string, object>;
                    boards[boardId] = ParseLevelBoard(boardNode, assetMap);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[SALTR: ERROR] Level content boards parsing failed." + e.Message);
            }

            return boards;
        }


    }
}