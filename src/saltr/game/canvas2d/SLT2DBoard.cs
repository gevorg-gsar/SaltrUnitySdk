using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLT2DBoard : SLTBoard
    {

        private float _width;

        public float width
        {
            get { return _width; }
        }

        private float _height;

        public float height
        {
            get { return _height; }
        }


        public SLT2DBoard(float width, float height, List<SLTBoardLayer> layers, Dictionary<string, object> properties)
            : base(layers, properties)
        {
            _width = width;
            _height = height;
        }


    }
}
