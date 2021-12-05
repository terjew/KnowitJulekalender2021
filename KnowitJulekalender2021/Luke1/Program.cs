using System;
using System.IO;

namespace Luke1
{
    class Program
    {
        static readonly string[] ones = new[] { "null", "en", "to", "tre", "fire", "fem", "seks", "sju", "åtte", "ni", "ti", "elleve", "tolv", "tretten", "fjorten", "femten", "seksten", "sytten", "atten", "nitten" };
        static readonly string[] tens = new[] { "", "", "tjue", "tretti", "førti", "femti" };

        static void Main(string[] args)
        {
            var sum = SumInts(File.ReadAllLines("tall.txt")[0]);
            Console.WriteLine(sum);
        }

        static long SumInts(string s)
        {
            int pos = 0;
            long sum = 0;
            while (pos < s.Length)
            {
                int number = ParseIntAt(s, ref pos);
                sum += number;
            }
            return sum;
        }

        static int ParseIntAt(string s, ref int pos)
        {
            for (int i = 19; i >= 10; i--)
            {
                var digit = ones[i];
                if (pos + digit.Length <= s.Length && s.Substring(pos, digit.Length) == digit)
                {
                    pos += digit.Length;
                    return i;
                }
            }

            for (int i = 5; i >= 2; i--)
            {
                var digit = tens[i];
                if (pos + digit.Length <= s.Length && s.Substring(pos, digit.Length) == digit)
                {
                    pos += digit.Length;
                    return i * 10;
                }
            }

            for (int i = 9; i > 0; i--)
            {
                var digit = ones[i];
                if (pos + digit.Length <= s.Length && s.Substring(pos, digit.Length) == digit)
                {
                    pos += digit.Length;
                    return i;
                }
            }

            return 0;
        }
    }
}
