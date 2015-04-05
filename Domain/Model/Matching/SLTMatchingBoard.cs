using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Model.Matching
{
    public class SLTMatchingBoard
    {
        public string Token { get; set; }

        public int Rows { get; set; }

        public int Cols { get; set; }

        public SLTCell[,] Cells { get; set; }
    }
}
