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
        #region Properties

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int Cols { get; set; }

        /// <summary>
        /// Gets the cells.
        /// </summary>
        public SLTCells Cells { get; set; }

        #endregion Properties
    }
}
