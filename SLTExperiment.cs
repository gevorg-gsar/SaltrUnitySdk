using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk
{
	/// <summary>
	/// The SLTExperiment class provides the currently running experiment data.
	/// It is possible to A/B test any feature included in the game AND/OR different levels, level packs.
	/// </summary>
    public class SLTExperiment
    {
        #region Fields

        private string _token;
        private string _partition;

        private SLTExperimentType _experimentType;
        private IEnumerable<object> _customEvents;

        #endregion Fields

        #region Properties


        /// <summary>
        /// The partition letter the client is assign to (<c>"A"</c>, <c>"B"</c>, <c>"C"</c>, etc.).
        /// </summary>
        public string Partition
        {
            get { return _partition; }
        }

        /// <summary>
        /// A unique identifier for the experiment.
        /// </summary>
        public string Token
        {
            get { return _token; }
        }

        /// <summary>
        /// The experimentType of the experiment. See <see cref="saltr.SLTExperimet.SLTExperimentType"/>.
        /// </summary>
        public SLTExperimentType ExperimentType
        {
            get { return _experimentType; }
        }

        // <summary>
        // The array of comma separated event names for which A/B test data should be send.
        // </summary>
        public IEnumerable<object> CustomEvents
        {
            get { return _customEvents; }
        }

        #endregion Properties

        #region Ctor

        public SLTExperiment(string token, string partition, SLTExperimentType experimentType, IEnumerable<object> customEvents)
        {
            _token = token;
            _partition = partition;
            _customEvents = customEvents;
            _experimentType = experimentType;
        }

        #endregion Ctor       

    }
    
    /// <summary>
    /// Represents the type of an experiment.
    /// </summary>
    public enum SLTExperimentType
    {
        Unknown,
        Feature,
        Level
    }
}
