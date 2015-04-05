using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Model.Matching
{
    /// <summary>
    /// Represents a matching board cell.
    /// </summary>
    public class SLTCell
    {
        #region Filds

        private Dictionary<string, SLTMatchingAsset> _assetsByLayerToken;
        private Dictionary<string, SLTMatchingAsset> _assetsByLayerIndex;

        #endregion Filds
        
        #region Properties

        public bool IsBlocked { get; set; }

        public int Col { get; set; }

        public int Row { get; set; }

        public Dictionary<string, object> Properties { get; set; }


        #endregion Properties

        #region Ctor

        public SLTCell(int row, int col)
        {
            Row = row;
            Col = col;

            _assetsByLayerToken = new Dictionary<string, SLTMatchingAsset>();
            _assetsByLayerIndex = new Dictionary<string, SLTMatchingAsset>();
        }

        #endregion Ctor

        #region Business Methods

        public void SetAsset(string layerToken, int layerIndex, SLTMatchingAsset asset)
        {
            if (!IsBlocked)
            {
                _assetsByLayerToken[layerToken] = asset;
                _assetsByLayerIndex[layerIndex.ToString()] = asset;
            }
        }

        public void RemoveAsset(string layerToken, int layerIndex)
        {
            _assetsByLayerToken.Remove(layerToken);
            _assetsByLayerIndex.Remove(layerIndex.ToString());

        }

        public SLTMatchingAsset GetAssetByLayerToken(string layerToken)
        {
            if (!_assetsByLayerToken.ContainsKey(layerToken))
            {
                return null;
            }

            return _assetsByLayerToken[layerToken];            
        }

        public SLTMatchingAsset GetAssetByLayerIndex(int layerIndex)
        {
            if (!_assetsByLayerIndex.ContainsKey(layerIndex.ToString()))
            {
                return null;
            }

            return _assetsByLayerIndex[layerIndex.ToString()];
        }

        #endregion Business Methods

    }
}