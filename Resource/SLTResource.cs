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
    public class SLTResource
    {
        #region Fields

        private string _id;
        private bool _isLoaded;
        private SLTResourceTicket _ticket;

        protected int _fails;
        protected int _maxAttempts;
        protected int _dropTimeout;
        protected int _httpStatus;
        protected List<object> _responseHeaders = new List<object>();

        protected Action<SLTResource> _onSuccess;
        protected Action<SLTResource> _onFail;
        //        protected Action _onProgress;

        #endregion Fields

        #region Properties

        protected string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        public SLTResourceTicket Ticket
        {
            get { return _ticket; }
            set { _ticket = value; }
        }

        public Dictionary<string, object> Data { set; get; }

        #endregion Properties

        #region Ctor

        public SLTResource(string id, SLTResourceTicket ticket, Action<SLTResource> onSuccess, Action<SLTResource> onFail)
        {
            this._id = id;
            this._ticket = ticket;
            this._onSuccess = onSuccess;
            this._onFail = onFail;

            _maxAttempts = _ticket.MaxAttempts;
            _fails = 0;
            _dropTimeout = _ticket.DropTimeout;
            _httpStatus = -1;
        }

        #endregion Ctor

        #region Business Methods

        public void Load()
        {
            _fails++;
            GameObject go = GameObject.Find(SLTUnity.SALTR_GAME_OBJECT_NAME);
            NetworkWrapper wrapper = (NetworkWrapper)go.GetComponent(typeof(NetworkWrapper));
            if (Ticket.Method == SLTConstants.HttpMethodPost)
            {
                //post
                wrapper.POST(Ticket.GetURLRequest(), Ticket.GetUrlVars(), _onSuccess, _onFail, this);
            }
            else
            {
                wrapper.GET(_ticket.GetURLRequest(), _onSuccess, _onFail, this);
            }
        }

        public void Dispose()
        {
        }

        #endregion Business Methods

        #region Event Handlers

        public void IOErrorHandler()
        {
            if (_fails == _maxAttempts)
            {
                _onFail(this);
            }
            else
            {
                this.Load();
            }
        }

        #endregion Event Handlers

    }
}
