using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace saltr_unity_sdk
{
    public class SLT2dBoard
    {
        public string id { get; set; }
        public List<SLT2dBoardLayer> layers { get; set; }
        private Dictionary<string, object> layersNode { get; set; }


        public string width { get; set; }
        public string height { get; set; }
        public Vector2 position { get; set; }
       public object properties { get; set; }


     
    }
}
