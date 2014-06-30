using saltr_unity_sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace saltr_unity_sdk
{
    class SLT2dAsset : SLTAsset
    {
        public SLT2dAsset(string token, Dictionary<string ,object> states, Dictionary<string,object> properties, Vector2 size, Vector2 pivot):base(token,properties,states)
        {
            _size = size;
            _pivot = pivot;
        }

        private Vector2 _size;

        public Vector2 size
        {
            get { return _size; }
            set { _size = value; }
        }

        private Vector2 _pivot;

        public Vector2 pivot
        {
            get { return _pivot; }
            set { _pivot = value; }
        }



    }
}
