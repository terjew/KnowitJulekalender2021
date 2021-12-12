using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;

namespace Luke12
{
    class Entry
    {
        public bool IsGift { get; init; }
        public int Level { get; init; }
        public bool HasGift { get; set; }

        public static Entry Parse(string line)
        {
            int level = 0, i = 0;
            while (line[i] == '-')
            {
                level++;
                i++;
            }
            return new Entry()
            {
                IsGift = line[i] == 'G',
                Level = level,
            };
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            BenchmarkRunner.Run<Program>();
        }

        int populatedCategories;
        Stack<Entry> path;
        Entry current;
        
        [Benchmark]
        public int Solve()
        {
            var lines = File.ReadAllLines("task.txt");
            populatedCategories = 0;
            path = new Stack<Entry>();
            current = Entry.Parse(lines[0]);
            path.Push(current);
            for (int i = 1; i < lines.Length; i++)
            {
                var entry = Entry.Parse(lines[i]);
                PopToLevel(entry.Level);
                if (entry.IsGift)
                {
                    foreach (var node in path)
                    {
                        if (node.HasGift) break;
                        node.HasGift = true;
                    }
                }
                else
                {
                    current = entry;
                    path.Push(entry);
                }
            }
            PopToLevel(0);
            return populatedCategories;
        }

        private void PopToLevel(int level)
        {
            while (level <= current?.Level)
            {
                if (current.HasGift) populatedCategories++;
                path.Pop();
                path.TryPeek(out current);
            }
        }

    }
}
