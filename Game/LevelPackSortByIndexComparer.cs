using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    public class LevelPackSortByIndexComparer : IComparer<SLTLevelPack>
    {
        public int Compare(SLTLevelPack firstLevelPack, SLTLevelPack secondLevelPack)
        {
            if (firstLevelPack == null && secondLevelPack != null)
                return -1;

            if (firstLevelPack != null && secondLevelPack == null)
                return 1;

            if (firstLevelPack == null && secondLevelPack == null)
                return 1;

            if (firstLevelPack.Index > secondLevelPack.Index)
                return 1;

            if (firstLevelPack.Index < secondLevelPack.Index)
                return -1;
            return 1;
        }
    }

}
