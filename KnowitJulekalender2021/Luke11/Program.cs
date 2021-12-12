using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Luke11
{
    record NameInfo(int length, Regex[] regexes);
    public class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            var k = p.Solve();
            Console.WriteLine(k);
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public (string, int) Solve()
        {
            var names = File.ReadLines("names.txt")
                .ToDictionary(n => n, n => new NameInfo(n.Length, Permutations(n)
                                                                    .Select(p => ToRegex(p))
                                                                    .ToArray()));
            var lines = File.ReadLines("locked.txt");

            var bestKid = lines
                .AsParallel()
                .Select(l => DecodeKid(l, names))
                .Where(name => name != null)
                .GroupBy(name => name)
                .OrderByDescending(grp => grp.Count())
                .First();

            return (bestKid.Key, bestKid.Count());
        }

        private string DecodeKid(string line, Dictionary<string, NameInfo> names)
        {
            var matches = names
                    .Select(kvp => new { Name = kvp.Key, Score = BestMatch(line, kvp.Value) })
                    .Where(p => p.Score != int.MaxValue)
                    .OrderBy(p => p.Score)
                    .ToArray();
            if (matches.Length == 0) return null;
            var first = matches[0];
            if (matches.Length == 1 || matches[1].Score > first.Score)
            {
                return first.Name;
            }
            return null;
        }

        private static IEnumerable<string> Permutations(string name)
        {
            yield return name;
            for (int i = 0; i < name.Length - 1; i++)
            {
                int j = i + 1;
                var sb = new StringBuilder(name);
                sb[j] = name[i];
                sb[i] = name[j];
                yield return sb.ToString();
            }
        }

        private static Regex ToRegex(string permutation)
        {
            var reString = string.Join(".*?", permutation.ToCharArray());
            return new Regex(reString);
        }

        private static int BestMatch(string line, NameInfo nameInfo)
        {
            int min = int.MaxValue;
            foreach (var re in nameInfo.regexes)
            {
                var match = re.Match(line);
                if (match.Success && match.Length < min) min = match.Length - nameInfo.length;
            }
            return min;
        }

    }
}
