using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;

namespace Luke7
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public int SolveCmInt()
        {
            long length = 20;
            double pos = 1;
            while (pos < length)
            {
                pos += 20 * pos / length + 1;
                length += 20;
            }
            return (int)(length / 100);
        }

        [Benchmark]
        public int SolveCmDouble()
        {
            double length = 20;
            double pos = 1;
            while (pos < length)
            {
                pos += 20 * pos / length + 1;
                length += 20;
            }
            return (int)(length / 100);
        }

        [Benchmark]
        public int Solve5xInt()
        {
            int length = 1;
            double pos = 0.05;
            while (pos < length)
            {
                pos += pos / length + 0.05;
                length ++;
            }
            return (int)(length / 5);
        }

        [Benchmark]
        public int Solve5xDouble()
        {
            double length = 1;
            double pos = 0.05;
            while (pos < length)
            {
                pos += pos / length + 0.05;
                length += 1;
            }
            return (int)(length / 5);
        }

    }
}
