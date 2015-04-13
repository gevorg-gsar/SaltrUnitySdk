using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.InternalModel;
using Saltr.UnitySdk.Domain.InternalModel.Canvas;
using Saltr.UnitySdk.Domain.InternalModel.Matching;
using Saltr.UnitySdk.Domain.Model.Matching.Matcher;
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
                    foreach (List<int> blockedCellPosition in board.BlockedCells)
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
                board.Layers.ForEach(layer => layer.RegenerateLayer(boardModel.Cells, assetTypes, index++, board.MatchingRulesEnabled, board.SquareMatchingRuleEnabled, board.AlternativeMatchAssets, board.ExcludedMatchAssets));

                return boardModel;
            }

            return null;
        }

        private static void RegenerateLayer(this SLTMatchingBoardLayer boardLayer, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, int index,
            bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            boardLayer.Index = index;

            boardLayer.FixedAssets.ForEach(fa => RegenerateLayerFixedAsset(fa, boardCells, assetTypes, boardLayer.Token, boardLayer.Index.Value, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets));
            boardLayer.Chunks.ForEach(chunk => chunk.RegenerateChunkContent(boardLayer.Token, boardLayer.Index.Value, boardCells, assetTypes, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets));
        }

        private static void RegenerateLayerFixedAsset(SLTMatchingFixedAssetConfig fixedAssetConfig, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex,
            bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            SLTMatchingAssetType assetType = assetTypes[fixedAssetConfig.AssetId];
            foreach (List<int> position in fixedAssetConfig.Cells)
            {
                SLTCell fixedAssetCell = boardCells[position[1], position[0]];
                SLTMatchingAsset fixedAsset = new SLTMatchingAsset() { Token = assetType.Token, State = assetType.States[fixedAssetConfig.StateId], Properties = assetType.Properties };
                fixedAssetCell.SetAsset(layerToken, layerIndex, fixedAsset);
            }
        }

        public static void RegenerateChunkContent(this SLTChunk chunk, string layerToken, int layerIndex, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes,
            bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            List<SLTCell> chunkCells = chunk.FilterChunkCells(boardCells);

            ResetChunkCells(chunkCells, layerToken, layerIndex);

            List<SLTChunkAssetConfig> countChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> ratioChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> randomChunkAssetConfigs = new List<SLTChunkAssetConfig>();

            foreach (var chunkAssetConfig in chunk.Assets)
            {
                switch (chunkAssetConfig.DistributionType)
                {
                    case ChunkAssetDistributionType.Count:
                        countChunkAssetConfigs.Add(chunkAssetConfig);
                        break;
                    case ChunkAssetDistributionType.Ratio:
                        ratioChunkAssetConfigs.Add(chunkAssetConfig);
                        break;
                    case ChunkAssetDistributionType.Random:
                        randomChunkAssetConfigs.Add(chunkAssetConfig);
                        break;
                }
            }

            int unresolvedAssetCount = 0;
            unresolvedAssetCount += GenerateChunkAssetsByCount(countChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets);
            unresolvedAssetCount += GenerateChunkAssetsByRatio(ratioChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets);
            unresolvedAssetCount += GenerateChunkAssetsRandomly(randomChunkAssetConfigs, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets);

            CorrectRemainingChunkCellsToMatchRules(boardCells, chunkCells, assetTypes, chunk.Assets, layerToken, layerIndex, unresolvedAssetCount, matchingRulesEnabled, squareMatchingRuleEnabled, alternativeMatchAssets, excludedMatchAssets);

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

        private static int GenerateChunkAssetsRandomly(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex,
            SLTCell[,] boardCells, bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            int unresolvedAssetCount = 0;

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

                        unresolvedAssetCount += GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, excludedMatchAssets);
                    }
                }
            }

            return unresolvedAssetCount;
        }

        private static int GenerateChunkAssetsByRatio(List<SLTChunkAssetConfig> chunkAssets, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex,
            SLTCell[,] boardCells, bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            int unresolvedAssetCount = 0;

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
                            return unresolvedAssetCount;
                        }

                        proportion = chunkAssetConfig.DistributionValue.Value * availableCellsCount / ratioSum;
                        toBeGeneratedAssetsCount = UnityEngine.Mathf.FloorToInt(proportion);

                        TempAssetFraction fractObject = new TempAssetFraction()
                        {
                            Fraction = proportion - toBeGeneratedAssetsCount,
                            ChunkAssetConfig = chunkAssetConfig
                        };

                        fractionAssets.Add(fractObject);

                        unresolvedAssetCount += GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, excludedMatchAssets);
                    }

                    //fractionAssets = fractionAssets.OrderBy(fa => fa.Fraction).ToList();
                    fractionAssets = fractionAssets.OrderByDescending(fa => fa.Fraction).ToList();
                    availableCellsCount = chunkCells.Count;

                    for (int i = 0; i < availableCellsCount; i++)
                    {
                        unresolvedAssetCount += GenerateChunkAssets(1, fractionAssets[i % fractionAssets.Count].ChunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, excludedMatchAssets);
                    }
                }
            }

            return unresolvedAssetCount;
        }

        private static int GenerateChunkAssetsByCount(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex,
            SLTCell[,] boardCells, bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            int unresolvedAssetCount = 0;

            if (!chunkAssetConfigs.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                chunkAssetConfigs.ForEach(chunkAssetConfig => unresolvedAssetCount += GenerateChunkAssets(chunkAssetConfig.DistributionValue.Value, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, excludedMatchAssets));
            }

            return unresolvedAssetCount;
        }

        private static int GenerateChunkAssets(int count, SLTChunkAssetConfig chunkAssetConfig, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex,
            SLTCell[,] boardCells, bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> excludedMatchAssets)
        {
            int unresolvedAssetCount = 0;
            SLTMatchingAssetType assetType = assetTypes[chunkAssetConfig.AssetId];

            try
            {
                for (int i = 0; i < count; i++)
                {
                    if (!chunkCells.Any())
                    {
                        return unresolvedAssetCount;
                    }

                    SLTCell randomCell = null;
                    int randomCellIndex = Random.Range(0, chunkCells.Count);
                    randomCell = chunkCells.ElementAt<SLTCell>(randomCellIndex);

                    SLTMatchingAsset matchingAsset = new SLTMatchingAsset()
                    {
                        Token = assetType.Token,
                        State = assetType.States[chunkAssetConfig.StateId],
                        Properties = assetType.Properties
                    };

                    chunkCells.Remove(randomCell);
                    randomCell.SetAsset(layerToken, layerIndex, matchingAsset);

                    if (matchingRulesEnabled.HasValue && matchingRulesEnabled.Value
                        && !excludedMatchAssets.Any(sltConfig => (sltConfig.AssetId == chunkAssetConfig.AssetId && sltConfig.StateId == chunkAssetConfig.StateId)))
                    {
                        Dictionary<SLTMatchPattern, List<List<SLTCell>>> matchGroups = SLTMatchManager.GetMatchGroups(boardCells, layerToken, layerIndex, squareMatchingRuleEnabled.Value);
                        if (SLTMatchManager.IsInAllMatchCells(matchGroups, randomCell))
                        {
                            chunkCells.Add(randomCell);
                            randomCell.RemoveAsset(layerToken, layerIndex);

                            unresolvedAssetCount += CorrectGeneratedAsssetsToMatchRules(boardCells, chunkCells, matchingAsset, layerToken, layerIndex, squareMatchingRuleEnabled.Value);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return unresolvedAssetCount;
        }

        public static int CorrectGeneratedAsssetsToMatchRules(SLTCell[,] boardCells, List<SLTCell> chunkCells, SLTMatchingAsset matchingAsset, string layerToken, int layerIndex, bool squareMatchingRuleEnabled)
        {
            int unresolvedAssetCount = 0;

            int chunkCellsCount = chunkCells.Count;
            for (int j = 0; j < chunkCellsCount; j++)
            {
                SLTCell matchingRuleEnabledChunkCell = chunkCells[j];

                chunkCells.Remove(matchingRuleEnabledChunkCell);
                matchingRuleEnabledChunkCell.SetAsset(layerToken, layerIndex, matchingAsset);

                Dictionary<SLTMatchPattern, List<List<SLTCell>>> matchGroups = SLTMatchManager.GetMatchGroups(boardCells, layerToken, layerIndex, squareMatchingRuleEnabled);
                if (SLTMatchManager.IsInAllMatchCells(matchGroups, matchingRuleEnabledChunkCell))
                {
                    ++unresolvedAssetCount;

                    chunkCells.Add(matchingRuleEnabledChunkCell);
                    matchingRuleEnabledChunkCell.RemoveAsset(layerToken, layerIndex);
                }
                else
                {
                    unresolvedAssetCount = 0;
                    break;
                }
            }

            return unresolvedAssetCount == 0 ? 0 : 1;
        }

        public static void CorrectRemainingChunkCellsToMatchRules(SLTCell[,] boardCells, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, List<SLTChunkAssetConfig> chunkAssetConfigs, string layerToken, int layerIndex, int unresolvedAssetCount,
            bool? matchingRulesEnabled, bool? squareMatchingRuleEnabled, List<SLTAssetConfig> alternativeMatchAssets, List<SLTAssetConfig> excludedMatchAssets)
        {
            if ((matchingRulesEnabled.HasValue && matchingRulesEnabled.Value)
                || squareMatchingRuleEnabled.HasValue && squareMatchingRuleEnabled.Value)
            {
                int chunkCellsCount = chunkCells.Count;
                for (int i = 0; (i < chunkCellsCount && unresolvedAssetCount > 0); i++)
                {
                    for (int j = 0; (j < chunkAssetConfigs.Count && unresolvedAssetCount > 0); j++)
                    {
                        int tmpUnresolvedAssetCount = GenerateChunkAssets(1, chunkAssetConfigs[j], chunkCells, assetTypes, layerToken, layerIndex, boardCells, matchingRulesEnabled, squareMatchingRuleEnabled, excludedMatchAssets);

                        if (tmpUnresolvedAssetCount == 0)
                        {
                            --unresolvedAssetCount;
                        }
                    }
                }

                while (chunkCells.Any())
                {
                    SLTCell chunkCell = chunkCells[0];
                    int alternativeAssetConfigIndex = Random.Range(0, alternativeMatchAssets.Count);
                    SLTAssetConfig alternativeAssetConfig = alternativeMatchAssets.ElementAt<SLTAssetConfig>(alternativeAssetConfigIndex);
                    SLTMatchingAssetType assetType = assetTypes[alternativeAssetConfig.AssetId];

                    SLTMatchingAsset alternativeAsset = new SLTMatchingAsset()
                    {
                        Token = assetType.Token,
                        State = assetType.States[alternativeAssetConfig.StateId],
                        Properties = assetType.Properties
                    };

                    chunkCells.Remove(chunkCell);
                    chunkCell.SetAsset(layerToken, layerIndex, alternativeAsset);
                }
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