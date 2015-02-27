using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class LevelSortByIndexComparer : IComparer<SLTLevel>
    {
        public int Compare(SLTLevel firstLevel, SLTLevel secondLevel)
        {
            if (firstLevel == null && secondLevel != null)
                return -1;

            if (firstLevel != null && secondLevel == null)
                return 1;

            if (firstLevel == null && secondLevel == null)
                return 1;

            if (firstLevel.Index > secondLevel.Index)
                return 1;

            if (firstLevel.Index < secondLevel.Index)
                return -1;

            if (firstLevel.Index == secondLevel.Index)
                return 0;

            return 1;
        }
    }

}
