﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Saltr.UnitySdk.Domain;

namespace Saltr.UnitySdk.Domain.Model
{
    public class SLTAppData : SLTBaseEntity
    {
        #region Properties

        [JsonConverter(typeof(StringEnumConverter))]
        public SLTLevelType? LevelType { get; set; }

        public List<SLTFeature> Features { get; set; }

        public List<SLTLevelPack> LevelPacks { get; set; }

        public List<SLTExperiment> Experiments { get; set; }

        #endregion Properties

        #region Ctor

        public SLTAppData() { }

        #endregion Ctor
    }

    public enum SLTLevelType
    {
        /// <summary>
        /// Used for parsing data retrieved from saltr.
        /// </summary>
        NoLevels,
        /// <summary>
        /// A level with "matching" boards and assets.
        /// </summary>
        Matching,
        /// <summary>
        /// A level with 2D boards and assets.
        /// </summary>
        Canvas2D
    }
}
