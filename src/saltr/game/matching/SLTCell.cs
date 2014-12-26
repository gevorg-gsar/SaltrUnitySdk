using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saltr.utils;

namespace saltr.game.matching
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
		public int col
		{
			get { return _col; }
			set { _col = value; }
		}

		/// <summary>
		/// Gets or sets the row of the cell.
		/// </summary>
		public int row
		{
			get { return _row; }
			set { _row = value; }
		}

		/// <summary>
		/// Gets or sets the properties of the cell.
		/// </summary>
		public Dictionary<string,object> properties
		{
			get { return _properties; }
			set { _properties = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="saltr.game.matching.SLTCell"/> is blocked.
		/// </summary>
		/// <value><c>true</c> if is blocked; otherwise, <c>false</c>.</value>
        public bool isBlocked
        {
            get { return _isBlocked; }
            set { _isBlocked = value; }
        }

		/// <summary>
		/// Gets the asset instance by layer identifier(token).
		/// </summary>
		/// <returns>The asset instance that is positioned in the cell in the layer specified by layerId.</returns>
		/// <param name="layerId">Layer identifier(token).</param>
        public SLTAssetInstance getAssetInstanceByLayerId(string layerId)
        {
            return _instancesByLayerId.getValue(layerId) as SLTAssetInstance;
        }

		/// <summary>
		/// Gets the asset instance by layer index.
		/// </summary>
		/// <returns>The asset instance that is positioned in the cell in the layer specified by layerIndex.</returns>
		/// <param name="layerIndex">Layer index.</param>
        public SLTAssetInstance getAssetInstanceByLayerIndex(int layerIndex)
        {
            return _instancesByLayerIndex.getValue(layerIndex.ToString()) as SLTAssetInstance;
        }
		
        internal void setAssetInstance(string layerId, int layerIndex, SLTAssetInstance assetInstance) 
        {
            if (_isBlocked == false)
            {
                _instancesByLayerId[layerId] = assetInstance;
				_instancesByLayerIndex[layerIndex.ToString()] = assetInstance;
            }
        }
		
		internal void removeAssetInstance(string layerId, int layerIndex) 
		{
			_instancesByLayerId.Remove(layerId);
			_instancesByLayerIndex.Remove(layerIndex.ToString());

		}

    }
}