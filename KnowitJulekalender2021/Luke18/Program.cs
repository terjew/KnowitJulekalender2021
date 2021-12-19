using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Luke18
{
    public class Program
    {
        private Solver precached;

        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            Console.WriteLine(p.SolveUncached());
            Console.WriteLine(p.SolveInterleaved());
            Console.WriteLine(p.SolveInterleavedUncached());
            Console.WriteLine(p.SolveSplitWork());
            Console.WriteLine(p.SolveParallelUncached());
            BenchmarkRunner.Run<Program>();
        }

        public Program()
        {
            precached = new Solver();
            precached.CacheCollatz();
            precached.CacheNiklatz();
        }

        [Benchmark]
        public void CacheNiklatz()
        {
            new Solver().CacheNiklatz();
        }

        [Benchmark]
        public void CacheCollatz()
        {
            new Solver().CacheCollatz();
        }

        [Benchmark]
        public void CombineOnly()
        {
            CombineParallel(precached.CollatzCache, precached.NiklatzCache);
        }

        [Benchmark]
        public long Solve()
        {
            return new Solver().Solve(2, 1000000, 1);
        }

        [Benchmark]
        public long SolveUncached()
        {
            return new Solver().SolveUncached(2, 1000000, 1);
        }

        [Benchmark]
        public long SolveInterleaved()
        {
            long sum = 0;
            Parallel.For(0, 8, i =>
            {
                long part = new Solver().Solve(2 + i, 1000000, 8);
                Interlocked.Add(ref sum, part);
            });
            return sum;
        }

        [Benchmark]
        public long SolveInterleavedUncached()
        {
            var solver = new Solver();
            long sum = 0;
            Parallel.For(0, 8, i =>
            {
                long part = solver.SolveUncached(2 + i, 1000000, 8);
                Interlocked.Add(ref sum, part);
            });
            return sum;
        }

        [Benchmark]
        public long SolveParallelUncached()
        {
            var solver = new Solver();
            long total = 0;
            Parallel.For(2, 1000001, i =>
            {
                var collatz = solver.CollatzUncachedIterative(i);
                var (niklatz, sum) = solver.NiklatzUncachedIterative(i);
                if (niklatz != collatz) Interlocked.Add(ref total, sum);
            });
            return total;
        }

        [Benchmark]
        public long SolveSplitWork()
        {
            var niklatzSolver = new Solver();
            var collatzSolver = new Solver();
            Parallel.For(0, 2, i =>
            {
                if (i == 0) niklatzSolver.CacheNiklatz();
                else collatzSolver.CacheCollatz();
            });

            return CombineParallel(collatzSolver.CollatzCache, niklatzSolver.NiklatzCache);
        }

        public long CombineParallel(Dictionary<long, long> collatzCache, Dictionary<(long, long), (long, long)> niklatzCache)
        {
            long[] sums = new long[8];
            Parallel.For(0, 8, thread =>
            {
                for (int i = 2 + thread; i <= 1000000; i += 8)
                {
                    var (nik, sum) = niklatzCache[(i, 0)];
                    var col = collatzCache[i];
                    if (nik != col) sums[thread] += sum;
                }
            });

            long sum = 0;
            for (int i = 0; i < 8; i++) sum += sums[i];
            return sum;
        }
    }

    public class Solver
    {

        public Dictionary<long, long> CollatzCache = new Dictionary<long, long>();
        public Dictionary<(long, long), (long, long)> NiklatzCache = new Dictionary<(long, long), (long, long)>();

        public long SolveUncached(long start, long max, long step)
        {
            long total = 0;
            for (long i = start; i <= max; i += step)
            {
                var collatz = CollatzUncachedIterative(i);
                var (niklatz, sum) = NiklatzUncachedIterative(i);
                if (collatz != niklatz)
                {
                    total += sum;
                }
            }
            return total;
        }

        public long Solve(long start, long max, long step)
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

        public void CacheNiklatz()
        {
            for (long i = 2; i <= 1000000; i++) Niklatz(i,0);
        }

        public void CacheCollatz()
        {
            for (long i = 2; i <= 1000000; i++) Collatz(i);
        }

        public (long, long) NiklatzUncachedIterative(long start)
        {
            long i = start;
            long count = 0;
            long sum = start;
            int reverse = 0;
            while (i != 1)
            {
                var isEven = (i % 2 == 0);
                reverse = (i % 37 == 0) ? 3 : reverse;
                if (reverse > 0)
                {
                    isEven = !isEven;
                    reverse--;
                }
                i = isEven ? i / 2 : i * 3 + 1;
                count++;
                sum += i;
            }
            return (count + 1, sum);
        }

        public long CollatzUncachedIterative(long start)
        {
            long i = start;
            long count = 0;
            while (i != 1)
            {
                var isEven = (i % 2 == 0);
                i = isEven ? i / 2 : i * 3 + 1;
                count++;
            }
            return count + 1;
        }

        public (long, long) NiklatzUncachedRecursive(long i, long reverseStepsIn)
        {
            if (i == 1) return (1, 1);
            long next;
            var reverseSteps = (i % 37 == 0) ? 3 : reverseStepsIn;
            var isEven = (i % 2 == 0);
            if (reverseSteps > 0)
            {
                isEven = !isEven;
                reverseSteps--;
            }
            next = isEven ? i / 2 : i * 3 + 1;
            var (count, sum) = NiklatzUncachedRecursive(next, reverseSteps);
            count++;
            sum += i;
            return (count, sum);
        }

        public long CollatzUncachedRecursive(long i)
        {
            if (i == 1) return 1;
            long next = (i % 2 == 0) ? i / 2 : i * 3 + 1;
            long steps = 1 + CollatzUncachedRecursive(next);
            return steps;
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
