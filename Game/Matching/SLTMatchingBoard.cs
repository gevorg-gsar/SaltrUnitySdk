using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTMatchingBoard : SLTBoard
    {
        public List<SLTMatchingBoardLayer> Layers { get; set; }

        public SLTCell[,] Cells { get; private set; }

        public int? Rows { get; set; }

        public int? Cols { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        public List<int> CellSize { get; set; }

        public SLTMatchingBoardOrientation? Orientation { get; set; }

        public List<CellProperty> CellProperties { get; set; }

        public List<List<int>> BlockedCells { get; set; }

        public SLTCell[,] Regenerate(Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            if (Cells == null && Rows.HasValue && Rows.Value > 0
                && Cols.HasValue && Cols.Value > 0)
            {
                Cells = new SLTCell[Rows.Value, Cols.Value];

                for (int row = 0; row < Cells.GetLength(0); row++)
                {
                    for (int col = 0; col < Cells.GetLength(1); col++)
                    {
                        Cells[row, col] = new SLTCell(row, col);
                    }
                }
            }
            int index = 0;
            Layers.ForEach(layer => layer.Regenerate(Cells, assetTypes, index++));

            return Cells;
        }

    }

    public enum SLTMatchingBoardOrientation
    {
        TOP_LEFT,
        BOTTOM_LEFT
    }
}