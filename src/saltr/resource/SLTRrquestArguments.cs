using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.resource
{
   public class SLTRequestArguments
    {
       public string CLIENT { get; set; }
        public string deviceId { get; set; }

        public string socialId { get; set; }

        public string socialNetwork { get; set; }

        public string clientKey { get; set; }

        public string appVersion { get; set; }

        public List<object> developerFeatures { get; set; }

        public string apiVersion { get; set; }

        public string saltrUserId { get; set; }

        public Dictionary<string,object> basicProperties { get; set; }

        public Dictionary<string, object> customProperties { get; set; }


    }
}
