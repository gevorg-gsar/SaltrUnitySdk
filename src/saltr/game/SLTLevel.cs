using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace saltr_unity_sdk
{
    public abstract class SLTLevel
	{
		protected Dictionary<string, object> _boards;

		private string _id;
		private string _contentUrl;
		private int _index;
		private object _properties;
		private bool _contentReady;
		private string _version;
		private int _packIndex;
		private int _localIndex;
		Dictionary<string, object> _assetMap = new Dictionary<string, object>();

        public  const string LEVEL_TYPE_NONE = "noLevels";
        public  const string LEVEL_TYPE_MATCHING = "matching";
        public  const string LEVEL_TYPE_2DCANVAS = "canvas2D";
		
        public int packIndex
        {
            get { return _packIndex; }
        }
		
        public int localIndex
        {
            get { return _localIndex; }
        }
		
        public string contentUrl
        {
            get { return _contentUrl; }
        }
		
        public int index
        {
            get { return _index; }
        }

        public object properties
        {
            get { return _properties; }
        }

        public bool contentReady
        {
            get { return _contentReady; }
        }


        public string version
        {
            get { return _version; }
        }

        public SLTLevel(string id, int index, int localIndex, int packIndex, string contentUrl, object properties, string version)
        {
            _localIndex = localIndex;
            _packIndex = packIndex;
            _id = id;
            _index = index;
            _contentUrl = contentUrl;
            _contentReady = false;
            _properties = properties;
            _version = version;
        }

        SLTBoard getBoard(string id)
        {
            return _boards[id] as SLTBoard;
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
                Debug.Log("[SALTR: ERROR] Level content asset parsing failed." + e.Message);
            }

            try
            {
                _boards = parser.parseLevelContent(boardsNode, _assetMap);
            }
            catch (Exception e)
            {
                Debug.Log("[SALTR: ERROR] Level content boards parsing failed." + e.Message);
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

		public class SortByIndex : IComparer<SLTLevel>
		{
			public int Compare(SLTLevel x, SLTLevel y)
			{
				if (x == null && y != null)
					return -1;
				
				if (x != null && y == null)
					return 1;
				
				if (x == null && y == null)
					return 1;
				
				
				if (x.index > y.index)
					return 1;
				
				if (x.index < y.index)
					return -1;

				if (x.index == y.index)
					return 0;

				return 1;
			}
		}

    }
}