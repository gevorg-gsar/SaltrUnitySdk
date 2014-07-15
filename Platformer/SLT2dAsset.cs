using saltr_unity_sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{
    class SLT2DAssetState : SLTAssetState
    {
        private float _pivotX;
        private float _pivotY;

        public SLT2DAssetState(string token, Dictionary<string, object> properties, float pivotX, float pivotY)
            : base(token, properties)
        {
            _pivotX = pivotX;
            _pivotY = pivotY;
        }


        public float pivotX
        {
            get { return _pivotX; }
            private set { _pivotX = value; }
        }

        public float pivotY
        {
            get { return _pivotY; }
            private set { _pivotY = value; }
        }

    }
}
