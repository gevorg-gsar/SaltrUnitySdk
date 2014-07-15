using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCell
    {
        public SLTCell(int col, int row)
        {
            _col = col;
            _row = row;
            _properties = null;
            _isBlocked = false;
            _instancesByLayerId = new Dictionary<string, object>();
            _instancesByLayerIndex = new Dictionary<string, object>();
        }

        private int _col;

        public int col
        {
            get { return _col; }
            set { _col = value; }
        }
        private int _row;

        public int row
        {
            get { return _row; }
            set { _row = value; }
        }
        private object _properties;

        public object properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
        private bool _isBlocked;

        public bool isBlocked
        {
            get { return _isBlocked; }
            set { _isBlocked = value; }
        }

        Dictionary<string, object> _instancesByLayerId;

        Dictionary<string, object> _instancesByLayerIndex;

        public SLTAssetInstance getAssetInstanceByLayerId(string layerId)
        {
            return _instancesByLayerId[layerId] as SLTAssetInstance;
        }

        public SLTAssetInstance getAssetInstanceByLayerIndex(int layerIndex)
        {
            return _instancesByLayerIndex[layerIndex.ToString()] as SLTAssetInstance;
        }

        public void setAssetInstance(string layerId, int layerIndex, SLTAssetInstance assetInstance)
        {
            if (_isBlocked == false)
            {
                _instancesByLayerId[layerId] = assetInstance;
                _instancesByLayerId[layerIndex.ToString()] = assetInstance;
            }
        }

		public void removeAssetInctance(string layerId, int layerIndex)
		{
			_instancesByLayerId.Remove(layerId);
			_instancesByLayerIndex.Remove(layerIndex);
		}

    }
}