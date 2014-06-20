using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTCompositeInstance : SLTAssetInstance
    {
        private IEnumerable<SLTCell> _cells;

        public IEnumerable<SLTCell> cells
        {
            get { return _cells; }
        }

        public SLTCompositeInstance(string Token, string state, object Properties, IEnumerable<SLTCell> cells)
            : base(Token, state, Properties)
        {
            _cells = cells;
        }
    }
}
