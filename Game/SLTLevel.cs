using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Utils;
using Newtonsoft.Json;

namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// Represents a level - a uniquely identifiable collection of boards and user defined properties.
    /// </summary>
    public class SLTLevel
    {
        #region Properties

        public int Id { get; set; }

        /// <summary>
        /// Gets the index of the level in all levels.
        /// </summary>
        public int? Index { get; set; }

        public int? PackIndex { get; set; }

        /// <summary>
        /// Gets the index of the level within its pack.
        /// </summary>
        public int? LocalIndex { get; set; }

        public int? Version { get; set; }

        public int? VariationId { get; set; }

        public int? VariationVersion { get; set; }

        /// <summary>
        /// Gets the URL, used to retrieve level content from Saltr.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets the properties of the level.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        public SLTLevelContent Content { get; set; }

        #endregion Properties

    }

    
}