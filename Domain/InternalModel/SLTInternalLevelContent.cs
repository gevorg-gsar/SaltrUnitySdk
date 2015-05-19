using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Domain.InternalModel.Matching;
using Saltr.UnitySdk.Utils;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain.InternalModel.Canvas;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Saltr.UnitySdk.Domain.Model;

namespace Saltr.UnitySdk.Domain.InternalModel
{
    public class SLTInternalLevelContent
    {
        #region Properties

        public Dictionary<string, SLTInternalBoard> Boards { get; set; }

        public Dictionary<string, SLTAssetType> Assets { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        #endregion Properties

    }

    public class BoardConverter : CustomCreationConverter<SLTInternalBoard>
    {
        public static SLTLevelType LevelType { get; set; }

        public override SLTInternalBoard Create(Type objectType)
        {
            if (LevelType == SLTLevelType.Matching)
            {
                return new SLTInternalMatchingBoard();
            }
            else if (LevelType == SLTLevelType.Canvas2D)
            {
                return new SLTInternalCanvasBoard();
            }
            else
            {
                return null;
            }
        }
    }

    public class SLTAssetTypeConverter : CustomCreationConverter<SLTAssetType>
    {
        public static SLTLevelType LevelType { get; set; }

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