using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain
{
	/// <summary>
	/// The SLTExperiment class provides the currently running experiment data.
	/// It is possible to A/B test any feature included in the game AND/OR different levels, level packs.
	/// </summary>
    public class SLTExperiment
    {
        #region Properties

        /// <summary>
        /// A unique identifier for the experiment.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The partition letter the client is assign to (<c>"A"</c>, <c>"B"</c>, <c>"C"</c>, etc.).
        /// </summary>
        public string Partition { get; set; }

        /// <summary>
        /// The experimentType of the experiment. See <see cref="saltr.SLTExperimet.SLTExperimentType"/>.
        /// </summary>
        public SLTExperimentType? ExperimentType { get; set; }

        // <summary>
        // The array of comma separated event names for which A/B test data should be send.
        // </summary>
        public IEnumerable<object> CustomEvents { get; set; }

        #endregion Properties
    }
    
    /// <summary>
    /// Represents the type of an experiment.
    /// </summary>
    public enum SLTExperimentType
    {
        Feature,
        Level
    }
}
