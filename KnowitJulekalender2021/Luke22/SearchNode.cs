using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luke22
{
    struct SearchNode
    {
        public byte Gen;
        public Board Board;

        public SearchNode(byte gen, Board board) { this.Gen = gen; this.Board = board; }

        public SearchNode[] GetEdges()
        {
            var arr = new SearchNode[16];
            for (byte i = 0; i < 16; i++) arr[i] = new SearchNode((byte)(Gen + 1), Board.WithMove(i));
            return arr;
        }
    }


}
