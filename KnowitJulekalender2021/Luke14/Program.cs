using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luke14
{
    public class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine(p.SolveRegex());
            BenchmarkRunner.Run<Program>();
        }

        static Regex regex = new Regex("(t.{1,5}r.{1,5}o.{1,5}l.{1,5}l)|(^[^n].*(n.{0,2}i.{0,2}s.{0,2}s.{0,2}e).*[^e]$)");

        [Benchmark]
        public int SolveRegex()
        {
            return File.ReadLines("ordliste.txt").AsParallel().Count(l => regex.IsMatch(l));
        }

    }
}
