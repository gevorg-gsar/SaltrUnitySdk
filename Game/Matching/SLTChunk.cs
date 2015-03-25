using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
{
    // <summary>
    // Represents a chunk, a collection of cells on matching board that is populated with assets according to certain rules.
    // </summary>
    public class SLTChunk
    {
        #region Properties

        public int? ChunkId { get; set; }

        public List<List<int>> Cells { get; set; }

        public List<SLTChunkAssetConfig> Assets { get; set; }

        #endregion Properties

        #region Public Methods

        public void GenerateContent(string layerToken, int layerIndex, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            List<SLTCell> chunkCells = FilterChunkCells(boardCells);

            ResetChunkCells(chunkCells, layerToken, layerIndex);

            List<SLTChunkAssetConfig> countChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> ratioChunkAssetConfigs = new List<SLTChunkAssetConfig>();
            List<SLTChunkAssetConfig> randomChunkAssetConfigs = new List<SLTChunkAssetConfig>();

            foreach (var chunkAsset in Assets)
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

        #endregion public Methods


        #region Internal Methods

        private List<SLTCell> FilterChunkCells(SLTCell[,] boardCells)
        {
            List<SLTCell> chunkCells = new List<SLTCell>();

            foreach (List<int> cell in Cells)
            {

                int chunkAssetRowIndex = cell.ElementAt<int>(1);
                int chunkAssetColIndex = cell.ElementAt<int>(0);

                chunkCells.Add(boardCells[chunkAssetRowIndex, chunkAssetColIndex]);
            }

            return chunkCells;
        }

        private void ResetChunkCells(List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            chunkCells.ForEach(cell => cell.RemoveAsset(layerToken, layerIndex));
        }

        private void GenerateChunkAssetsRandomly(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
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
                        
                        if(i == chunkAssetsCount - 1)
                        {
                            toBeGeneratedAssetsCount = chunkCells.Count;
                        }
                        else
                        {
                            toBeGeneratedAssetsCount = new Random().Next(minAssetCount, maxAssetCount);
                        }                        
                        
                        GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex);
                    }
                }
            }
        }

        private void GenerateChunkAssetsByRatio(List<SLTChunkAssetConfig> chunkAssets, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            if (!chunkAssets.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                float ratioSum = 0;
                chunkAssets.ForEach(chunkAsset => ratioSum += chunkAsset.DistributionValue.Value);

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

                        proportion = chunkAssetConfig.DistributionValue.Value * chunkCells.Count / ratioSum;
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
                    int remainingCellsCount = chunkCells.Count;

                    for (int i = 0; i < remainingCellsCount; i++)
                    {
                        GenerateChunkAssets(1, fractionAssets[i].ChunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex);
                    }
                }
            }
        }

        private void GenerateChunkAssetsByCount(List<SLTChunkAssetConfig> chunkAssetConfigs, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            if (!chunkAssetConfigs.IsNullOrEmpty<SLTChunkAssetConfig>())
            {
                chunkAssetConfigs.ForEach(chunkAssetConfig => GenerateChunkAssets(chunkAssetConfig.DistributionValue.Value, chunkAssetConfig, chunkCells, assetTypes, layerToken, layerIndex));
            }
        }

        private void GenerateChunkAssets(int count, SLTChunkAssetConfig chunkAssetConfig, List<SLTCell> chunkCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            SLTMatchingAssetType assetType = assetTypes[chunkAssetConfig.AssetId];

            for (int i = 0; i < count; i++)
            {
                if (!chunkCells.Any())
                {
                    return;
                }

                SLTCell randomCell = null;
                int randomCellIndex = new Random().Next(0, chunkCells.Count - 1);

                randomCell = chunkCells.ElementAt<SLTCell>(randomCellIndex);
                chunkCells.Remove(randomCell);

                SLTMatchingAsset matchingAsset = new SLTMatchingAsset() { Token = assetType.Token, State = assetType.States[chunkAssetConfig.StateId], Properties = assetType.Properties };

                randomCell.SetAsset(layerToken, layerIndex, matchingAsset);
            }
        }

        #endregion Internal Methods

        #region TempFractionObject

        public struct TempAssetFraction
        {
            public float Fraction { get; set; }
            public SLTChunkAssetConfig ChunkAssetConfig { get; set; }
        }

        #endregion TempFractionObject

    }
}
