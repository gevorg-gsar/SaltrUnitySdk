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
        #region Properties

        public Dictionary<string, string> Variables { get; private set; }

        public string Url { get; private set; }

        public int MaxAttempts { get; set; }

        public int Fails { get; set; }

        public int DropTimeout { get; set; }

        public int IdleTimeout { get; set; }

        public string Method { get; set; }

        public Dictionary<string, string> GetUrlVars()
        {
            return Variables;
        }

        #endregion Properties

        #region Ctor

        public SLTResourceTicket(string url, Dictionary<string, string> urlVars)
        {
            // TODO: Complete member initialization
            this.Url = url;
            this.Variables = urlVars;
        }

        #endregion Ctor

        #region Business Mehods

        public string GetURLRequest()
        {
            string requestUrl = Url;
            char seperator = '?';
            if (Variables != null)
            {
                foreach (string key in Variables.Keys)
                {
                    requestUrl += seperator;
                    requestUrl += key + "=" + WWW.EscapeURL(Variables[key]);
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
