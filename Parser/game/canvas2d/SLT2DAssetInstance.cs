using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace saltr_unity_sdk
{
    public class SLT2DAssetInstance : SLTAssetInstance
    {
        private float _x;

        public float x
        {
            get { return _x; }
            private set { _x = value; }
        }

        private float _y;

        public float y
        {
            get { return _y; }
            private set { _y = value; }
        }

        private float _rotation;
        public float rotation
        {
            get { return _rotation; }
            private set { _rotation = value; }
        }


        public SLT2DAssetInstance(string token, List<SLTAssetState> states, object properties, float x, float y, float rotaition)
            : base(token, states, properties)
        {
            _x = x;
            _y = y;
        }

        public Vector3 position { get { return new Vector2(_x, _y); } set { _x = value.x; _y = value.y; } }

    }
}
