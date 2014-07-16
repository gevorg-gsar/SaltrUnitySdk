using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace saltr_unity_sdk
{
   public class SLTBoard
    {
        private Dictionary<string, object> _properties;

        protected Dictionary<string, object> properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        private List<SLTBoardLayer> _layers;

		internal List<SLTBoardLayer> layers
        {
            get { return _layers; }
            set { _layers = value; }
        }


        public SLTBoard(List<SLTBoardLayer> layers, Dictionary<string, object> properties)
        {
            _properties = properties;
            _layers = layers;

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
