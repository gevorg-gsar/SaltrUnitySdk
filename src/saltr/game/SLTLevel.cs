using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using saltr.game.matching;
using saltr.game.canvas2d;
using saltr.utils;
using saltr.status;

namespace saltr.game
{
	/// <summary>
	/// Represents a level - a uniquely identifiable collection of boards and user defined properties.
	/// </summary>
    public class SLTLevel
    {
        protected Dictionary<string, object> _boards;

        private string _id;
        private string _levelType;
        private int _index;
        private int _localIndex;
        private int _packIndex;
        private string _contentUrl;
        private object _properties;
        private string _version;

        private bool _contentReady;
        Dictionary<string, object> _assetMap = new Dictionary<string, object>();

		/// <summary>
		/// Used for parsing data retrieved from saltr.
		/// </summary>
        public const string LEVEL_TYPE_NONE = "noLevels";
        /// <summary>
        /// A level with "matching" boards and assets.
        /// </summary>
		public const string LEVEL_TYPE_MATCHING = "matching";
        /// <summary>
        /// A level with 2D boards and assets.
        /// </summary>
		public const string LEVEL_TYPE_2DCANVAS = "canvas2D";

        public static SLTLevelParser getParser(string levelType)
        {
            switch (levelType)
            {
                case LEVEL_TYPE_MATCHING:
                    return SLTMatchingLevelParser.getInstance();
                    break;
                case LEVEL_TYPE_2DCANVAS:
                    return SLT2DLevelParser.getInstance();
                    break;
            }
            return null;
        }

		/// <summary>
		/// Gets the index of the pack the level is in.
		/// </summary>
        public int packIndex
        {
            get { return _packIndex; }
        }

		/// <summary>
		/// Gets the index of the level within its pack.
		/// </summary>
        public int localIndex
        {
            get { return _localIndex; }
        }

		/// <summary>
		/// Gets the URL, used to retrieve contents of the level from Saltr.
		/// </summary>
        public string contentUrl
        {
            get { return _contentUrl; }
        }

		/// <summary>
		/// Gets the index of the level in all levels.
		/// </summary>
        public int index
        {
            get { return _index; }
        }

		/// <summary>
		/// Gets the properties of the level.
		/// </summary>
        public object properties
        {
            get { return _properties; }
        }

		/// <summary>
		/// Gets a value indicating whether this <see cref="saltr.game.SLTLevel"/> content is ready to be read.
		/// </summary>
		/// <value><c>true</c> if content is ready; otherwise, <c>false</c>.</value>
        public bool contentReady
        {
            get { return _contentReady; }
        }

		/// <summary>
		/// Gets the version.
		/// </summary>
        public string version
        {
            get { return _version; }
        }

        public SLTLevel(string id, string levelType, int index, int localIndex, int packIndex, string contentUrl, object properties, string version)
        {
            _localIndex = localIndex;
            _packIndex = packIndex;
            _id = id;
            _levelType = levelType;
            _index = index;
            _contentUrl = contentUrl;
            _contentReady = false;
            _properties = properties;
            _version = version;
        }

		/// <summary>
		/// Gets a board by id.
		/// </summary>
		/// <returns>The board specified by the id.</returns>
		/// <param name="id">Board identifier.</param>
        public SLTBoard getBoard(string id)
        {
            return _boards.getValue<SLTBoard>(id);
        }

        public void updateContent(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> boardsNode = new Dictionary<string, object>();

            List<string> keys = new List<string>();
            foreach (var item in rootNode.Keys)
            {
                keys.Add(item);

            }

            if (rootNode.ContainsKey("boards"))
            {
                boardsNode = rootNode["boards"].toDictionaryOrNull();

            }
            else
            {
                Debug.Log("[SALTR: ERROR] Level content's 'boards' node can not be found.");
            }

            _properties = rootNode["properties"];

            SLTLevelParser parser = getParser(_levelType);
            if (parser != null)
            {
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

                if (_boards != null)
                {
                    regenerateAllBoards();
                    _contentReady = true;
                }
            }
            else
            {
                // no parser was found for current level type
                new SLTStatusLevelsParserMissing();
            }

        }

		/// <summary>
		/// Regenerates contents of the board specified by boardId. 
		/// </summary>
		/// <param name="boardId">Board identifier.</param>
        public void regenerateBoard(string boardId)
        {
            if (_boards != null && _boards[boardId] != null)
            {
                SLTBoard board = _boards[boardId] as SLTBoard;
                board.regenerate();

            }
        }

		/// <summary>
		/// Regenerates contents of all boards.
		/// </summary>
        public void regenerateAllBoards()
        {
            foreach (var key in _boards.Keys)
            {
                if (_boards[key] as SLTBoard == null)
                    Debug.Log("castNull");

                (_boards[key] as SLTBoard).regenerate();
            }
        }

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