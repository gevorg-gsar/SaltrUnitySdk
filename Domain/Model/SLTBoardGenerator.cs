using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Domain.InternalModel;
using Saltr.UnitySdk.Domain.InternalModel.Canvas;
using Saltr.UnitySdk.Domain.InternalModel.Matching;
using Saltr.UnitySdk.Domain.Model.Canvas;
using Saltr.UnitySdk.Domain.Model.Matching;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.Model
{
    public static class SLTBoardGenerator
    {
        public static SLTLevelContent RegenerateBoard(this SLTInternalLevelContent internalLevelContent, string token)
        {
            if (string.IsNullOrEmpty(token) || internalLevelContent.Boards == null || !internalLevelContent.Boards.ContainsKey(token))
            {
                return null;
            }            

            Dictionary<string, SLTInternalBoard> internalBoards = new Dictionary<string,SLTInternalBoard>();
            internalBoards.Add(token, internalLevelContent.Boards[token]);
            
            SLTLevelContent levelContent = RegenerateBoards(internalBoards, internalLevelContent.Assets, BoardConverter.LevelType);
            levelContent.Properties = internalLevelContent.Properties;

            return levelContent;
        }

        public static SLTLevelContent RegenerateBoards(this SLTInternalLevelContent internalLevelContent)
        {
            SLTLevelContent levelContent = RegenerateBoards(internalLevelContent.Boards, internalLevelContent.Assets, BoardConverter.LevelType);
            levelContent.Properties = internalLevelContent.Properties;

            return levelContent;
        }

        public static SLTLevelContent RegenerateBoards(Dictionary<string, SLTInternalBoard> boards, Dictionary<string, SLTAssetType> assetTypes, SLTLevelType levelType)
        {
            SLTLevelContent levelContent = new SLTLevelContent();

            if (levelType == SLTLevelType.Matching)
            {
                Dictionary<string, SLTInternalMatchingBoard> matchingBoards = new Dictionary<string, SLTInternalMatchingBoard>();
                foreach (var item in boards)
                {
                    matchingBoards.Add(item.Key, item.Value as SLTInternalMatchingBoard);
                }

                Dictionary<string, SLTMatchingAssetType> matchingAssets = new Dictionary<string, SLTMatchingAssetType>();
                foreach (var item in assetTypes)
                {
                    matchingAssets.Add(item.Key, item.Value as SLTMatchingAssetType);
                }

                levelContent.MatchingBoards = RegenerateBoards(matchingBoards, matchingAssets); 
            }
            else if (levelType == SLTLevelType.Canvas2D)
            {
                Dictionary<string, SLTInternalCanvasBoard> canvasBoards = new Dictionary<string, SLTInternalCanvasBoard>();
                foreach (var item in boards)
                {
                    canvasBoards.Add(item.Key, item.Value as SLTInternalCanvasBoard);
                }

                Dictionary<string, SLTCanvasAssetType> canvasAssets = new Dictionary<string, SLTCanvasAssetType>();
                foreach (var item in assetTypes)
                {
                    canvasAssets.Add(item.Key, item.Value as SLTCanvasAssetType);
                }

                levelContent.CanvasBoards = RegenerateBoards(canvasBoards, canvasAssets);
            }

            return levelContent;
        }

        public static Dictionary<string, SLTMatchingBoard> RegenerateBoards(Dictionary<string, SLTInternalMatchingBoard> internalBoards, Dictionary<string, SLTMatchingAssetType> assetTypes)
        {
            if (internalBoards != null)
            {
                Dictionary<string, SLTMatchingBoard> boards = new Dictionary<string, SLTMatchingBoard>();

                foreach (var item in internalBoards)
                {
                    boards.Add(item.Key, item.Value.RegenerateBoard(assetTypes));
                }

                return boards;
            }

            return null;
        }

        public static Dictionary<string, SLTCanvasBoard> RegenerateBoards(Dictionary<string, SLTInternalCanvasBoard> internalBoards, Dictionary<string, SLTCanvasAssetType> assetTypes)
        {
            if (internalBoards != null)
            {
                Dictionary<string, SLTCanvasBoard> boards = new Dictionary<string, SLTCanvasBoard>();

                foreach (var item in internalBoards)
                {
                    boards.Add(item.Key, item.Value.RegenerateBoard(assetTypes));
                }

                return boards;
            }

            return null;
        }

    }
}