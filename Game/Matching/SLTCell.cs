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

        private int _col;
        private int _row;
        private Dictionary<string, object> _properties;		
        private bool _isBlocked;

		Dictionary<string, object> _instancesByLayerId;
		Dictionary<string, object> _instancesByLayerIndex;

		internal SLTCell(int col, int row)
		{
			_col = col;
			_row = row;
			_properties = null;
			_isBlocked = false;
			_instancesByLayerId = new Dictionary<string, object>();
			_instancesByLayerIndex = new Dictionary<string, object>();
		}

		/// <summary>
		/// Gets or sets the column of the cell.
		/// </summary>
		public int Col
		{
			get { return _col; }
			set { _col = value; }
		}

		/// <summary>
		/// Gets or sets the row of the cell.
		/// </summary>
		public int Row
		{
			get { return _row; }
			set { _row = value; }
		}

		/// <summary>
		/// Gets or sets the properties of the cell.
		/// </summary>
		public Dictionary<string,object> Properties
		{
			get { return _properties; }
			set { _properties = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="saltr.Game.Matching.SLTCell"/> is blocked.
		/// </summary>
		/// <value><c>true</c> if is blocked; otherwise, <c>false</c>.</value>
        public bool IsBlocked
        {
            get { return _isBlocked; }
            set { _isBlocked = value; }
        }

		/// <summary>
		/// Gets the asset instance by layer identifier(token).
		/// </summary>
		/// <returns>The asset instance that is positioned in the cell in the layer specified by layerId.</returns>
		/// <param name="layerId">Layer identifier(token).</param>
        public SLTAssetInstance GetAssetInstanceByLayerId(string layerId)
        {
            return _instancesByLayerId.GetValue(layerId) as SLTAssetInstance;
        }

		/// <summary>
		/// Gets the asset instance by layer index.
		/// </summary>
		/// <returns>The asset instance that is positioned in the cell in the layer specified by layerIndex.</returns>
		/// <param name="layerIndex">Layer index.</param>
        public SLTAssetInstance GetAssetInstanceByLayerIndex(int layerIndex)
        {
            return _instancesByLayerIndex.GetValue(layerIndex.ToString()) as SLTAssetInstance;
        }
		
        internal void SetAssetInstance(string layerId, int layerIndex, SLTAssetInstance assetInstance) 
        {
            if (_isBlocked == false)
            {
                _instancesByLayerId[layerId] = assetInstance;
				_instancesByLayerIndex[layerIndex.ToString()] = assetInstance;
            }
        }
		
		internal void RemoveAssetInstance(string layerId, int layerIndex) 
		{
			_instancesByLayerId.Remove(layerId);
			_instancesByLayerIndex.Remove(layerIndex.ToString());

		}

    }
}