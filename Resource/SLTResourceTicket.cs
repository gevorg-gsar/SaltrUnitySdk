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
        #region Fields

        private int _fails;
        private int _maxAttempts;
        private int _dropTimeout;
        private string _url;
        private Dictionary<string, string> _variables;

        #endregion  Fields

        #region Properties

        public int MaxAttempts
        {
            get { return _maxAttempts; }
            set { _maxAttempts = value; }
        }

        public int Fails
        {
            get { return _fails; }
            set { _fails = value; }
        }

        public int DropTimeout
        {
            get { return _dropTimeout; }
            set { _dropTimeout = value; }
        }

        public int IdleTimeout { get; set; }

        public string Method { get; set; }

        public Dictionary<string, string> GetUrlVars()
        {
            return _variables;
        }

        #endregion Properties

        #region Ctor

        public SLTResourceTicket(string url, Dictionary<string, string> urlVars)
        {
            // TODO: Complete member initialization
            this._url = url;
            this._variables = urlVars;
        }

        #endregion Ctor

        #region Business Mehods

        public string GetURLRequest()
        {
            string requestUrl = _url;
            char seperator = '?';
            if (_variables != null)
            { 
                foreach (string key in _variables.Keys)
                {
                    requestUrl += seperator;
                    requestUrl += key + "=" + WWW.EscapeURL(_variables[key]);
                    if ('?' == seperator)
                    {
                        seperator = '&';
                    }
                }
            }

            return requestUrl;
        }

        #endregion  Business Mehods
        
    }
}
