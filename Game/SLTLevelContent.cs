using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Utils;
using Newtonsoft.Json;
using Saltr.UnitySdk.Game.Canvas;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Saltr.UnitySdk.Game
{
    public class SLTLevelContent
    {
        #region Properties

        public Dictionary<string, SLTBoard> Boards { get; set; }

        public Dictionary<string, SLTAssetType> Assets { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

    }

    public class BoardConverter : CustomCreationConverter<SLTBoard>
    {
        public SLTLevelType LevelType { get; set; }

        public override SLTBoard Create(Type objectType)
        {
            if (LevelType == SLTLevelType.Matching)
            {
                return new SLTMatchingBoard();
            }
            else if (LevelType == SLTLevelType.Canvas2D)
            {
                return new SLTCanvasBoard();
            }
            else
            {
                return null;
            }
        }
    }

    public class SLTAssetTypeConverter : CustomCreationConverter<SLTAssetType>
    {
        public SLTLevelType LevelType { get; set; }

        public override SLTAssetType Create(Type objectType)
        {
            if (LevelType == SLTLevelType.Matching)
            {
                return new SLTMatchingAssetType();
            }
            else if (LevelType == SLTLevelType.Canvas2D)
            {
                return new SLTCanvasAssetType();
            }
            else
            {
                return null;
            }
        }
    }

}