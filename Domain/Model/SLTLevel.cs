using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Saltr.UnitySdk.Domain.InternalModel.Matching;
using Saltr.UnitySdk.Utils;
using Newtonsoft.Json;
using Saltr.UnitySdk.Domain.InternalModel;

namespace Saltr.UnitySdk.Domain.Model
{
    public class SLTLevel
    {
        #region Properties

        public int Id { get; set; }

        public int? Index { get; set; }

        public int? PackIndex { get; set; }

        public int? LocalIndex { get; set; }

        public int? Version { get; set; }

        public int? VariationId { get; set; }

        public int? VariationVersion { get; set; }

        public string Url { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        [JsonIgnore]
        public SLTLevelContent Content { get; set; }

        [JsonIgnore]
        public SLTInternalLevelContent InternalLevelContent { get; set; }

        #endregion Properties

        #region Public Methods

        public void RegenerateBoards()
        {
            if (InternalLevelContent != null)
            {
                Content = InternalLevelContent.RegenerateBoards();
            }
        }

        public void RegenerateBoard(string token)
        {
            if (InternalLevelContent != null && InternalLevelContent.Boards.ContainsKey(token))
            {
                SLTLevelContent tmpLevelContent = InternalLevelContent.RegenerateBoard(token);
                if (tmpLevelContent != null)
                {
                    if (Content == null)
                    {
                        Content = new SLTLevelContent();
                    }

                    if (tmpLevelContent.CanvasBoards != null)
                    {
                        foreach (var item in tmpLevelContent.CanvasBoards)
                        {
                            Content.CanvasBoards[item.Key] = item.Value;
                        }
                    }

                    if (tmpLevelContent.MatchingBoards != null)
                    {
                        foreach (var item in tmpLevelContent.MatchingBoards)
                        {
                            Content.MatchingBoards[item.Key] = item.Value;
                        }
                    }
                }
            }
        }

        #endregion Public Methods

    }
}