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
        #region Fields

        public int? ChunkId { get; set; }

        public List<List<int>> Cells { get; set; }

        public List<SLTChunkAsset> Assets { get; set; }

        #endregion Fields

        #region Public Methods

        public void GenerateContent(string layerToken, int layerIndex, SLTCell[,] boardCells)
        {
            List<SLTCell> chunkCells = FilterChunkCells(boardCells);

            ResetChunkCells(chunkCells, layerToken, layerIndex);

            List<SLTChunkAsset> countChunkAssets = new List<SLTChunkAsset>();
            List<SLTChunkAsset> ratioChunkAssets = new List<SLTChunkAsset>();
            List<SLTChunkAsset> randomChunkAssets = new List<SLTChunkAsset>();

            foreach (var chunkAsset in Assets)
            {
                switch (chunkAsset.DistributionType)
                {
                    case ChunkAssetDistributionType.Count:
                        countChunkAssets.Add(chunkAsset);
                        break;
                    case ChunkAssetDistributionType.Ratio:
                        ratioChunkAssets.Add(chunkAsset);
                        break;
                    case ChunkAssetDistributionType.Random:
                        randomChunkAssets.Add(chunkAsset);
                        break;
                }
            }

            GenerateChunkAssetsByCount(countChunkAssets, chunkCells, layerToken, layerIndex);
            GenerateChunkAssetsByRatio(ratioChunkAssets, chunkCells, layerToken, layerIndex);
            //GenerateChunkAssetsRandomly(randomChunkAssets);
        }

        #endregion public Methods


        #region Internal Methods

        private List<SLTCell> FilterChunkCells(SLTCell[,] boardCells)
        {
            List<SLTCell> chunkCells = new List<SLTCell>();

            foreach (List<int> cell in Cells)
            {

                int chunkAssetRowIndex = cell.ElementAt<int>(0);
                int chunkAssetColIndex = cell.ElementAt<int>(1);

                chunkCells.Add(boardCells[chunkAssetRowIndex, chunkAssetColIndex]);
            }

            return chunkCells;
        }

        private void ResetChunkCells(List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            chunkCells.ForEach(cell => cell.RemoveAsset(layerToken, layerIndex));
        }

        private void GenerateChunkAssetsRandomly(List<SLTChunkAsset> chunkAssets, List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            if (!chunkAssets.IsNullOrEmpty<SLTChunkAsset>())
            {
                int chunkAssetsCount = chunkAssets.Count;
                
                if (chunkAssetsCount > 0)
                {
                    float assetConcentration = chunkCells.Count > chunkAssetsCount ? chunkCells.Count / chunkAssetsCount : 1;
                    int minAssetCount = (int)(assetConcentration <= 2 ? 1 : assetConcentration - 2);
                    int maxAssetCount = (int)(assetConcentration == 1 ? 1 : assetConcentration + 2);

                    SLTChunkAsset chunkAsset;
                    int toBeGeneratedAssetsCount = 0;

                    for (int i = 0; (0 < chunkCells.Count && i < chunkAssetsCount); i++)
                    {
                        chunkAsset = chunkAssets[i];
                        
                        if(i == chunkAssetsCount - 1)
                        {
                            toBeGeneratedAssetsCount = chunkCells.Count;
                        }
                        else
                        {
                            toBeGeneratedAssetsCount = new Random().Next(minAssetCount, maxAssetCount);
                        }                        
                        
                        GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAsset, chunkCells, layerToken, layerIndex);
                    }
                }
            }
        }

        private void GenerateChunkAssetsByRatio(List<SLTChunkAsset> chunkAssets, List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            if (!chunkAssets.IsNullOrEmpty<SLTChunkAsset>())
            {
                float ratioSum = 0;
                chunkAssets.ForEach(chunkAsset => ratioSum += chunkAsset.DistributionValue.Value);

                if (ratioSum != 0)
                {
                    int toBeGeneratedAssetsCount;
                    float proportion = 0;
                    List<TempAssetFraction> fractionAssets = new List<TempAssetFraction>();

                    foreach (var chunkAsset in chunkAssets)
                    {
                        if (!chunkCells.Any())
                        {
                            return;
                        }

                        proportion = chunkAsset.DistributionValue.Value * chunkCells.Count / ratioSum;
                        toBeGeneratedAssetsCount = (int)proportion;

                        TempAssetFraction fractObject = new TempAssetFraction()
                        {
                            Fraction = proportion - toBeGeneratedAssetsCount,
                            ChunkAsset = chunkAsset
                        };

                        fractionAssets.Add(fractObject);

                        GenerateChunkAssets(toBeGeneratedAssetsCount, chunkAsset, chunkCells, layerToken, layerIndex);
                    }

                    fractionAssets = fractionAssets.OrderByDescending(fa => fa.Fraction).ToList();
                    
                    for (int i = 0; i < chunkCells.Count; i++)
                    {
                        GenerateChunkAssets(1, fractionAssets[i].ChunkAsset, chunkCells, layerToken, layerIndex);
                    }
                }
            }
        }

        private void GenerateChunkAssetsByCount(List<SLTChunkAsset> chunkAssets, List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            if (!chunkAssets.IsNullOrEmpty<SLTChunkAsset>())
            {
                chunkAssets.ForEach(chunkAsset => GenerateChunkAssets(chunkAsset.DistributionValue.Value, chunkAsset, chunkCells, layerToken, layerIndex));
            }
        }

        private void GenerateChunkAssets(int count, SLTChunkAsset chunkAsset, List<SLTCell> chunkCells, string layerToken, int layerIndex)
        {
            for (int i = 0; i < count; i++)
            {
                if (chunkCells.Any())
                {
                    return;
                }

                SLTCell randomCell = null;
                int randomCellIndex = new Random().Next(0, chunkCells.Count);

                randomCell = chunkCells.ElementAt<SLTCell>(randomCellIndex);
                chunkCells.Remove(randomCell);

                randomCell.SetAsset(layerToken, layerIndex, chunkAsset);
            }
        }

        #endregion Internal Methods

        #region TempFractionObject

        public struct TempAssetFraction
        {
            public float Fraction { get; set; }
            public SLTChunkAsset ChunkAsset { get; set; }
        }

        #endregion TempFractionObject

    }
}
