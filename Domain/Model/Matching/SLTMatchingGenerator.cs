using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.Game;
using Saltr.UnitySdk.Domain.Game.Canvas;
using Saltr.UnitySdk.Domain.Game.Matching;
using Saltr.UnitySdk.Utils;
using UnityEngine;

namespace Saltr.UnitySdk.Domain.Model.Matching
{
    public static class SLTMatchingGenerator
    {
        public static SLTMatchingBoard RegenerateBoard(this SLTInternalMatchingBoard board, Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            if (board.Rows.HasValue && board.Rows.Value > 0
                && board.Cols.HasValue && board.Cols.Value > 0)
            {
                SLTMatchingBoard boardModel = new SLTMatchingBoard();
                boardModel.Rows = board.Rows.Value;
                boardModel.Cols = board.Cols.Value;
                boardModel.Cells = new SLTCell[boardModel.Rows, boardModel.Cols];

                for (int row = 0; row < boardModel.Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < boardModel.Cells.GetLength(1); col++)
                    {
                        boardModel.Cells[row, col] = new SLTCell(row, col);
                    }
                }

                if (!board.BlockedCells.IsNullOrEmpty<List<int>>())
                {
                    foreach(List<int> blockedCellPosition in board.BlockedCells)
                    {
                        boardModel.Cells[blockedCellPosition[1], blockedCellPosition[0]].IsBlocked = true;
                    }
                }

                if (!board.CellProperties.IsNullOrEmpty<CellProperty>())
                {
                    foreach (CellProperty cellProperty in board.CellProperties)
                    {
                        boardModel.Cells[cellProperty.Coords[1], cellProperty.Coords[0]].Properties = cellProperty.Value;
                    }
                }

                int index = 0;
                board.Layers.ForEach(layer => layer.RegenerateLayer(boardModel.Cells, assetTypes, index++));

                return boardModel;
            }

            return null;
        }

        private static void RegenerateLayer(this SLTMatchingBoardLayer boardLayer, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, int index)
        {
            boardLayer.Index = index;

            boardLayer.FixedAssets.ForEach(fa => RegenerateLayerFixedAsset(fa, boardCells, assetTypes, boardLayer.Token, boardLayer.Index.Value));
            boardLayer.Chunks.ForEach(chunk => chunk.RegenerateChunkContent(boardLayer.Token, boardLayer.Index.Value, boardCells, assetTypes));
        }

        private static void RegenerateLayerFixedAsset(SLTMatchingFixedAssetConfig fixedAssetConfig, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            SLTMatchingAssetType assetType = assetTypes[fixedAssetConfig.AssetId];
            foreach (List<int> position in fixedAssetConfig.Cells)
            {
                SLTCell fixedAssetCell = boardCells[position[1], position[0]];
                SLTMatchingAsset fixedAsset = new SLTMatchingAsset() { Token = assetType.Token, State = assetType.States[fixedAssetConfig.StateId], Properties = assetType.Properties };
                fixedAssetCell.SetAsset(layerToken, layerIndex, fixedAsset);
            }
        }

        public static void RegenerateChunkContent(this SLTChunk chunk, string layerToken, int layerIndex, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            List<SLTCell> chunkCells = chunk.FilterChunkCells(boardCells);

            ResetChunkCells(chunkCells, layerToken, layerIndex);

            List<SLTChunkAssetConfig> countChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> ratioChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> randomChunkAssetConfigs = new List<SLTChunkAssetConfig>();

            foreach (var chunkAsset in chunk.Assets)
            {
                switch (chunkAsset.DistributionType)
                {
                    case ChunkAssetDistributionType.Count:
                        countChunkAssetConfigs.Add(chunkAsset);
                        break;
                    case ChunkAssetDistributionType.Ratio:
                        ratioChunkAssetConfigs.Add(chunkAsset);
                        break;
                    case ChunkAssetDistributionType.Random:
                        randomChunkAssetConfigs.Add(chunkAsset);
                        break;
                }
            }

            GenerateChunkAssetsByCount(countChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex);
            GenerateChunkAssetsByRatio(ratioChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex);
            GenerateChunkAssetsRandomly(randomChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex);
        }

        private static List<SLTCell> FilterChunkCells(this SLTChunk chunk, SLTCell[,] boardCells)
        {
            List<SLTCell> chunkCells = new List<SLTCell>();

            foreach (List<int> cell in chunk.Cells)
            {

                int chunkAssetRowIndex = cell.ElementAt<int>(1);
                int chunkAssetColIndex = cell.ElementAt<int>(0);

                chunkCells.Add(boardCells[chunkAssetRowIndex, chunkAssetColIndex]);
            }

            return chunkCells;
        }

