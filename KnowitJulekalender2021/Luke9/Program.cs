using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Utilities;

namespace Luke9
{
    public class Program
    {
        record YearlyNumbers(int Year, long GiftsRemaining, long NiceKids);
        YearlyNumbers[] years = new[]
        {
            new YearlyNumbers(2019,1854803357,2424154637),
            new YearlyNumbers(2020,2787141611,2807727397),
            new YearlyNumbers(2021,1159251923,2537380333),
        };

        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine($"X (Seek): {p.SolveSeek()}");
            Console.WriteLine($"X (CRT): {p.SolveCRT()}");
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public long SolveSeek()
        {
            Dictionary<long, int> candidates = new Dictionary<long, int>();
            for (int giftsPerKid = 1; giftsPerKid <= 2000; giftsPerKid++)
            {
                for (int i = 0; i < years.Length; i++)
                {
                    var handedOut = giftsPerKid * years[i].NiceKids;
                    var remainder = years[i].GiftsRemaining;
                    var sum = handedOut + remainder;
                    if (candidates.ContainsKey(sum))
                    {
                        var count = ++candidates[sum];
                        if (count == 3) return sum;
                    }
                    else candidates.Add(sum, 1);
                }
            }
            return -1;
        }

        [Benchmark]
        public long SolveCRT()
        {
            var num = years.Select(y => (BigInteger)y.NiceKids).ToArray();
            var rem = years.Select(y => (BigInteger)y.GiftsRemaining).ToArray();
            BigInteger x = ChineseRemainderTheorem.Solve(num, rem);
            return (long)x;
        }

        
    }
}
