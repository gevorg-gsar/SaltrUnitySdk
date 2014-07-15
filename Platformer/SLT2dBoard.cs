using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Saltr_SDK.Platformer;


namespace saltr_unity_sdk
{
    public class SLT2dBoard : SLTBoard
    {
        private string _width;

        public string width
        {
            get { return _width; }
            set { _width = value; }
        }


        private string _height;

        public string height
        {
            get { return _height; }
            set { _height = value; }
        }


        public List<SLT2DBoardLayer> layers { get; set; }
        private Dictionary<string, object> layersNode { get; set; }


        public Vector2 position { get; set; }
       public object properties { get; set; }


     
    }
}
