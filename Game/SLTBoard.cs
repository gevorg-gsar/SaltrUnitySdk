using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Game.Matching;


namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// The SLTBoard class represents a game board. 
    /// </summary>
    public class SLTBoard
    {
        #region Properties

        #region Matching

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int? Rows { get; set; }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int? Cols { get; set; }

        public SLTCell[,] _cells;

        #endregion Matching
        
        /// <summary>
        /// Gets the width of the board in pixels as is in Saltr level editor.
        /// </summary>
        public float? Width { get; set; }

        /// <summary>
        /// Gets the height of the board in pixels as is in Saltr level editor.
        /// </summary>
        public float? Height { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        public List<int> Position { get; set; }

        public List<int> CellSize { get; set; }

        public SLTBoardOrientation? Orientation { get; set; }

        public List<CellProperty> CellProperties { get; set; }

        public List<List<int>> BlockedCells { get; set; }

        /// <summary>
        /// Gets the properties associated with the board.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets the layers of the board.
        /// </summary>
        public List<SLTBoardLayer> Layers { get; set; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Regenerates contents of all layers.
        /// </summary>
        public SLTCell[,] Regenerate()
        {
            if (_cells == null && Rows.HasValue && Rows.Value > 0
                && Cols.HasValue && Cols.Value > 0)
            {
                _cells = new SLTCell[Rows.Value, Cols.Value];

                for (int row = 0; row < _cells.GetLength(0); row++)
                {
                    for (int col = 0; col < _cells.GetLength(1); col++)
                    {
                        _cells[row, col] = new SLTCell(row, col);
                    }
                }
            }

            Layers.ForEach(layer => layer.Regenerate(_cells));

            return _cells;
        }

        #endregion Business Methods

    }

    public enum SLTBoardOrientation
    {
        TOP_LEFT,
        BOTTOM_LEFT
    }
}
