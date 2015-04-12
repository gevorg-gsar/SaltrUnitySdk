#region File Information
//-----------------------------------------------
// Company:			Plexonic  
// Project:			Gems  
// Developer:		[HGEG] - Hayk Geghamyan
// Create Date:		3/12/2015 11:56:50 AM
// ----------------------------------------------
#endregion File Information

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Saltr.UnitySdk.Domain.Model.Matching.Matcher
{
    public static class SLTMatchManager
    {
        #region Public Methods

        public static Dictionary<SLTMatchPattern, List<List<SLTCell>>> GetMatchGroups(SLTCell[,] cells, string layerToken, int layerIndex, bool squareMatchingRuleEnabled)
        {
            SLTMatchPatternHelper.InitializePatterns(squareMatchingRuleEnabled);
            SLTMatchPatternHelper.MatchPatterns.OrderBy(p => p.Priority).ToList();

            Dictionary<SLTMatchPattern, List<List<SLTCell>>>  matchCells = new Dictionary<SLTMatchPattern, List<List<SLTCell>>>();
            foreach (SLTMatchPattern matchPattern in SLTMatchPatternHelper.MatchPatterns)
            {
                for (int row = 0; row < cells.GetLength(0); row++)
                {
                    for (int col = 0; col < cells.GetLength(1); col++)
                    {
                        SLTCell cell = cells[row, col];

                        if (!IsInAllMatchCells(matchCells, cell))
                        {
                            foreach (SLTMatchPatternField[,] pattern in matchPattern.Patterns)
                            {
                                List<SLTCell> matchCellsAtCurrentPosition = GetMatchAtPosititon(row, col, pattern, cells, matchCells, layerToken, layerIndex);
                                if (matchCellsAtCurrentPosition != null && matchCellsAtCurrentPosition.Count > 0)
                                {
                                    if (matchCells.ContainsKey(matchPattern))
                                    {
                                        matchCells[matchPattern].Add(matchCellsAtCurrentPosition);
                                    }
                                    else
                                    {
                                        List<List<SLTCell>> patternMatchCells = new List<List<SLTCell>>();
                                        patternMatchCells.Add(matchCellsAtCurrentPosition);
                                        matchCells.Add(matchPattern, patternMatchCells);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return matchCells;
        }

        #endregion Public Methods

        #region Private Methods

        private static List<SLTCell> GetMatchAtPosititon(int row, int col, SLTMatchPatternField[,] pattern, SLTCell[,] cells, Dictionary<SLTMatchPattern, List<List<SLTCell>>>  matchCells, string layerToken, int layerIndex)
        {
            List<SLTCell> matchCellsAtCurrentPosition = new List<SLTCell>();
            int matchPatternRowCount = pattern.GetLength(0);
            int matchPatternColCount = pattern.GetLength(1);

            int patternRightBound = col + matchPatternColCount - 1;
            int patternBottomBound = row + matchPatternRowCount - 1;

            bool isPatternInMatrixBounds = IsInCellsBounds(cells, patternBottomBound, patternRightBound);

            if (isPatternInMatrixBounds)
            {
                SLTCell currentCell = cells[row, col];
                SLTAsset currentAsset = null;
                Dictionary<SLTCell, SLTMatchPatternField> extendableCellsDict = new Dictionary<SLTCell, SLTMatchPatternField>();

                for (int i = 0; i < matchPatternRowCount; i++)
                {
                    for (int j = 0; j < matchPatternColCount; j++)
                    {
                        SLTMatchPatternField patternField = pattern[i, j];
                        if (patternField != SLTMatchPatternField.None)
                        {
                            SLTCell patternCurrentCell = cells[row + i, col + j];

                            if (patternCurrentCell == null
                                || patternCurrentCell.GetAssetByLayerToken(layerToken) == null
                                || IsInAllMatchCells(matchCells, patternCurrentCell))
                            {
                                return null;
                            }

                            if (currentAsset == null)
                            {
                                currentAsset = patternCurrentCell.GetAssetByLayerToken(layerToken);
                            }

                            if (!IsSameAssetMatch(patternCurrentCell.GetAssetByLayerToken(layerToken), currentAsset))
                            {
                                return null;
                            }

                            if (SLTMatchPattern.IsExtendableField(patternField))
                            {
                                extendableCellsDict.Add(patternCurrentCell, patternField);
                            }

                            matchCellsAtCurrentPosition.Add(patternCurrentCell);
                        }
                    }
                }
                matchCellsAtCurrentPosition.AddRange(GetExtendedMatchCells(extendableCellsDict, currentAsset, cells, matchCells, layerToken, layerIndex));
            }
            return matchCellsAtCurrentPosition;
        }

        private static List<SLTCell> GetExtendedMatchCells(Dictionary<SLTCell, SLTMatchPatternField> extendableCellsDict, SLTAsset currentAsset, SLTCell[,] cells, Dictionary<SLTMatchPattern, List<List<SLTCell>>>  matchCells, string layerToken, int layerIndex)
        {
            List<SLTCell> extendedMatchCells = new List<SLTCell>();
            foreach (SLTCell extendableCell in extendableCellsDict.Keys)
            {
                int horizontalDirection = 0;
                int verticalDirection = 0;

                SLTMatchPatternField field = extendableCellsDict[extendableCell];
                switch (field)
                {
                    case SLTMatchPatternField.Up:
                        verticalDirection = -1;
                        break;
                    case SLTMatchPatternField.Right:
                        horizontalDirection = 1;
                        break;
                    case SLTMatchPatternField.Down:
                        verticalDirection = 1;
                        break;
                    case SLTMatchPatternField.Left:
                        horizontalDirection = -1;
                        break;
                }

                int extendedCellRow = extendableCell.Row + verticalDirection;
                int extendedCellCol = extendableCell.Col + horizontalDirection;

                while (IsInCellsBounds(cells, extendedCellRow, extendedCellCol))
                {
                    SLTCell cell = cells[extendedCellRow, extendedCellCol];

                    bool isMatchedFigure = cell.GetAssetByLayerToken(layerToken) != null && IsSameAssetMatch(cell.GetAssetByLayerToken(layerToken), currentAsset);

                    if (isMatchedFigure && !IsInAllMatchCells(matchCells, cell))
                    {
                        extendedMatchCells.Add(cell);
                    }
                    else
                    {
                        break;
                    }

                    extendedCellRow += verticalDirection;
                    extendedCellCol += horizontalDirection;
                }
            }
            return extendedMatchCells;
        }

        public static bool IsInAllMatchCells(Dictionary<SLTMatchPattern, List<List<SLTCell>>> matchCells, SLTCell cell)
        {
            //@TODO: Gor try to implement with SelectMany
            foreach (List<List<SLTCell>> patternMatchCells in matchCells.Values)
            {
                if (IsInMatchCells(patternMatchCells, cell))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInMatchCells(List<List<SLTCell>> patternMatchCells, SLTCell cell)
        {
            foreach (List<SLTCell> patternMatchGroup in patternMatchCells)
            {
                if (patternMatchGroup.Contains(cell))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSameAssetMatch(SLTAsset asset1, SLTAsset asset2)
        {
            if (asset1 == null || asset2 == null)
            {
                return false;
            }

            return asset1.Token == asset2.Token;
        }

        private static bool IsInCellsBounds(SLTCell[,] cells, int row, int col)
        {
            return col < cells.GetLength(1) && col >= 0 && row < cells.GetLength(0) && row >= 0;
        }

        #endregion Private Methods

    }
}