using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Domain.Model
{
    public class SLTLevelPack
    {
        public string Token { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public int Index { get; set; }

        public List<SLTLevel> Levels { get; set; }

        public override string ToString()
        {
            return Token;
        }
    }
}
