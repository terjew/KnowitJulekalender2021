using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luke18
{
    public class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public long Solve()
        {
            return new Solver().SolvePartition(2, 1000000, 1);
        }

        [Benchmark]
        public async Task<long> SolveThreadedInterleaved()
        {
            var tasks = Enumerable.Range(0, 4).Select(i => Task.Run(() => {
                return new Solver().SolvePartition(2 + i, 1000000, 4);
            }));
            var sums = await Task.WhenAll(tasks);
            return sums.Sum();
        }

        [Benchmark]
        public async Task<long> SolveThreadedBlocks()
        {
            var tasks = Enumerable.Range(0, 4).Select(i => Task.Run(() => {
                long start = Math.Max(i * 250000, 2);
                long max = (i + 1) * 250000;
                long step = 1;
                return new Solver().SolvePartition(start, max, step);
            }));
            var sums = await Task.WhenAll(tasks);
            return sums.Sum();
        }

    }

    public class Solver { 

        public Dictionary<long, long> CollatzCache = new Dictionary<long, long>();
        public Dictionary<(long, long), (long, long)> NiklatzCache = new Dictionary<(long, long), (long, long)>();

        public long SolvePartition(long start, long max, long step)
        {
            long total = 0;
            for (long i = start; i <= max; i += step)
            {
                var collatz = Collatz(i);
                var (niklatz, sum) = Niklatz(i, 0);
                if (collatz != niklatz)
                {
                    total += sum;
                }
            }
            return total;
        }

        public (long, long) Niklatz(long i, long reverseStepsIn)
        {
            if (i == 1) return (1, 1);
            if (!NiklatzCache.ContainsKey((i, reverseStepsIn)))
            {
                long next;
                var reverseSteps = (i % 37 == 0) ? 3 : reverseStepsIn;
                var isEven = (i % 2 == 0);
                if (reverseSteps > 0)
                {
                    isEven = !isEven;
                    reverseSteps--;
                }
                next = isEven ? i / 2 : i * 3 + 1;
                var (count, sum) = Niklatz(next, reverseSteps);
                count++;
                sum += i;
                NiklatzCache[(i, reverseStepsIn)] = (count, sum);
            }
            return NiklatzCache[(i, reverseStepsIn)];
        }

        public long Collatz(long i)
        {
            if (i == 1) return 1;
            if (!CollatzCache.ContainsKey(i))
            {
                long next = (i % 2 == 0) ? i / 2 : i * 3 + 1;
                long steps = 1 + Collatz(next);
                CollatzCache[i] = steps;
            }
            return CollatzCache[i];
        }

    }
}
