using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevelBoard
    {
        private int _rows;
        public int rows
        {
            get { return _rows; }
        }

        private int _cols;
        public int cols
        {
            get { return _cols; }
        }

        private SLTCells _cells;
        public SLTCells cells
        {
            get { return _cells; }
        }

        private Dictionary<string, object> _properties;
        public Dictionary<string, object> properties
        {
            get { return _properties; }
        }

        private Dictionary<string, object> _layers;
        public Dictionary<string, object> layers
        {
            get { return _layers; }
        }

        public SLTLevelBoard(SLTCells cells, Dictionary<string, object> layers, Dictionary<string, object> properties)
        {
            _cells = cells;
            _cols = cells.width;
            _rows = cells.height;
            _properties = properties;
            _layers = layers;
        }
    }
}
