using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game.Matching
{
    public class SLTBoardGenerator
    {
        public static void RegenerateBoards(Dictionary<string, SLTMatchingBoard> Boards, Dictionary<string, SLTMatchingAssetType> assets)
        {
            if (Boards != null)
            {
                foreach (var item in Boards)
                {
                    item.Value.Regenerate(assets);
                }
            }
        }
    }
}