        private static void ResetChunkCells(List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            chunkCells.ForEach(cell => cell.RemoveAsset(layerToken, layerIndex));
        }

        private static void GenerateChunkAssetsRandomly(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            if (!chunkAssetConfigs.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                int chunkAssetsCount = chunkAssetConfigs.Count;

                if (chunkAssetsCount > 0)
                {
                    float assetConcentration = chunkCells.Count > chunkAssetsCount ? chunkCells.Count / chunkAssetsCount : 1;
                    int minAssetCount = (int)(assetConcentration <= 2 ? 1 : assetConcentration - 2);
                    int maxAssetCount = (int)(assetConcentration == 1 ? 1 : assetConcentration + 2);

                    SLTChunkAssetConfig chunkAssetConfig;
                    int toBeGeneratedAssetsCount = 0;

                    for (int i = 0; (0 < chunkCells.Count && i < chunkAssetsCount); i++)
                    {
                        chunkAssetConfig = chunkAssetConfigs[i];

                        if (i == chunkAssetsCount - 1)
                        {
                            toBeGeneratedAssetsCount = chunkCells.Count;
                        }
                        else
                        {
                            //toBeGeneratedAssetsCount = new Random().Next(minAssetCount, maxAssetCount);
                            toBeGeneratedAssetsCount = Random.Range(minAssetCount, maxAssetCount);
                        }

                        GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex);
                    }
                }
            }
        }

        private static void GenerateChunkAssetsByRatio(List<SLTChunkAssetConfig> chunkAssets, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            if (!chunkAssets.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                float ratioSum = 0;
                chunkAssets.ForEach(chunkAsset => ratioSum += chunkAsset.DistributionValue.Value);

                int availableCellsCount = chunkCells.Count;

                if (ratioSum != 0)
                {
                    int toBeGeneratedAssetsCount;
                    float proportion = 0;
                    List<TempAssetFraction> fractionAssets = new List<TempAssetFraction>();

                    foreach (var chunkAssetConfig in chunkAssets)
                    {
                        if (!chunkCells.Any())
                        {
                            return;
                        }

                        proportion = chunkAssetConfig.DistributionValue.Value * availableCellsCount / ratioSum;
                        toBeGeneratedAssetsCount = UnityEngine.Mathf.FloorToInt(proportion);

                        TempAssetFraction fractObject = new TempAssetFraction()
                        {
                            Fraction = proportion - toBeGeneratedAssetsCount,
                            ChunkAssetConfig = chunkAssetConfig
                        };

                        fractionAssets.Add(fractObject);

                        GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex);
                    }

                    //fractionAssets = fractionAssets.OrderBy(fa => fa.Fraction).ToList();
                    fractionAssets = fractionAssets.OrderByDescending(fa => fa.Fraction).ToList();
                    availableCellsCount = chunkCells.Count;

                    for (int i = 0; i < availableCellsCount; i++)
                    {
                        GenerateChunkAssets(1, fractionAssets[i].ChunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex);
                    }
                }
            }
        }

        private static void GenerateChunkAssetsByCount(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            if (!chunkAssetConfigs.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                chunkAssetConfigs.ForEach(chunkAssetConfig => GenerateChunkAssets(chunkAssetConfig.DistributionValue.Value, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex));
            }
        }

        private static void GenerateChunkAssets(int count, SLTChunkAssetConfig chunkAssetConfig, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            SLTMatchingAssetType assetType = assetTypes[chunkAssetConfig.AssetId];

            for (int i = 0; i < count; i++)
            {
                if (!chunkCells.Any())
                {
                    return;
                }

                SLTCell randomCell = null;
                int randomCellIndex = Random.Range(0, chunkCells.Count - 1);

                randomCell = chunkCells.ElementAt<SLTCell>(randomCellIndex);
                chunkCells.Remove(randomCell);

                SLTMatchingAsset matchingAsset = new SLTMatchingAsset() { Token = assetType.Token, State = assetType.States[chunkAssetConfig.StateId], Properties = assetType.Properties };

                randomCell.SetAsset(layerToken, layerIndex, matchingAsset);
            }
        }

    }

    #region TempFractionObject

    public struct TempAssetFraction
    {
        public float Fraction { get; set; }
        public SLTChunkAssetConfig ChunkAssetConfig { get; set; }
    }

    #endregion TempFractionObject
}