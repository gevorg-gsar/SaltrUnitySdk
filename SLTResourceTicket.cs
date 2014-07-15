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
       public bool isGet;

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
            isGet = true;
            // TODO: Complete member initialization
            this._url = url;
            this.args = args;
            this.maxAttempts = 2;
        }

       public object GetUrlVars()
        {
            return urlVars;
        }
        public SLTResourceTicket()
        {
            // TODO: Complete member initialization
        }

        public SLTResourceTicket(string url, SLTRequestArguments args, object urlVars)
        {
            isGet = false;
            // TODO: Complete member initialization
            this._url = url;
            this.urlVars = urlVars;
            this.args = args;
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

                if(isGet)
                requestUrl += "?cmd=getAppData&args=" + arguments;
                else
                {
                    requestUrl += "?cmd=syncFeatures&args=" + arguments;  
                }
            }

            return requestUrl;
        }
    }
}
