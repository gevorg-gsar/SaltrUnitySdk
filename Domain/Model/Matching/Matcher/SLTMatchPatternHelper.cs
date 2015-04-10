#region File Information
//-----------------------------------------------
// Company:			Plexonic  
// Project:			Gems  
// Developer:		[HGEG] - Hayk Geghamyan
// Create Date:		3/12/2015 12:08:53 PM
// ----------------------------------------------
#endregion File Information

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Saltr.UnitySdk.Domain.Model.Matching.Matcher
{
    public static class SLTMatchPatternHelper
    {
        public static List<SLTMatchPattern> MatchPatterns { get; private set; }
        
        public static void InitializePatterns()
        {
            MatchPatterns = new List<SLTMatchPattern>();

            SLTMatchPatternField[,] basePattern = new SLTMatchPatternField[1, 3] { 
            { 
                SLTMatchPatternField.Static, SLTMatchPatternField.Static, SLTMatchPatternField.Static
            }};

            SLTMatchPattern matchPattern = new SLTMatchPattern(basePattern, SLTMatchPatternType.Simetric);
            matchPattern.Token = "Pattern_Line_3";
            matchPattern.Priority = 10;
            MatchPatterns.Add(matchPattern);

            basePattern = new SLTMatchPatternField[2, 2]
            { 
                {SLTMatchPatternField.Static, SLTMatchPatternField.Static}, 
                {SLTMatchPatternField.Static, SLTMatchPatternField.Static}
            };

            matchPattern = new SLTMatchPattern(basePattern, SLTMatchPatternType.Simetric);
            matchPattern.Token = "Pattern_Square";
            matchPattern.Priority = 20;
            MatchPatterns.Add(matchPattern);
        }
    }
}