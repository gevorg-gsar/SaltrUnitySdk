using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Game.Matching;
using Saltr.UnitySdk.Game.Canvas2D;
using Saltr.UnitySdk.Utils;
using Saltr.UnitySdk.Status;
using Newtonsoft.Json;

namespace Saltr.UnitySdk.Game
{
    /// <summary>
    /// Represents a level - a uniquely identifiable collection of boards and user defined properties.
    /// </summary>
    public class SLTLevel
    {
        #region Properties

        public int Id { get; set; }

        /// <summary>
        /// Gets the index of the level in all levels.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the index of the level within its pack.
        /// </summary>
        public int LocalIndex { get; set; }

        public int Version { get; set; }

        public int VariationId { get; set; }

        public int VariationVersion { get; set; }

        /// <summary>
        /// Gets the URL, used to retrieve contents of the level from Saltr.
        /// </summary>
        [JsonProperty("url")]
        public string ContentUrl { get; set; }

        /// <summary>
        /// Gets the properties of the level.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        public SLTLevelContent Content { get; set; }

        #endregion Properties

        #region Business Methods

        ///// <summary>
        ///// Gets a board by id.
        ///// </summary>
        ///// <returns>The board specified by the id.</returns>
        ///// <param name="id">Board identifier.</param>
        //public SLTBoard GetBoard(string id)
        //{
        //    return _boards.GetValue<SLTBoard>(id);
        //}

        //public void UpdateContent(Dictionary<string, object> rootNode)
        //{
        //    Dictionary<string, object> boardsNode = new Dictionary<string, object>();

        //    List<string> keys = new List<string>();
        //    foreach (var item in rootNode.Keys)
        //    {
        //        keys.Add(item);
        //    }

        //    if (rootNode.ContainsKey(SLTConstants.Boards))
        //    {
        //        boardsNode = rootNode[SLTConstants.Boards] as Dictionary<string, object>;
        //    }
        //    else
        //    {
        //        Debug.Log("[SALTR: ERROR] Level content's 'boards' node can not be found.");
        //    }

        //    _properties = rootNode[SLTConstants.Properties];

        //    SLTLevelParser parser = SLTLevelParserFactory.GetParser(_levelType);
        //    if (parser != null)
        //    {
        //        _assetMap = parser.ParseLevelAssets(rootNode);
        //        _boards = parser.ParseLevelContent(boardsNode, _assetMap);

        //        if (_boards != null)
        //        {
        //            RegenerateAllBoards();
        //            _isContentReady = true;
        //        }
        //    }
        //    else
        //    {
        //        // no parser was found for current level type
        //        //new SLTStatusLevelsParserMissing();
        //    }
        //}

        ///// <summary>
        ///// Regenerates contents of the board specified by boardId. 
        ///// </summary>
        ///// <param name="boardId">Board identifier.</param>
        //public void RegenerateBoard(string boardId)
        //{
        //    if (_boards != null && _boards[boardId] != null)
        //    {
        //        SLTBoard board = _boards[boardId] as SLTBoard;
        //        board.Regenerate();
        //    }
        //}

        ///// <summary>
        ///// Regenerates contents of all boards.
        ///// </summary>
        //public void RegenerateAllBoards()
        //{
        //    foreach (var key in _boards.Keys)
        //    {
        //        if (_boards[key] as SLTBoard == null)
        //        { 
        //            Debug.Log("castNull");
        //        }

        //        (_boards[key] as SLTBoard).Regenerate();
        //    }
        //}

        #endregion Business Methods
    }

    
}