using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
{
    /// <summary>
    /// Represents a matching board cell.
    /// </summary>
    public class SLTCell
    {
        #region Firlds

        private Dictionary<string, SLTAsset> _assetsByLayerToken;
        private Dictionary<string, SLTAsset> _assetsByLayerIndex;

        #endregion Firlds
        
        #region Properties

        /// <summary>
        /// Gets or sets the column of the cell.
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// Gets or sets the row of the cell.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the properties of the cell.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="saltr.Game.Matching.SLTCell"/> is blocked.
        /// </summary>
        /// <value><c>true</c> if is blocked; otherwise, <c>false</c>.</value>
        public bool IsBlocked { get; set; }

        #endregion Properties

        #region Ctor

        public SLTCell(int row, int col)
        {
            Row = row;
            Col = col;

            _assetsByLayerToken = new Dictionary<string, SLTAsset>();
            _assetsByLayerIndex = new Dictionary<string, SLTAsset>();
        }

        #endregion Ctor

        #region Business Methods

        public void SetAsset(string layerToken, int layerIndex, SLTAsset assetInstance)
        {
            if (IsBlocked == false)
            {
                _assetsByLayerToken[layerToken] = assetInstance;
                _assetsByLayerIndex[layerIndex.ToString()] = assetInstance;
            }
        }

        public void RemoveAsset(string layerToken, int layerIndex)
        {
            _assetsByLayerToken.Remove(layerToken);
            _assetsByLayerIndex.Remove(layerIndex.ToString());

        }

        /// <summary>
        /// Gets the asset instance by layer identifier(token).
        /// </summary>
        /// <returns>The asset instance that is positioned in the cell in the layer specified by layerId.</returns>
        /// <param name="layerToken">Layer identifier(token).</param>
        public SLTAsset GetAssetByLayerToken(string layerToken)
        {
            if (!_assetsByLayerToken.ContainsKey(layerToken))
            {
                return null;
            }

            return _assetsByLayerToken[layerToken];            
        }

        /// <summary>
        /// Gets the asset instance by layer index.
        /// </summary>
        /// <returns>The asset instance that is positioned in the cell in the layer specified by layerIndex.</returns>
        /// <param name="layerIndex">Layer index.</param>
        public SLTAsset GetAssetByLayerIndex(int layerIndex)
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