using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
    // <summary>
    // Represents a chunk, a collection of cells on matching board that is populated with assets according to certain rules.
    // </summary>
    public class SLTChunk
    {
        #region Fields

        private string _layerToken;
        private int _layerIndex;

        private List<SLTCell> _chunkCells;
        private List<SLTCell> _availableCells;
        private Dictionary<string, object> _assetMap;
        private List<SLTChunkAssetRule> _chunkAssetRules;

        #endregion Fields

        #region Ctor

        public SLTChunk(string layerToken, int layerIndex, List<SLTCell> chunkCells, List<SLTChunkAssetRule> chunkAssetInfos, Dictionary<string, object> assetMap)
        {
            _layerIndex = layerIndex;
            _layerToken = layerToken;
            _chunkCells = chunkCells;
            _chunkAssetRules = chunkAssetInfos;
            _assetMap = assetMap;

            //_availableCells = new List<SLTCell>();
        }

        #endregion Ctor

        #region Internal Methods

        private void ResetChunkCells()
        {
            _chunkCells.ForEach(cell => cell.RemoveAssetInstance(_layerToken, _layerIndex));
        }

        private void GenerateAssetInstancesRandomly(List<SLTChunkAssetRule> randomChunkAssetRules)
        {
            int randomChunkAssetRulesLength = randomChunkAssetRules.Count;
            uint availableCellsNum = (uint)_availableCells.Count;

            if (randomChunkAssetRulesLength > 0)
            {
                float assetConcentration = availableCellsNum > randomChunkAssetRulesLength ? availableCellsNum / randomChunkAssetRulesLength : 1;
                uint minAssetCount = (uint)(assetConcentration <= 2 ? 1 : assetConcentration - 2);
                uint maxAssetCount = (uint)(assetConcentration == 1 ? 1 : assetConcentration + 2);
                int lastChunkAssetIndex = randomChunkAssetRulesLength - 1;
                SLTChunkAssetRule chunkAssetRule;
                int count = 0;

                for (int i = 0; (0 < _availableCells.Count && i < randomChunkAssetRulesLength); i++)
                {
                    chunkAssetRule = randomChunkAssetRules[i];
                    count = (i == lastChunkAssetIndex ? _availableCells.Count : (int)RandomWithin(minAssetCount, maxAssetCount));
                    GenerateAssetInstances(count, chunkAssetRule.AssetId, chunkAssetRule.StateIds);
                }
            }
        }

        private void GenerateAssetInstancesByRatio(List<SLTChunkAssetRule> ratioChunkAssetRules)
        {
            float ratioSum = 0;
            int ratioChunkAssetRulesLength = ratioChunkAssetRules.Count;

            ratioChunkAssetRules.ForEach(assetRule => ratioSum += assetRule.DistributionValue);

            int count;
            float proportion = 0;
            int availableCellsNum = _availableCells.Count;
            List<TempFractionObject> fractionAssets = new List<TempFractionObject>();

            if (ratioSum != 0)
            {
                foreach (var assetRule in ratioChunkAssetRules)
                {
                    if (ratioSum * availableCellsNum != 0)
                    {
                        proportion = assetRule.DistributionValue * availableCellsNum / ratioSum;
                    }

                    count = (int)proportion;

                    TempFractionObject fractObject = new TempFractionObject()
                    {
                        Fraction = (int)(proportion - count),
                        AssetRule = assetRule
                    };

                    fractionAssets.Add(fractObject);

                    GenerateAssetInstances(count, assetRule.AssetId, assetRule.StateIds);
                }

                fractionAssets.Sort(new TempFractionComparer());
                availableCellsNum = _availableCells.Count;

                //TODO: Gor review why fractionAssets are iterated availableCellsNum times. 
                for (int i = 0; i < availableCellsNum; i++)
                {
                    GenerateAssetInstances(1, fractionAssets[i].AssetRule.AssetId, fractionAssets[i].AssetRule.StateIds);
                }
            }
        }

        private void GenerateAssetInstancesByCount(List<SLTChunkAssetRule> countChunkAssetRules)
        {
            countChunkAssetRules.ForEach(assetRule => GenerateAssetInstances(assetRule.DistributionValue, assetRule.AssetId, assetRule.StateIds));
        }

        private void GenerateAssetInstances(float count, string assetId, IEnumerable<object> stateIds)
        {
            SLTAsset asset = _assetMap[assetId] as SLTAsset;

            SLTCell randCell = new SLTCell(0, 0);
            int randCellIndex;

            for (int i = 0; i < count; i++)
            {
                randCellIndex = new Random().Next(0, _availableCells.Count);

                if (_availableCells.Any())
                    randCell = _availableCells[randCellIndex];

                randCell.SetAssetInstance(_layerToken, _layerIndex, new SLTAssetInstance(asset.Token, asset.GetInstanceStates(stateIds), asset.Properties));

                if (_availableCells.Any())
                    _availableCells.RemoveAt(randCellIndex);
                if (_availableCells.Count == 0)
                    return;
            }
        }

        private void GenerateWeakAssetsInstances(List<SLTChunkAssetRule> weakChunkAssetInfos)
        {
            int weakChunkAssetInfosLength = weakChunkAssetInfos.Count;

            if (weakChunkAssetInfosLength > 0)
            {
                float assetConcentration = _availableCells.Count > weakChunkAssetInfosLength ? _availableCells.Count / weakChunkAssetInfosLength : 1;
                uint minAssetCount = (uint)(assetConcentration <= 2 ? 1 : assetConcentration - 2);

                uint maxAssetCount = (uint)(assetConcentration == 1 ? 1 : assetConcentration + 2);
                int lastChunkAssetIndex = weakChunkAssetInfosLength - 1;

                //for (int i = 0; i < weakChunkAssetInfosLength; i++)
                //{
                //    SLTChunkAssetRule chunkAssetInfo = weakChunkAssetInfos[i];
                //    uint count = (uint)(i == lastChunkAssetIndex ? _availableCells.Count : RandomWithin(minAssetCount, maxAssetCount));
                //}
            }
        }

        #endregion Internal Methods

        #region Business Methods
        public override string ToString()
        {
            return "[Chunk] cells:" + _availableCells.Count + ", " + " chunkAssets: " + _chunkAssetRules.Count;
        }

        public void GenerateContent()
        {
            //resetting chunk cells, as when chunk can contain empty cells, previous generation can leave assigned values to cells
            ResetChunkCells();

            //availableCells are being always overwritten here, so no need to initialize
            _availableCells = _chunkCells.ToList();

            //            List<SLTCell> tempCells = new List<SLTCell>();
            //            foreach (var item in _chunkCells)
            //            {
            //                if (item == null)
            //                    continue;
            //                SLTCell cell = new SLTCell(item.col, item.row)
            //                {
            //                    isBlocked = item.isBlocked,
            //                    properties = item.properties
            //                };
            //                tempCells.Add(cell);
            //            }
            //
            //            _availableCells = tempCells;

            List<SLTChunkAssetRule> countChunkAssetRules = new List<SLTChunkAssetRule>();
            List<SLTChunkAssetRule> ratioChunkAssetRules = new List<SLTChunkAssetRule>();
            List<SLTChunkAssetRule> randomChunkAssetRules = new List<SLTChunkAssetRule>();

            foreach (var assetRule in _chunkAssetRules)
            {
                switch (assetRule.DistributionType)
                {
                    case ChunkAssetRuleDistributionType.Count:
                        countChunkAssetRules.Add(assetRule);
                        break;
                    case ChunkAssetRuleDistributionType.Ratio:
                        ratioChunkAssetRules.Add(assetRule);
                        break;
                    case ChunkAssetRuleDistributionType.Random:
                        randomChunkAssetRules.Add(assetRule);
                        break;
                }
            }

            if (countChunkAssetRules.Count > 0)
            {
                GenerateAssetInstancesByCount(countChunkAssetRules);
            }
            if (ratioChunkAssetRules.Count > 0)
            {
                GenerateAssetInstancesByRatio(ratioChunkAssetRules);
            }
            else if (randomChunkAssetRules.Count > 0)
            {
                GenerateAssetInstancesRandomly(randomChunkAssetRules);
            }
            _availableCells.Clear();
        }

        #endregion Business Methods

        #region Util Methods

        private static float RandomWithin(float min, float max, bool isFloat)
        {
            return isFloat ? new Random().Next(0, (int)(1 + (max - min))) + min : (int)(new Random().Next(0, (int)(1 + max - min))) + min;
        }

        private static float RandomWithin(float min, float max)
        {
            return RandomWithin(min, max, false);
        }

        #endregion Util Methods

        #region TempFractionObject

        public class TempFractionObject
        {
            public int Fraction { get; set; }
            public SLTChunkAssetRule AssetRule { get; set; }
        }

        public class TempFractionComparer : IComparer<TempFractionObject>
        {
            public int Compare(TempFractionObject firstTempFractionObject, TempFractionObject secondTempFractionObject)
            {
                if (firstTempFractionObject == null & secondTempFractionObject == null)
                    return 1;
                if (firstTempFractionObject == null && secondTempFractionObject != null)
                    return 1;
                if (firstTempFractionObject != null && secondTempFractionObject == null)
                    return -1;
                if (firstTempFractionObject.Fraction > secondTempFractionObject.Fraction)
                    return -1;
                if (firstTempFractionObject.Fraction < secondTempFractionObject.Fraction)
                    return 1;
                if (firstTempFractionObject.Fraction == secondTempFractionObject.Fraction)
                    return 1;

                return 0;
            }
        }

        #endregion TempFractionObject

    }
}
