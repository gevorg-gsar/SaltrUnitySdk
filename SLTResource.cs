using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;
using System.Threading;
using System.ComponentModel;
using saltr_unity_sdk;

namespace Assets
{
    public class SLTResource : MonoBehaviour
    {
        private string _id;

        protected string id
        {
            get { return _id; }
            set { _id = value; }
        }

        private bool _isLoaded;

        public bool isLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        private SLTResourceTicket _ticket;

        public SLTResourceTicket ticket
        {
            get { return _ticket; }
            set { _ticket = value; }
        }

        protected int _fails;

        protected int _maxAttempts;
        protected int _dropTimeout;
        protected int _httpStatus;
        protected List<object> _responseHeaders = new List<object>();


        protected Action<SLTResource> _onSuccess;
        protected Action _onProgress;
        private Action<SLTResource> _onFail;

        public SLTResource(string Id, SLTResourceTicket Ticket, Action<SLTResource> onSuccess, Action<SLTResource> onFail)
        {
            this._id = Id;
            this._ticket = Ticket;
            this._onSuccess = onSuccess;
            this._onFail = onFail;

            _maxAttempts = _ticket.maxAttempts;
            _fails = 0;
            _dropTimeout = _ticket.dropTimeout;
            _httpStatus = -1;
        }


        public void ioErrorHandler()
        {
            if (_fails == _maxAttempts)
                _onFail(this);
            else
                this.load();
        }


        private void initLoader()
        {
        }


        public void load()
        {
            _fails++;
            GameObject go = GameObject.Find("saltr");
            GETPOSTWrapper wrapper = (GETPOSTWrapper)go.GetComponent(typeof(GETPOSTWrapper));
            if (ticket.method == "post")
                //post

                wrapper.POST(ticket.getURLRequest(), ticket.GetUrlVars().toDictionaryOrNull(), _onSuccess , _onFail, this);

            else
                wrapper.GET(_ticket.getURLRequest(), _onSuccess, _onFail, this);
        }


        internal void dispose()
        {
        }

        public Dictionary<string, object> data
        {
            set;
            get;
        }
    }
}
