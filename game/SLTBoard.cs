using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace saltr_unity_sdk
{
   public class SLTBoard
    {
        private Dictionary<string, object> _properties;
        private List<SLTBoardLayer> _layers;
		
        public SLTBoard(List<SLTBoardLayer> layers, Dictionary<string, object> properties)
        {
            _properties = properties;
            _layers = layers;

        }

		protected Dictionary<string, object> properties
		{
			get { return _properties; }
		}

		internal List<SLTBoardLayer> layers
		{
			get { return _layers; }
		}

        public void regenerate()
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                SLTBoardLayer layer = _layers[i] as SLTBoardLayer;
                layer.regenerate();
            }
        }
    }
}
