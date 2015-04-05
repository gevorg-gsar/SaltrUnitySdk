using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Saltr.UnitySdk.Domain.Game.Matching
{
    public class SLTChunkAssetConfig : SLTAssetConfig
    {
        #region Properties

        public int? DistributionValue { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChunkAssetDistributionType? DistributionType { get; set; }

        #endregion Properties
    }

    public enum ChunkAssetDistributionType
    {
        Count,
        Ratio,
        Random
    }
}
