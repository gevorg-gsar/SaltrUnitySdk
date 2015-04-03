using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Game.Canvas;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Game
{
    public class SLTBoardGenerator
    {
        public static void RegenerateBoards(Dictionary<string, SLTBoard> boards, Dictionary<string, SLTAssetType> assetTypes, SLTLevelType levelType)
        {
            if (levelType == SLTLevelType.Matching)
            {
                Dictionary<string, SLTMatchingBoard> matchingBoards = new Dictionary<string, SLTMatchingBoard>();
                foreach (var item in boards)
                {
                    matchingBoards.Add(item.Key, item.Value as SLTMatchingBoard);
                }

                Dictionary<string, SLTMatchingAssetType> matchingAssets = new Dictionary<string, SLTMatchingAssetType>();
                foreach (var item in assetTypes)
                {
                    matchingAssets.Add(item.Key, item.Value as SLTMatchingAssetType);
                }

                RegenerateBoards(matchingBoards, matchingAssets);
            }
            else if (levelType == SLTLevelType.Canvas2D)
            {
                Dictionary<string, SLTMatchingBoard> matchingBoards = new Dictionary<string, SLTMatchingBoard>();
                foreach (var item in boards)
                {
                    matchingBoards.Add(item.Key, item.Value as SLTMatchingBoard);
                }

                Dictionary<string, SLTMatchingAssetType> matchingAssets = new Dictionary<string, SLTMatchingAssetType>();
                foreach (var item in assetTypes)
                {
                    matchingAssets.Add(item.Key, item.Value as SLTMatchingAssetType);
                }

                RegenerateBoards(matchingBoards, matchingAssets);
            }
        }

        public static void RegenerateBoards(Dictionary<string, SLTMatchingBoard> boards, Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            if (boards != null)
            {
                foreach (var item in boards)
                {
                    item.Value.Regenerate(assetTypes);
                }
            }
        }

        public static void RegenerateBoards(Dictionary<string, SLTCanvasBoard> boards, Dictionary<string, SLTCanvasAssetType> assetTypes)
        {
            if (boards != null)
            {
                foreach (var item in boards)
                {
                    item.Value.Regenerate(assetTypes);
                }
            }
        }
    }
}