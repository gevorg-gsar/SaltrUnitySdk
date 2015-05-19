using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.InternalModel
{
    public abstract class SLTAssetState
    {
        #region Properties

        public string Token { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

    }
}
