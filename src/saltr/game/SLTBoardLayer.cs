using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTBoardLayer
    {
        private string _layerId;
        private int _layerIndex;

        public SLTBoardLayer(string LayerId, int LayerIndex)
        {
            _layerId = LayerId;
            _layerIndex = LayerIndex;
        }

		
		public string layerId
		{
			get { return _layerId; }
		}
		
		public int layerIndex
		{
			get { return _layerIndex; }
		}

		public virtual void regenerate()
		{
			//override
		}
    }
}
