using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// Represents a level pack - a uniquely identifiable collection of levels.
    /// </summary>
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
