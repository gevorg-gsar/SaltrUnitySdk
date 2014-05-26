using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevelBoardLayer
    {
        private string _layerId;
        private int _layerIndex;
        private IEnumerable<object> _fixedAssetsNodes;
        private IEnumerable<object> _compositeNodes;
        private IEnumerable<object> _chunkNodes;

        public string layerId
        {
            get { return _layerId; }
        }

        public int layerIndex
        {
            get { return _layerIndex; }
        }

        public IEnumerable<object> fixedAssetsNodes
        {
            get { return _fixedAssetsNodes; }
        }

        public IEnumerable<object> chunkNodes
        {
            get { return _chunkNodes; }
        }

        public IEnumerable<object> compositeNodes
        {
            get { return _compositeNodes; }
            set { _compositeNodes = value; }
        }

        public SLTLevelBoardLayer(string LayerId, int LayerIndex, IEnumerable<object> FixedAssetNodes, IEnumerable<object> ChunkNodes, IEnumerable<object> CompositeNodes)
        {
            _layerId = LayerId;
            _layerIndex = LayerIndex;
            _fixedAssetsNodes = FixedAssetNodes;
            _chunkNodes = ChunkNodes;
            _compositeNodes = CompositeNodes;
        }
    }
}
