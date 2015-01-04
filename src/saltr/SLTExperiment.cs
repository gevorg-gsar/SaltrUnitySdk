using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
{
	/// <summary>
	/// The SLTExperiment class provides the currently running experiment data.
	/// It is possible to A/B test any feature included in the game AND/OR different levels, level packs.
	/// </summary>
    public class SLTExperiment
    {
		/// <summary>
		/// Represents experiment type.
		/// </summary>
		public enum Type
		{
			Feature,
			Level_Pack
		}

        private string _partition;
		private string _token;
		private Type _type;

		private IEnumerable<object> _customEvents;
		
		internal SLTExperiment(string token, string partition, string type, IEnumerable<object> customEvents)
		{
			_token = token;
			_partition = partition;
			_type = (Type)Enum.Parse(typeof(Type), type, true);
			_customEvents = customEvents;
		}

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
		/// The type of the experiment. See <see cref="saltr.SLTExperimet.Type"/>.
		/// </summary>
		public Type type
        {
            get { return _type; }
        }

		// <summary>
		// The array of comma separated event names for which A/B test data should be send.
		// </summary>
		internal IEnumerable<object> CustomEvents
		{
			get { return _customEvents; }
		}
    }
}
