using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;


namespace saltr_unity_sdk
{
    public abstract class SLTLevel
    {
        private int _packIndex;

        public  const string LEVEL_TYPE_NONE = "noLevels";
        public  const string LEVEL_TYPE_MATCHING = "matching";
        public  const string LEVEL_TYPE_2DCANVAS = "canvas2D";

        Dictionary<string, object> _assetMap = new Dictionary<string, object>();

        public int packIndex
        {
            get { return _packIndex; }
            private set { _packIndex = value; }
        }


        private int _localIndex;

        public int localIndex
        {
            get { return _localIndex; }
            private set { _localIndex = value; }
        }

        private string _id;


        private string _contentDataUrl;

        public string contentDataUrl
        {
            get { return _contentDataUrl; }
        }

        private int _index;

        public int index
        {
            get { return _index; }
        }

        private object _properties;

        public object properties
        {
            get { return _properties; }
        }

        protected Dictionary<string, object> _boards;

        public virtual Dictionary<string, object> boards
        {
            get { return _boards; }
            set { _boards = value; }
        }

        private bool _contentReady;

        public bool contentReady
        {
            get { return _contentReady; }
        }

        private string _version;

        public string version
        {
            get { return _version; }
        }

        private Dictionary<string, object> _rootNode;

        public Dictionary<string, object> rootNode
        {
            get { return _rootNode; }
            set { _rootNode = value; }
        }
        private SLTLevelSettings _levelSettings;

        public SLTLevelSettings levelSettings
        {
            get { return _levelSettings; }
            set { _levelSettings = value; }
        }
        private Dictionary<string, object> _boardsNode;

        public Dictionary<string, object> boardsNode
        {
            get { return _boardsNode; }
            set { _boardsNode = value; }
        }

        public SLTLevel(string id, int index, int localIndex, int packIndex, string contentDataUrl, object properties, string version)
        {
            _localIndex = localIndex;
            _packIndex = packIndex;
            _id = id;
            _index = index;
            _contentDataUrl = contentDataUrl;
            _contentReady = false;
            _properties = properties;
            _version = version;
        }

        SLTLevelBoard getBoard(string id)
        {
            return _boards[id] as SLTLevelBoard;
        }

        public void updateContent(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> boardsNode = new Dictionary<string, object>();

			List<string> kieys = new List<string>();
			foreach (var item in rootNode.Keys) {
				kieys.Add(item);
	
			}

            if (rootNode.ContainsKey("boards"))
            {
                boardsNode = rootNode["boards"].toDictionaryOrNull();

            }
            else
            {
                Debug.Log("[SALTR: ERROR] Level content's 'boards' node can not be found.");
            }

            SLTLevelParser parser = getParser();

            _properties = rootNode["properties"];

            try
            {
                _assetMap = parser.parseLevelAssets(rootNode);
            }
            catch (Exception e)
            {
                Debug.Log("[SALTR: ERROR] Level content asset parsing failed.");
            }

            try
            {
                _boards = parser.parseLevelContent(boardsNode, _assetMap);
            }
            catch (Exception e)
            {
                Debug.Log("[SALTR: ERROR] Level content boards parsing failed.");
            }

            regenerateAllBoards();
            _contentReady = true;

        }

        public void regenerateBoard(string boardId)
        {
            if (_boards != null && _boards[boardId] != null)
            {
                SLTBoard board = _boards[boardId] as SLTBoard;
                board.regenerate();

            }
        }

        public void regenerateAllBoards()
        {
            foreach (var key in _boards.Keys)
            {
                if (_boards[key] as SLTBoard == null)
                    Debug.Log("castNull");

                (_boards[key] as SLTBoard).regenerate();
            }
        }


        protected abstract SLTLevelParser getParser();


        internal void dispose()
        {
            //TODO @GSAR: implement
        }

        public class SortById : IComparer<SLTLevel>
        {
            public int Compare(SLTLevel x, SLTLevel y)
            {
                if (x == null && y != null)
                    return -1;

                if (x != null && y == null)
                    return 1;

                if (x == null && y == null)
                    return 0;

                if (x.index > y.index)
                    return 1;

                if (x.index < y.index)
                    return -1;

                if (x.index == y.index)
                    return 0;

                return 0;
            }

        }
    }
}