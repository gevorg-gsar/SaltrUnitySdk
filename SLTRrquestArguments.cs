using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
   public class SLTRequestArguments
    {
        public string deviceId { get; set; }

        public string socialId { get; set; }

        public string socialNetwork { get; set; }

        public string clientKey { get; set; }

        public string appVersion { get; set; }

        public List<object> developerFeature { get; set; }
    }
}
