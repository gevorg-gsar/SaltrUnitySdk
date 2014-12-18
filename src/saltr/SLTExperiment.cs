using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
{
	/// <summary>
	/// Represents an experiment.
	/// </summary>
    public class SLTExperiment
    {
		/// <summary>
		/// Represents experiment type.
		/// </summary>
		public enum Type
		{
			FEATURE,
			LEVEL_PACK
		}
//        public const string SPLIT_TEST_TYPE_FEATURE = "FEATURE";
//        public const string SPLIT_TEST_TYPE_LEVEL_PACK = "LEVEL_PACK";

        private string _partition;

		/// <summary>
		/// Gets the partition letter the client is assign to (<c>"A"</c>, <c>"B"</c>, <c>"C"</c>, etc.).
		/// </summary>
        public string partition
        {
            get { return _partition; }
        }

        private string _token;

		/// <summary>
		/// Gets the token -  a unique identifier for the experiment.
		/// </summary>
        public string token
        {
            get { return _token; }
        }

		private Type _type;

		/// <summary>
		/// Gets the type of the experiment.
		/// </summary>
		public Type type
        {
            get { return _type; }
        }

		private IEnumerable<object> _customEvents;

		internal IEnumerable<object> customEvents
        {
            get { return _customEvents; }
        }

        internal SLTExperiment(string token, string partition, string type, IEnumerable<object> customEvents)
        {
            _token = token;
            _partition = partition;
            _type = (Type)Enum.Parse(typeof(Type), type);
            _customEvents = customEvents;
        }
    }
}
