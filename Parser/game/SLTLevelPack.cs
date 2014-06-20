using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevelPack
    {
        private string _token;

        public string token
        {
            get { return _token; }
        }

        private List<SLTLevel> _levels;

        public List<SLTLevel> levels
        {
            get { return _levels; }
        }

        private int _index;
        public int index
        {
            get { return _index; }
        }

        public SLTLevelPack(string token, int index, List<SLTLevel> levels)
        {
            _token = token;
            _index = index;
            _levels = levels;
        }

        public override string ToString()
        {
            return _token;
        }

        public class SortByIndex : IComparer<SLTLevelPack>
        {
            public int Compare(SLTLevelPack x, SLTLevelPack y)
            {
                if (x == null && y != null)
                    return -1;

                if (x != null && y == null)
                    return 1;

                if (x == null && y == null)
                    return 1;


                if (x.index > y.index)
                    return 1;

                if (x.index < y.index)
                    return -1;
                return 1;
            }
        }

    }
}
