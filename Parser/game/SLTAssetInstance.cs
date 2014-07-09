using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTAssetInstance
    {
        private object _properties;
        public object properties
        {
            get { return _properties; }
        }
        private List<SLTAssetState> _state;
        public List<SLTAssetState> state
        {
            get { return _state; }
        }
        private string _token;

        public string token
        {
            get { return _token; }
        }

        public SLTAssetInstance(string Token, List<SLTAssetState> state, object Properties)
        {
            _token = Token;
            _state = state;
            _properties = Properties;
        }
    }
}
