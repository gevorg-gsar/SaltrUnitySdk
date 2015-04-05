using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Model
{
    public class SLTAsset
    {
        #region Properties

        public string Token { get; set; }

        public Dictionary<string, object> Properties { get; set; }
        
        #endregion Properties
    }
}
