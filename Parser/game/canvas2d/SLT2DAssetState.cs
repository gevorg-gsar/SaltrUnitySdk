using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLT2DAssetState : SLTAssetState
    {
        private float _pivotX;

        public float pivotX
        {
            get { return _pivotX; }
        }

        private float _pivotY;

        public float pivotY
        {
            get { return _pivotY; }
        }

        public SLT2DAssetState(string token, Dictionary<string, object> properties, float pivotX, float pivotY)
            : base(token, properties)
        {
            _pivotX = pivotX;
            _pivotY = pivotY;
        }
    }
}
