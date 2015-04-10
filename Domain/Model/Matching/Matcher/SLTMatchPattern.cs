using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Saltr.UnitySdk.Domain.Model.Matching.Matcher
{
    public class SLTMatchPattern
    {
        #region Properties

        public string Token { get; set; }

        public int Priority { get; set; }

        public List<SLTMatchPatternField[,]> Patterns { get; private set; }

        #endregion Properties

        #region Ctor

        public SLTMatchPattern(SLTMatchPatternField[,] basePattern, SLTMatchPatternType matchPatternType)
        {
            Patterns = new List<SLTMatchPatternField[,]>();
            Patterns.Add(basePattern);

            SLTMatchPatternField[,] rotatedPattern = basePattern;

            if (matchPatternType == SLTMatchPatternType.Permanent)
            {
                return;
            }
            else if (matchPatternType == SLTMatchPatternType.Simetric)
            {
                rotatedPattern = RotateRight(rotatedPattern);
                Patterns.Add(rotatedPattern);
            }
            else if (matchPatternType == SLTMatchPatternType.Asimetric)
            {
                for (int i = 0; i < 3; i++)
                {
                    rotatedPattern = RotateRight(rotatedPattern);
                    Patterns.Add(rotatedPattern);
                }
            }
        }

        #endregion Ctor

        private SLTMatchPatternField[,] RotateRight(SLTMatchPatternField[,] basePattern)
        {
            int rowCount = basePattern.GetLength(0);
            int colCount = basePattern.GetLength(1);

            SLTMatchPatternField[,] rotatedPattern = new SLTMatchPatternField[colCount, rowCount];
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    SLTMatchPatternField currentField = basePattern[row, col];
                    rotatedPattern[col, rowCount - row - 1] = RotateField(currentField);
                }
            }
            return rotatedPattern;
        }

        private SLTMatchPatternField RotateField(SLTMatchPatternField currentField)
        {
            if (IsExtendableField(currentField))
            {
                switch (currentField)
                {
                    case SLTMatchPatternField.Up:
                        return SLTMatchPatternField.Right;

                    case SLTMatchPatternField.Right:
                        return SLTMatchPatternField.Down;

                    case SLTMatchPatternField.Down:
                        return SLTMatchPatternField.Left;

                    case SLTMatchPatternField.Left:
                        return SLTMatchPatternField.Up;
                }
            }
            return currentField;
        }

        public static bool IsExtendableField(SLTMatchPatternField patternField)
        {
            return patternField == SLTMatchPatternField.Up ||
                   patternField == SLTMatchPatternField.Right ||
                   patternField == SLTMatchPatternField.Down ||
                   patternField == SLTMatchPatternField.Left;
        }
    }

    public enum SLTMatchPatternField
    {
        None = 0,
        Static,
        Up,
        Right,
        Down,
        Left,
        Random
    }

    public enum SLTMatchPatternType
    {
        Asimetric,
        Simetric,
        Permanent
    }
}