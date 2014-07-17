using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTMatchingBoard : SLTBoard
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


        public SLTMatchingBoard(SLTCells cells, List<SLTBoardLayer> layers, Dictionary<string, object> properties)
            : base(layers, properties)
        {
            _cells = cells;
            _cols = cells.width;
            _rows = cells.height;
        }

    }
}
