using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr.game
{
    public class SLTBoardLayer
    {
        private string _token;
        private int _index;

        public SLTBoardLayer(string LayerId, int LayerIndex)
        {
            _token = LayerId;
            _index = LayerIndex;
        }


        public string token
        {
            get { return _token; }
        }

        public int index
        {
            get { return _index; }
        }

        public virtual void regenerate()
        {
            //override
        }
    }
}
