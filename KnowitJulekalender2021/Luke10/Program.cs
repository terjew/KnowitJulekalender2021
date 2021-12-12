using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luke10
{
    class Node
    {
        public char Name { get; init; }
        public Dictionary<Node,Node> Occluded { get; set; }
    }
    public class Program
    {
        private Node[] nodes;

        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public int Solve()
        {
            Node
               a = new Node() { Name = 'a' },
               b = new Node() { Name = 'b' },
               c = new Node() { Name = 'c' },
               d = new Node() { Name = 'd' },
               e = new Node() { Name = 'e' },
               f = new Node() { Name = 'f' },
               g = new Node() { Name = 'g' },
               h = new Node() { Name = 'h' },
               i = new Node() { Name = 'i' };
            a.Occluded = new Dictionary<Node, Node>() { { c, b }, { i, e }, { g, d } };
            b.Occluded = new Dictionary<Node, Node>() { { h, e } };
            c.Occluded = new Dictionary<Node, Node>() { { a, b }, { g, e }, { i, f } };
            d.Occluded = new Dictionary<Node, Node>() { { f, e } };
            e.Occluded = new Dictionary<Node, Node>();
            f.Occluded = new Dictionary<Node, Node>() { { d, e } };
            g.Occluded = new Dictionary<Node, Node>() { { a, d }, { c, e }, { i, h } };
            h.Occluded = new Dictionary<Node, Node>() { { b, e } };
            i.Occluded = new Dictionary<Node, Node>() { { g, h }, { a, e }, { c, f } };
            nodes = new[] { a, b, c, d, e, f, g, h, i };
            return CountPaths(nodes.Single(n => n.Name == 'd'), new HashSet<Node>(), 8);
        }

        private int CountPaths(Node node, HashSet<Node> visited, int max)
        {
            if (visited.Count == max - 1) return 1;
            visited.Add(node);
            int count = 0;
            foreach(var next in nodes)
            {
                if (visited.Contains(next)) continue;
                if (node.Occluded.ContainsKey(next) && !visited.Contains(node.Occluded[next])) continue;
                count += CountPaths(next, visited, max);
            }
            visited.Remove(node);
            return count;
        }
    }
}
