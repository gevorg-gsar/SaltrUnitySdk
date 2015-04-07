using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saltr.UnitySdk.Utils;
using UnityEngine;

namespace Saltr.UnitySdk.Domain.InternalModel.Matching
{
    // <summary>
    // Represents a chunk, a collection of cells on matching board that is populated with assets according to certain rules.
    // </summary>
    public class SLTChunk
    {

        #region Properties

        public int? ChunkId { get; set; }

        public List<List<int>> Cells { get; set; }

        public List<SLTChunkAssetConfig> Assets { get; set; }

        #endregion Properties

    }
}
