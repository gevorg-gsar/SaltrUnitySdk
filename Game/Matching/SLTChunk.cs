using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
	// <summary>
	// Represents a chunk, a collection of cells on matching board that is populated with assets according to certain rules.
	// </summary>
    internal class SLTChunk
    {
        private string _layerToken;
        private int _layerIndex;
         

        private List<SLTChunkAssetRule> _chunkAssetRules;
        private List<SLTCell> _chunkCells;
        private List<SLTCell> _availableCells;
        private Dictionary<string, object> _assetMap;

        internal SLTChunk(string layerToken, int layerIndex, List<SLTCell> chunkCells, List<SLTChunkAssetRule> chunkAssetInfos, Dictionary<string,object> assetMap)
        {
            _layerIndex = layerIndex;
            _layerToken = layerToken;
            _chunkCells = chunkCells;
			_chunkAssetRules = chunkAssetInfos;
			_assetMap = assetMap;

            //_availableCells = new List<SLTCell>();
        }

		void ResetChunkCells ()
		{
			foreach(SLTCell cell in _chunkCells)
			{
				cell.RemoveAssetInstance(_layerToken, _layerIndex);
			}
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

            for (int i = 0; i < _chunkAssetRules.Count; i++)
            {
                SLTChunkAssetRule assetRule = _chunkAssetRules[i];
                switch (assetRule.DistributionType)
                {
                    case "count":
                        countChunkAssetRules.Add(assetRule);
                        break;
                    case "ratio":
                        ratioChunkAssetRules.Add(assetRule);
                        break;
                    case "random":
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

        private void GenerateAssetInstancesRandomly(List<SLTChunkAssetRule> randomChunkAssetRules)
        {
            int len = randomChunkAssetRules.Count;
            uint availableCellsNum = (uint)_availableCells.Count;
            if (len > 0)
            {
                float assetConcentration = availableCellsNum > len ? availableCellsNum / len : 1;
                uint minAssetCount = (uint)(assetConcentration <= 2 ? 1 : assetConcentration - 2);
                uint maxAssetCount = (uint)(assetConcentration == 1 ? 1 : assetConcentration + 2);
                int lastChunkAssetIndex = len - 1;
                SLTChunkAssetRule chunkAssetRule;
                int count = 0;

                for (int i = 0; i < len && _availableCells.Count > 0; i++)
                {

                    chunkAssetRule = randomChunkAssetRules[i];
                    count = i == lastChunkAssetIndex ? _availableCells.Count : (int)RandomWithin(minAssetCount, maxAssetCount);
                    GenerateAssetInstances(count, chunkAssetRule.AssetId, chunkAssetRule.StateId);
                }
            }
        }

        private void GenerateAssetInstancesByRatio(List<SLTChunkAssetRule> ratioChunkAssetRules)
        {
            float ratioSum = 0;
            int len = ratioChunkAssetRules.Count;
            SLTChunkAssetRule assetRule;
            for (int i = 0; i < len; i++)
            {
                assetRule = ratioChunkAssetRules[i];
                ratioSum += assetRule.DistributionValue;
            }
            int availableCellsNum = _availableCells.Count;
            float proportion = 0;
            int count;
            List<TempFractionObject> fractionAssets = new List<TempFractionObject>();
            if (ratioSum != 0)
            {
                for (int j = 0; j < len; j++)
                {
                    assetRule = ratioChunkAssetRules[j];

                    if (ratioSum * availableCellsNum != 0)
						proportion = assetRule.DistributionValue * availableCellsNum / ratioSum;

                    count = (int)proportion;

                    TempFractionObject fractObject = new TempFractionObject()
                    {
                        fraction = (int)(proportion - count),
                        assetRule = assetRule
                    };

                    fractionAssets.Add(fractObject);

                    GenerateAssetInstances(count, assetRule.AssetId, assetRule.StateId);
                }

                fractionAssets.Sort(new TempFractionComparer());
                availableCellsNum = _availableCells.Count;

                for (int k = 0; k < availableCellsNum; k++)
                {
                    GenerateAssetInstances(1, fractionAssets[k].assetRule.AssetId, fractionAssets[k].assetRule.StateId);
                }
            }
        }

        internal class TempFractionComparer : IComparer<TempFractionObject>
        {
            public int Compare(TempFractionObject x, TempFractionObject y)
            {
                if (x == null & y == null)
                    return 1;
                if (x == null && y != null)
                    return 1;
                if (x != null && y == null)
                    return -1;
                if (x.fraction > y.fraction)
                    return -1;
                if (x.fraction < y.fraction)
                    return 1;
                if (x.fraction == y.fraction)
                    return 1;

                return 0;
            }
        }


        internal class TempFractionObject 
        {
            public int fraction { get; set; }
            public SLTChunkAssetRule assetRule { get; set; }
        }

        private void GenerateAssetInstancesByCount(List<SLTChunkAssetRule> countChunkAssetRules)
        {
            for (int i = 0; i < countChunkAssetRules.Count; i++)
            {
                SLTChunkAssetRule assetRule = countChunkAssetRules[i];
                GenerateAssetInstances(assetRule.DistributionValue, assetRule.AssetId, assetRule.StateId);
            }
        }

        public override string ToString()
        {
            return "[Chunk] cells:" + _availableCells.Count + ", " + " chunkAssets: " + _chunkAssetRules.Count;
        }


        private void GenerateAssetInstances(float count, string assetId, IEnumerable<object> stateIds)
        {
            SLTAsset asset = _assetMap[assetId] as SLTAsset;

            SLTCell randCell = new SLTCell(0,0);
            int randCellIndex;

            for (int i = 0; i < count; i++)
            {
                randCellIndex = new Random().Next(0, _availableCells.Count );

                if(_availableCells.Any())
                randCell = _availableCells[randCellIndex];

				randCell.SetAssetInstance( _layerToken , _layerIndex, new SLTAssetInstance(asset.Token, asset.GetInstanceStates(stateIds), asset.Properties));

                if(_availableCells.Any())
                _availableCells.RemoveAt(randCellIndex);
                if (_availableCells.Count == 0)
                    return;
            }
        }


        private void GenerateWeakAssetsInstances(List<SLTChunkAssetRule> weakChunkAssetInfos)
        {
            int len = weakChunkAssetInfos.Count;

            if (len > 0)
            {
                float assetConcentration = _availableCells.Count > len ? _availableCells.Count / len : 1;
                uint minAssetCount = (uint)(assetConcentration <= 2 ? 1 : assetConcentration - 2);

                uint maxAssetCount = (uint)(assetConcentration == 1 ? 1 : assetConcentration + 2);
                int lastChunkAssetIndex = len - 1;

                for (int i = 0; i < len; i++)
                {
                    SLTChunkAssetRule chunkAssetInfo = weakChunkAssetInfos[i];
                    uint count = (uint)(i == lastChunkAssetIndex ? _availableCells.Count : RandomWithin(minAssetCount, maxAssetCount));
                }
            }
        }

        private static float RandomWithin(float min, float max, bool isFloat)
        {
            return isFloat ? new Random().Next(0, (int)(1 + (max - min))) + min : (int)(new Random().Next(0, (int)(1 + max - min))) + min;
        }

		private static float RandomWithin(float min, float max)
		{
			return RandomWithin(min,max,false);
		}
    }
}
