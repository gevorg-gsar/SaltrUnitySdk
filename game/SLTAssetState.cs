using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
   public class SLTAssetState
    {
       public string token { get; private set; }
       public Dictionary<string, object> properties { get; private set; }

      public  SLTAssetState(string Token, Dictionary<string,object> Properties)
       {
           this.token = Token;
           this.properties = properties;
       }

    }
}
