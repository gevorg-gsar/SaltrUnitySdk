using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.resource
{
   public class SLTRequestArguments
    {
       public string CLIENT;
       public string deviceId;

       public string socialId;

       public string socialNetwork;

       public string clientKey;

       public string appVersion;

       public List<object> developerFeatures;

       public string apiVersion;

       public string saltrUserId;

       public Dictionary<string, object> basicProperties;
       public Dictionary<string, object> customProperties;

    }
}
