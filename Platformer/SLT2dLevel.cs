using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace saltr_unity_sdk
{
    public class SLT2dLevel : SLTLevel
    {
        public override void generateAllBoards()
        {
            if (boardsNode != null)
                boards = Deserializer2d.decod2dBoards(boardsNode, levelSettings);
        }

        public override void generateBoard(string boardId)
        {
            generateAllBoards();
        }

        public SLT2dLevel(string id, int index, int localIndex, int packIndex, string contentDataUrl, object properties, string version)
            : base(id, index, localIndex, packIndex, contentDataUrl, properties, version)
        {

        }
    }
}
