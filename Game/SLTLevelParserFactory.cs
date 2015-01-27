using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Game.Canvas2D;
using Saltr.UnitySdk.Game.Matching;

namespace Saltr.UnitySdk.Game
{
    public class SLTLevelParserFactory
    {
        public static SLTLevelParser GetParser(SLTLevelType levelType)
        {
            if (levelType == SLTLevelType.Matching)
            {
                return SLTMatchingLevelParser.Instance;
            }
            else if (levelType == SLTLevelType.Canvas2D)
            {
                return SLT2DLevelParser.Instance;
            }

            return null;
        }
    }
}
