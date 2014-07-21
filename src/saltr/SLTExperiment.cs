using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr
{
    public class SLTExperiment
    {
        public const string SPLIT_TEST_TYPE_FEATURE = "FEATURE";
        public const string SPLIT_TEST_TYPE_LEVEL_PACK = "LEVEL_PACK";

        private string _partition;

        public string partition
        {
            get { return _partition; }
        }
        private string _token;

        public string token
        {
            get { return _token; }
        }
        private string _type;

        public string type
        {
            get { return _type; }
        }

		private IEnumerable<object> _customEvents;

		public IEnumerable<object> customEvents
        {
            get { return _customEvents; }
        }

        public SLTExperiment(string token, string partition, string type, IEnumerable<object> customEvents)
        {
            _token = token;
            _partition = partition;
            _type = type;
            _customEvents = customEvents;
        }
    }
}
