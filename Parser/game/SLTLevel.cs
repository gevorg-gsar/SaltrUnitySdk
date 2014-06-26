using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLTLevel
    {
        private int _packIndex;

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

        private Dictionary<string, object> _boards;

        public virtual  Dictionary<string, object> boards
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

        public  void updateContent(Dictionary<string, object> rootNode)
        {
            _rootNode = rootNode;

            if (rootNode.ContainsKey("boards"))
                _boardsNode = rootNode["boards"].toDictionaryOrNull();

            if (rootNode.ContainsKey("properties"))
                _properties = rootNode["properties"].toDictionaryOrNull();

            _levelSettings = SLTLevelBoardParser.parseLevelSettings(rootNode);
            generateAllBoards();
            _contentReady = true;
        }

        public virtual void generateAllBoards()
        {
            if (_boardsNode != null)
                _boards = SLTLevelBoardParser.parseLevelBoards(_boardsNode, _levelSettings);
        }

        public virtual void generateBoard(string boardId)
        {
            if (_boardsNode != null)
                _boards[boardId] = SLTLevelBoardParser.parseLevelBoard(_boardsNode[boardId].toDictionaryOrNull(), _levelSettings);
        }

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