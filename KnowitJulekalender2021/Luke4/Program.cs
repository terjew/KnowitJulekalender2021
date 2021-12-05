using System;
using System.Numerics;

namespace Luke4
{
    class Program
    {
        static void Main(string[] args)
        {
            var ysteps = new[] { 3,  3, 3,  6}; //loop starts at 3, repeats with size 15
            var xsteps = new[] { 5, 10, 5, 10}; //loop starts at 5, repeats with size 30
            var steps = BigInteger.Parse("100000000000000000079");
            steps -= 8; //to align with the start of the loops
            var fullcycles = steps / 45;
            var remainder = (int)(steps % 45);

            bool goingNorth = true;
            int segment = 0;
            int x = 0;
            int y = 0;
            while (remainder > 0)
            {
                if (goingNorth)
                {
                    var segmentSize = Math.Min(remainder, ysteps[segment]);
                    y += segmentSize;
                    remainder -= segmentSize;
                }
                else
                {
                    var segmentSize = Math.Min(remainder, xsteps[segment]);
                    x += segmentSize;
                    remainder -= segmentSize;
                    segment++;
                }
                goingNorth = !goingNorth;
            }

            var totaly = fullcycles * 15 + y + 3;
            var totalx = fullcycles * 30 + x + 5;
            Console.WriteLine($"{totalx},{totaly}");
        }

    }
}
