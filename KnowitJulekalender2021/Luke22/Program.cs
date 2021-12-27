using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Luke22
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Emoji console output requires Windows Terminal, does not work in powershell or cmd.exe
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var solver = new Solver();
            //Console.WriteLine(solver.SolveBidirectional());
            Console.WriteLine(solver.SolveBidirectionalParallel());
            BenchmarkRunner.Run<Solver>();
        }

    }
    public class Solver {

        //[Benchmark]
        public int SolveBidirectional()
        {
            var boards = File.ReadAllLines("boards.txt").Select(l => new Board(l));
            int maxDepth = 6;
            var distancesFromSolved = GetDistances(Board.Solved, maxDepth);
            int total = 0;
            foreach(var board in boards) total += SearchBFS(board, distancesFromSolved);
            return total;
        }

        [Benchmark]
        public int SolveBidirectionalParallel()
        {
            var boards = File.ReadAllLines("boards.txt").Select(l => new Board(l));
            int maxDepth = 5;
            var distancesFromSolved = GetDistances(Board.Solved, maxDepth);
            return boards
                .AsParallel()
                .Select(b => SearchBFS(b, distancesFromSolved))
                .Sum();
        }

        //[Benchmark]
        public int SolveBidirectionalArraysParallel()
        {
            var boards = File.ReadAllLines("boards.txt").Select(l => new Board(l));
            Stopwatch sw = Stopwatch.StartNew();
            int maxDepth = 5;
            var distancesFromSolved = GetDistancesArray(Board.Solved, maxDepth);
            return boards
                .AsParallel()
                .Select(b => SearchBFSArray(b, distancesFromSolved))
                .Sum();
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

        const int MAX = 1820 * 499 * 71;

        internal static byte[] GetDistancesArray(Board startBoard, int maxDepth)
        {
            var node = new SearchNode(1, startBoard);
            Queue<SearchNode> queue = new Queue<SearchNode>();
            byte[] distances = new byte[MAX];
            distances[startBoard.GetBoardNo()] = 1;
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Gen > maxDepth) continue;
                foreach (var edge in current.GetEdges())
                {
                    var index = edge.Board.GetBoardNo();
                    if (distances[index] == 0)
                    {
                        distances[index] = edge.Gen;
                        queue.Enqueue(edge);
                    }
                }
            }
            return distances;
        }

        internal static int SearchBFSArray(Board start, byte[] targets)
        {
            if (targets[start.GetBoardNo()] != 0)
            {
                return targets[start.GetBoardNo()] - 1;
            }

            var node = new SearchNode(1, start);
            Queue<SearchNode> queue = new Queue<SearchNode>();
            var known = new byte[MAX];
            known[start.GetBoardNo()] = 1;
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in current.GetEdges())
                {
                    var index = edge.Board.GetBoardNo();
                    if (targets[index] != 0)
                    {
                        return targets[index] + edge.Gen - 2;
                    }

                    if (known[index] == 0)
                    {
                        known[index] = edge.Gen;

                        queue.Enqueue(edge);
                    }
                }
            }
            return 0;
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
            targets.Add(Board.Solved, 0);
            return SearchBFS(board, targets);
        }

    }
}
