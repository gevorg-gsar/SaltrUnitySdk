using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Game.Canvas2D;
using Saltr.UnitySdk.Utils;
using Saltr.UnitySdk.Status;

namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// Represents a level - a uniquely identifiable collection of boards and user defined properties.
    /// </summary>
    public class SLTLevel
    {
        #region Fields

        private string _id;
        private int _index;
        private int _localIndex;
        private int _packIndex;
        private string _version;
        private string _contentUrl;
        private object _properties;
        private bool _isContentReady;
        private SLTLevelType _levelType;
        private Dictionary<string, object> _assetMap = new Dictionary<string, object>();

        protected Dictionary<string, object> _boards;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the index of the pack the level is in.
        /// </summary>
        public int PackIndex
        {
            get { return _packIndex; }
        }

        /// <summary>
        /// Gets the index of the level within its pack.
        /// </summary>
        public int LocalIndex
        {
            get { return _localIndex; }
        }

        /// <summary>
        /// Gets the URL, used to retrieve contents of the level from Saltr.
        /// </summary>
        public string ContentUrl
        {
            get { return _contentUrl; }
        }

        /// <summary>
        /// Gets the index of the level in all levels.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Gets the properties of the level.
        /// </summary>
        public object Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="saltr.Game.SLTLevel"/> content is ready to be read.
        /// </summary>
        /// <value><c>true</c> if content is ready; otherwise, <c>false</c>.</value>
        public bool ContentReady
        {
            get { return _isContentReady; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version
        {
            get { return _version; }
        }

        #endregion Properties


        #region Ctor

        public SLTLevel(string id, SLTLevelType levelType, int index, int localIndex, int packIndex, string contentUrl, object properties, string version)
        {
            _localIndex = localIndex;
            _packIndex = packIndex;
            _id = id;
            _levelType = levelType;
            _index = index;
            _contentUrl = contentUrl;
            _isContentReady = false;
            _properties = properties;
            _version = version;
        }

        #endregion Ctor

        #region Business Methods

        /// <summary>
        /// Gets a board by id.
        /// </summary>
        /// <returns>The board specified by the id.</returns>
        /// <param name="id">Board identifier.</param>
        public SLTBoard GetBoard(string id)
        {
            return _boards.GetValue<SLTBoard>(id);
        }

        public void UpdateContent(Dictionary<string, object> rootNode)
        {
            Dictionary<string, object> boardsNode = new Dictionary<string, object>();

            List<string> keys = new List<string>();
            foreach (var item in rootNode.Keys)
            {
                keys.Add(item);
            }

            if (rootNode.ContainsKey(SLTConstants.Boards))
            {
                boardsNode = rootNode[SLTConstants.Boards] as Dictionary<string, object>;
            }
            else
            {
                Debug.Log("[SALTR: ERROR] Level content's 'boards' node can not be found.");
            }

            _properties = rootNode[SLTConstants.Properties];

            SLTLevelParser parser = SLTLevelParserFactory.GetParser(_levelType);
            if (parser != null)
            {
                _assetMap = parser.ParseLevelAssets(rootNode);
                _boards = parser.ParseLevelContent(boardsNode, _assetMap);

                if (_boards != null)
                {
                    RegenerateAllBoards();
                    _isContentReady = true;
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
        public void RegenerateBoard(string boardId)
        {
            if (_boards != null && _boards[boardId] != null)
            {
                SLTBoard board = _boards[boardId] as SLTBoard;
                board.Regenerate();
            }
        }

        /// <summary>
        /// Regenerates contents of all boards.
        /// </summary>
        public void RegenerateAllBoards()
        {
            foreach (var key in _boards.Keys)
            {
                if (_boards[key] as SLTBoard == null)
                { 
                    Debug.Log("castNull");
                }

                (_boards[key] as SLTBoard).Regenerate();
            }
        }

        #endregion Business Methods
    }

    public enum SLTLevelType
    {
        /// <summary>
        /// Default Enum value is 0, should be used to detect that not supported LevelType is received.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Used for parsing data retrieved from saltr.
        /// </summary>
        NoLevels,
        /// <summary>
        /// A level with "matching" boards and assets.
        /// </summary>
        Matching,
        /// <summary>
        /// A level with 2D boards and assets.
        /// </summary>
        Canvas2D
    }
}