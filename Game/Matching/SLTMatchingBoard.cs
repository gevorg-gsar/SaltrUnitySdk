using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game.Matching
{
	/// <summary>
	/// Represents a matching board.
	/// </summary>
    public class SLTMatchingBoard : SLTBoard
    {

        private int _rows;
        
		/// <summary>
        /// Gets the number of rows.
        /// </summary>
		public int Rows
        {
            get { return _rows; }
        }

        private int _cols;

		/// <summary>
		/// Gets the number of columns.
		/// </summary>
        public int Cols
        {
            get { return _cols; }
        }

        private SLTCells _cells;

		/// <summary>
		/// Gets the cells.
		/// </summary>
        public SLTCells Cells
        {
            get { return _cells; }
        }


        internal SLTMatchingBoard(SLTCells cells, List<SLTBoardLayer> layers, Dictionary<string, object> properties)
            : base(layers, properties)
        {
            _cells = cells;
            _cols = cells.Width;
            _rows = cells.Height;
        }

    }
}
