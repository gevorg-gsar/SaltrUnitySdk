using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    internal class SLTComposite
    {
        private string _assetId;
        private string _stateId;
        private SLTCell _cell;
        private Dictionary<string, object> _assetMap;

        private Dictionary<string, object> _stateMap;
        private SLTBoardLayer _layer;

        public SLTComposite(SLTBoardLayer layer, string compositeAssetId, string stateId, SLTCell cell, SLTLevelSettings levelSettings)
        {
            _assetId = compositeAssetId;
            _stateId = stateId;
            _cell = cell;
            _assetMap = levelSettings.assetMap;
            _stateMap = levelSettings.stateMap;
            _layer = layer;
            generateCellContent();
        }

        public string getAssetId()
        {
            return _assetId;
        }

        private void generateCellContent()
        {
            SLTCompositeAsset asset = _assetMap[_assetId] as SLTCompositeAsset;
            string state = _stateMap[_stateId].ToString();
         //   _cell.setAssetInstance(_layer.layerId, _layer.layerIndex, new SLTCompositeInstance(asset.token, state, asset.properties, asset._cellInfos as IEnumerable<SLTCell>));
        }
    }
}
