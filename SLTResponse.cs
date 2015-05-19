using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk
{
    public class SLTResponse<T>
    {
        public List<T> Response { get; set; }
    }
}
