using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTMatchingBoardLayer : SLTBoardLayer
    {
        #region Properties

        public List<SLTChunk> Chunks { get; set; }

        public List<SLTMatchingFixedAssetConfig> FixedAssets { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        #endregion

        public virtual void Regenerate(SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, int index)
        {
            Index = index;

            FixedAssets.ForEach(fa => GenerateFixedAsset(fa, boardCells, assetTypes, Token, Index.Value));
            Chunks.ForEach(chunk => chunk.GenerateContent(Token, Index.Value, boardCells, assetTypes));
        }

        private void GenerateFixedAsset(SLTMatchingFixedAssetConfig fixedAssetConfig, SLTCell[,] boardCells, Dictionary<string, SLTMatchingAssetType> assetTypes, string layerToken, int layerIndex)
        {
            SLTMatchingAssetType assetType = assetTypes[fixedAssetConfig.AssetId];
            foreach (var position in fixedAssetConfig.Cells)
            {
                SLTCell fixedAssetCell = boardCells[position.Last<int>(), position.First<int>()];
                SLTMatchingAsset fixedAsset = new SLTMatchingAsset() { Token = assetType.Token, State = assetType.States[fixedAssetConfig.StateId], Properties = assetType.Properties };
                fixedAssetCell.SetAsset(layerToken, layerIndex, fixedAsset);
            }
        }
    }
}