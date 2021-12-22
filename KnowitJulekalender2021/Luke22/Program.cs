using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Luke22
{
    class Board
    {
        string[,] Pieces = new string[4,4]; //row,col
        public string Debug => ToString();
        public bool IsSolved =>
            ToString() == 
@"🎅🎅🎅🎅
⛄⛄⛄⛄
✨✨✨✨
🎄🎄🎄🎄
";

        public Board(string pieces)
        {
            int i = 0;
            var enumerator = StringInfo.GetTextElementEnumerator(pieces);
            while (enumerator.MoveNext())
            {
                Pieces[i / 4, i % 4] = enumerator.GetTextElement();
                i++;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                sb.Append(Pieces[i / 4, i % 4]);
                if (i % 4 == 3) sb.AppendLine();
            }
            return sb.ToString();
        }

        public void MoveRow(int row, bool right)
        {
            int start = right ? 0 : 3;
            int end = right ? 3 : 0;
            var tmp = Pieces[row, end];
            if (right) for (int col = 3; col > 0; col--) Pieces[row, col] = Pieces[row, col - 1];
            else for (int col = 0; col < 3; col++) Pieces[row, col] = Pieces[row, col + 1];
            Pieces[row, start] = tmp;
        }

        public void MoveColumn(int col, bool down)
        {
            int start = down ? 0 : 3;
            int end = down ? 3 : 0;
            var tmp = Pieces[end, col];
            if (down) for (int row = 3; row > 0; row--) Pieces[row, col] = Pieces[row - 1, col];
            else for (int row = 0; row < 3; row++) Pieces[row, col] = Pieces[row + 1, col];
            Pieces[start, col] = tmp;
        }

        public void Move(byte move)
        {
            //0bRFNN 
            int no = move & 0b0011;
            bool row = (move & 0b1000) != 0;
            bool forward = (move & 0b0100) != 0;
            if (row) MoveRow(no, forward);
            else MoveColumn(no, forward);
        }
        
        public byte InverseMove(byte move)
        {
            bool forward = (move & 0b0100) != 0;
            if (forward) return (byte)(move & 0b1011);
            else return (byte)(move | 0b0100);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Emoji console output requires Windows Terminal, does not work in powershell or cmd.exe
            //SolveExample();
            Console.WriteLine(Solve());
        }

        static int Solve()
        {
            var boards = File.ReadAllLines("boards.txt").Select(l => new Board(l));
            int total = 0;
            foreach(var board in boards)
            {
                int best = SolveRecursive(board, new Stack<byte>(), 7);
                while (true)
                {
                    var count = SolveRecursive(board, new Stack<byte>(), best - 1);
                    if (count == 0) break;
                    best = count;
                }
                Console.WriteLine(best);
                total += best;
            }
            return total;
        }
        static bool SolveExample()
        {
            var board = new Board("🎅🎅⛄🎄✨⛄⛄🎅🎄✨✨⛄🎅🎄🎄✨");
            int best = SolveRecursive(board, new Stack<byte>(), 10);
            while (true)
            {
                var count = SolveRecursive(board, new Stack<byte>(), best - 1);
                if (count == 0) break;
                best = count;
            }
            Console.WriteLine(best);
            return board.IsSolved;
        }

        static int SolveRecursive(Board board, Stack<byte> moves, int max)
        {
            byte? back = moves.Count > 0 ? board.InverseMove(moves.Peek()) : null;
            if (board.IsSolved)
            {
                Console.WriteLine("Found solution!");
                Console.WriteLine(string.Join("->", moves));
                return moves.Count;
            }
            if (moves.Count >= max)
            {
                return 0;
            }
            for (byte move = 0; move < 16; move++)
            {
                if (move != back)
                {
                    board.Move(move);
                    moves.Push(move);
                    int count = SolveRecursive(board, moves, max);
                    board.Move(board.InverseMove(move));
                    moves.Pop();
                    if (count > 0) return count;
                }
            }
            return 0;
        }
    }
}
