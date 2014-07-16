using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTChunk
    {
        private SLTBoardLayer _layer;
        private List<SLTChunkAssetRule> _chunkAssetRules;
        private List<SLTCell> _chunkCells;
        private List<SLTCell> _availableCells;
        private Dictionary<string, object> _assetMap;

        public SLTChunk(SLTBoardLayer layer, List<SLTCell> chunkCells, List<SLTChunkAssetRule> chunkAssetInfos, Dictionary<string,object> assetMap)
        {
            _layer = layer;
            _chunkCells = chunkCells;
			_chunkAssetRules = chunkAssetInfos;
			_assetMap = assetMap;

            //_availableCells = new List<SLTCell>();
        }

		void resetChunkCells ()
		{
			foreach(SLTCell cell in _chunkCells)
			{
				cell.removeAssetInctance(_layer.layerId, _layer.layerIndex);
			}
		}

        public void generateContent()
        {
			resetChunkCells();

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
                switch (assetRule.distributionType)
                {
                    case "count":
                        countChunkAssetRules.Add(assetRule);
                        break;
                    case "ratio":
                        ratioChunkAssetRules.Add(assetRule);
                        break;
                    case "random":
                        ratioChunkAssetRules.Add(assetRule);
                        break;
                }
            }

            if (countChunkAssetRules.Count > 0)
            {
                generateAssetInstancesByCount(countChunkAssetRules);
            }
            if (ratioChunkAssetRules.Count > 0)
            {
                generateAssetInstancesByRatio(ratioChunkAssetRules);
            }
            else if (randomChunkAssetRules.Count > 0)
            {
                generateAssetInstancesRandomly(randomChunkAssetRules);
            }
			_availableCells.Clear();
        }

        private void generateAssetInstancesRandomly(List<SLTChunkAssetRule> randomChunkAssetRules)
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
                    count = i == lastChunkAssetIndex ? _availableCells.Count : (int)randomWithin(minAssetCount, maxAssetCount);
                    generateAssetInstances(count, chunkAssetRule.assetId, chunkAssetRule.stateId);
                }
            }
        }

        private void generateAssetInstancesByRatio(List<SLTChunkAssetRule> ratioChunkAssetRules)
        {
            int ratioSum = 0;
            int len = ratioChunkAssetRules.Count;
            SLTChunkAssetRule assetRule;
            for (int i = 0; i < len; i++)
            {
                assetRule = ratioChunkAssetRules[i];
                ratioSum += assetRule.distributionValue;
            }
            int availableCellsNum = _availableCells.Count;
            float proportion = 0;
            int count;
            List<tempFractionObject> fractionAssets = new List<tempFractionObject>();
            if (ratioSum != 0)
            {
                for (int j = 0; j < len; j++)
                {
                    assetRule = ratioChunkAssetRules[j];

                    if (ratioSum * availableCellsNum != 0)
                        proportion = assetRule.distributionValue / ratioSum * availableCellsNum;

                    count = (int)proportion;

                    tempFractionObject fractObject = new tempFractionObject()
                    {
                        fraction = (int)(proportion - count),
                        assetRule = assetRule
                    };

                    fractionAssets.Add(fractObject);

                    generateAssetInstances(count, assetRule.assetId, assetRule.stateId);
                }

                fractionAssets.Sort(new tempFractionComparer());
                availableCellsNum = _availableCells.Count;

                for (int k = 0; k < availableCellsNum; k++)
                {
                    generateAssetInstances(1, fractionAssets[k].assetRule.assetId, fractionAssets[k].assetRule.stateId);
                }
            }
        }

        class tempFractionComparer : IComparer<tempFractionObject>
        {
            public int Compare(tempFractionObject x, tempFractionObject y)
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


        public class tempFractionObject 
        {
            public int fraction { get; set; }
            public SLTChunkAssetRule assetRule { get; set; }



           
        }

        private void generateAssetInstancesByCount(List<SLTChunkAssetRule> countChunkAssetRules)
        {
            for (int i = 0; i < countChunkAssetRules.Count; i++)
            {
                SLTChunkAssetRule assetRule = countChunkAssetRules[i];
                generateAssetInstances(assetRule.distributionValue, assetRule.assetId, assetRule.stateId);
            }
        }

        public override string ToString()
        {
            return "[Chunk] cells:" + _availableCells.Count + ", " + " chunkAssets: " + _chunkAssetRules.Count;
        }


        private void generateAssetInstances(int count, string assetId, IEnumerable<object> stateIds)
        {
            SLTAsset asset = _assetMap[assetId] as SLTAsset;

            SLTCell randCell = new SLTCell(0,0);
            int randCellIndex;

            for (int i = 0; i < count; i++)
            {
                randCellIndex = new Random().Next(0, _availableCells.Count );

                if(_availableCells.Any())
                randCell = _availableCells[randCellIndex];

				randCell.setAssetInstance(_layer.layerId, _layer.layerIndex, new SLTAssetInstance(asset.token, asset.getInstanceStates(stateIds), asset.properties));

                if(_availableCells.Any())
                _availableCells.RemoveAt(randCellIndex);
                if (_availableCells.Count == 0)
                    return;
            }
        }


        private void generateWeakAssetsInstances(List<SLTChunkAssetRule> weakChunkAssetInfos)
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
                    uint count = (uint)(i == lastChunkAssetIndex ? _availableCells.Count : randomWithin(minAssetCount, maxAssetCount));
                }
            }
        }


        private static float randomWithin(float min, float max, bool isFloat = false)
        {
            return isFloat ? new Random().Next(0, (int)(1 + (max - min))) + min : (int)(new Random().Next(0, (int)(1 + max - min))) + min;
        }
    }
}
