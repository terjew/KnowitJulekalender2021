using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;

namespace Luke19
{
    class Gift
    {
        public string Type;
        public int ProduceDuration;
        public int WrapDuration;
    }

    class Machine
    {
        public DateTime StartTime;
        public Gift[] GiftsProduced;
        public static Machine Parse(string str)
        {
            var m = new Machine();
            m.StartTime = DateTime.Parse(str.Substring(0, 5));
            var tokens = str.Substring(7).Split(", ").ToArray();
            m.GiftsProduced = new Gift[tokens.Length / 3];
            for (int i = 0; i < tokens.Length / 3; i++)
            {
                m.GiftsProduced[i] = new Gift()
                {
                    //Type = tokens[i * 3],
                    ProduceDuration = int.Parse(tokens[i * 3 + 1]),
                    WrapDuration = int.Parse(tokens[i * 3 + 2]),
                };
            }
            return m;
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.Solve());
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public int Solve()
        {
            var machines = File.ReadLines("factory.txt").Select(l => Machine.Parse(l));
            int[] wrapping = new int[2880];
            foreach (var machine in machines)
            {
                int time = (int)(machine.StartTime - machine.StartTime.Date).TotalMinutes;
                foreach (var gift in machine.GiftsProduced)
                {
                    time += gift.ProduceDuration;
                    for (int t = 0; t < gift.WrapDuration; t++) wrapping[time + t]++;
                }
            }
            return wrapping.Max();
        }
    }
}
