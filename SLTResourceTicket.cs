using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class SLTResourceTicket : MonoBehaviour
    {
        private string _url;
        private SLTRequestArguments args;
        private object urlVars;

        private int _maxAttempts;

        public int maxAttempts
        {
            get { return _maxAttempts; }
            set { _maxAttempts = value; }
        }
        private int _fails;

        public int fails
        {
            get { return _fails; }
            set { _fails = value; }
        }
        private int _dropTimeout;

        public int dropTimeout
        {
            get { return _dropTimeout; }
            set { _dropTimeout = value; }
        }


        public SLTResourceTicket(string url, SLTRequestArguments args)
        {
            // TODO: Complete member initialization
            this._url = url;
            this.args = args;
            this.maxAttempts = 2;
        }

        public SLTResourceTicket()
        {
            // TODO: Complete member initialization
        }

        public SLTResourceTicket(string url, object urlVars)
        {
            // TODO: Complete member initialization
            this._url = url;
            this.urlVars = urlVars;
        }
        public int idleTimeout { get; set; }

        public string method { get; set; }


        internal string getURLRequest()
        {
            string arguments = "";
            string requestUrl = _url;
            if (args != null)
            {
                arguments = WWW.EscapeURL(LitJson.JsonMapper.ToJson(args));



                requestUrl += "?cmd=getAppData&args=" + arguments;
            }

            return requestUrl;
        }
    }
}
