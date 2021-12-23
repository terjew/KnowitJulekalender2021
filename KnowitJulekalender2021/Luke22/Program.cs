using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Luke22
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Emoji console output requires Windows Terminal, does not work in powershell or cmd.exe
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var solver = new Solver();
            Console.WriteLine(solver.SolveExample());
            Console.WriteLine(solver.SolveBidirectional());
            BenchmarkRunner.Run<Solver>();
        }

    }
    public class Solver { 

        [Benchmark]
        public int SolveBidirectional()
        {
            var boards = File.ReadAllLines("boards.txt").Select(l => new Board(l));
            Stopwatch sw = Stopwatch.StartNew();
            int maxDepth = 6;
            var distancesFromSolved = GetDistances(Board.Solved(), maxDepth);
            int total = 0;
            foreach(var board in boards)
            {
                int moves = SearchBFS(board, distancesFromSolved);
                total += moves;
            }
            return total;
        }

        public int SolveExample()
        {
            var b = new Board("🎅🎅⛄🎄✨⛄⛄🎅🎄✨✨⛄🎅🎄🎄✨");
            Console.WriteLine(b);
            b.Move(false, 3, false);
            b.Move(true, 0, true);
            b.Move(true, 0, true);
            b.Move(false, 0, true);
            Console.WriteLine(b);
            return SearchBFS(new Board("🎅🎅⛄🎄✨⛄⛄🎅🎄✨✨⛄🎅🎄🎄✨"));
        }

        internal static Dictionary<Board, byte> GetDistances(Board startBoard, int maxDepth)
        {
            var node = new SearchNode(0, startBoard);
            Queue<SearchNode> queue = new Queue<SearchNode>();
            Dictionary<Board, byte> known = new Dictionary<Board, byte>();
            known.Add(startBoard, 0);
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Gen > maxDepth - 1) continue;
                foreach (var edge in current.GetEdges())
                {
                    if (!known.ContainsKey(edge.Board))
                    {
                        known.Add(edge.Board, edge.Gen);
                        queue.Enqueue(edge);
                    }
                }
            }
            return known;
        }

        internal static int SearchBFS(Board start, Dictionary<Board, byte> targets)
        {
            if (targets.ContainsKey(start))
            {
                return targets[start];
            }
            var node = new SearchNode(0, start);
            Queue<SearchNode> queue = new Queue<SearchNode>();
            HashSet<Board> known = new HashSet<Board>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                known.Add(current.Board);
                foreach (var edge in current.GetEdges())
                {
                    if (targets.ContainsKey(edge.Board))
                    {
                        return targets[edge.Board] + edge.Gen;
                    }

                    if (!known.Contains(edge.Board))
                    {
                        known.Add(edge.Board);
                        queue.Enqueue(edge);
                    }
                }
            }
            return 0;
        }


        internal static int SearchBFS(Board board)
        {
            Dictionary<Board, byte> targets = new Dictionary<Board, byte>();
            targets.Add(Board.Solved(), 0);
            return SearchBFS(board, targets);
        }

    }
}
