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
using Saltr.UnitySdk.Utils;

namespace Saltr.UnitySdk.Resource
{
    internal class SLTResource
    {
        private string _id;

        protected string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        private SLTResourceTicket _ticket;

        public SLTResourceTicket Ticket
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
//        protected Action _onProgress;
        private Action<SLTResource> _onFail;

        internal SLTResource(string Id, SLTResourceTicket Ticket, Action<SLTResource> onSuccess, Action<SLTResource> onFail)
        {
            this._id = Id;
            this._ticket = Ticket;
            this._onSuccess = onSuccess;
            this._onFail = onFail;

            _maxAttempts = _ticket.MaxAttempts;
            _fails = 0;
            _dropTimeout = _ticket.DropTimeout;
            _httpStatus = -1;
        }


        public void IOErrorHandler()
        {
            if (_fails == _maxAttempts)
                _onFail(this);
            else
                this.Load();
        }

        public void Load()
        {
            _fails++;
            GameObject go = GameObject.Find(SLTUnity.SALTR_GAME_OBJECT_NAME);
            NetworkWrapper wrapper = (NetworkWrapper)go.GetComponent(typeof(NetworkWrapper));
            if (Ticket.Method == "post")
                //post
                wrapper.POST(Ticket.GetURLRequest(), Ticket.GetUrlVars(), _onSuccess , _onFail, this);

            else
                wrapper.GET(_ticket.GetURLRequest(), _onSuccess, _onFail, this);
        }


        internal void Dispose()
        {
        }

        public Dictionary<string, object> Data
        {
            set;
            get;
        }
    }
}
