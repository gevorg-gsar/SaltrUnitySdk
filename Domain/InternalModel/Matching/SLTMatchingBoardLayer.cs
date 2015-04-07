using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Domain.InternalModel.Matching
{
    public class SLTMatchingBoardLayer : SLTBoardLayer
    {
        #region Properties

        public List<SLTChunk> Chunks { get; set; }

        public List<SLTMatchingFixedAssetConfig> FixedAssets { get; set; }

        public bool? MatchingRulesEnabled { get; set; }

        #endregion

       
    }
}