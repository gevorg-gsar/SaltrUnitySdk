using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Saltr.UnitySdk.Resource
{
    public class SLTResourceTicket
    {
        private string _url;
        private Dictionary<string,string> _variables;

        private int _maxAttempts;

        public int MaxAttempts
        {
            get { return _maxAttempts; }
            set { _maxAttempts = value; }
        }

        private int _fails;
        public int Fails
        {
            get { return _fails; }
            set { _fails = value; }
        }

        private int _dropTimeout;
        public int DropTimeout
        {
            get { return _dropTimeout; }
            set { _dropTimeout = value; }
        }
		

       	public Dictionary<string,string> GetUrlVars()
        {
            return _variables;
        }

        public SLTResourceTicket(string url, Dictionary<string,string> urlVars)
        {
            // TODO: Complete member initialization
            this._url = url;
            this._variables = urlVars;
        }

        public int IdleTimeout { get; set; }
        public string Method { get; set; }


        internal string GetURLRequest()
        {
            string requestUrl = _url;
			char seperator = '?';
			if(_variables != null)
				foreach (string key in _variables.Keys)
	            {
	                requestUrl += seperator;
					requestUrl += key + "=" + WWW.EscapeURL(_variables[key]);	
					if('?' == seperator)
						seperator = '&';
	            }

            return requestUrl;
        }
    }
}
