using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Saltr.UnitySdk.Game.Canvas2D
{
    /// <summary>
    /// Represents a layer of a 2D board.
    /// </summary>
    public class SLT2DBoardLayer : SLTBoardLayer
    {
        #region Properties

        /// <summary>
        /// Gets all the asset instances present on the board.
        /// </summary>
        public List<SLT2DAssetInstance> AssetInstances
        {
            get;
            private set;
        }

        #endregion Properties

        #region Ctor

        public SLT2DBoardLayer(string layerId, int layerIndex)
            : base(layerId, layerIndex)
        {
            AssetInstances = new List<SLT2DAssetInstance>();
        }

        #endregion Ctor

        #region Business Methods

        /// <summary>
        /// Adds the asset instance to the board.
        /// </summary>
        public void AddAssetInstance(SLT2DAssetInstance instance)
        {
            AssetInstances.Add(instance);
        }

        /// <summary>
        /// Currently has no effect, since 2D boards do not have any randomized content.
        /// </summary>
        public override void Regenerate()
        {
            // nothing to do here yet
        }

        #endregion Business Methods        
    }
}