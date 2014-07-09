using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace saltr_unity_sdk
{
    public class SLT2dAssetInstance : SLTAssetInstance
    {
        public SLT2dAssetInstance(string token, List<SLTAssetState> state, object properties)
            : base(token,state, properties)
        {
        }

        public Vector3 position { get; set; }
        public Vector2 size { get; set; }
        public float rotationAngle { get; set; }

        public Vector2 pivot { get; set; }

    }
}
