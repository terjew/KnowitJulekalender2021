using System;
using System.IO;
using System.Linq;

namespace Luke6
{
    record Package(int Startpos, int Length);

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("pakker.txt");
            //var lines = new[] { "0,6", "0,1", "4,3", "2,4", "0,5" };
            var packages = lines
                .Select(l => l
                    .Split(",")
                    .Select(s => int.Parse(s))
                    .ToArray())
                .Select(a => new Package(a[0], a[1]))
                .ToArray();
            var maxPos = packages.Max(p => p.Startpos + p.Length);
            var count = packages.Length;
            var stack = new byte[count, maxPos];
            int dropped = 0;
            int height = 0;
            foreach (var package in packages) dropped += DropPackage(stack, package, ref height);
            Console.WriteLine($"{dropped} dropped");

            for (int y = height; y >= 0; y--)
            {
                for (int x = 0; x < maxPos; x++)
                {
                    Console.Write(stack[y, x] == 0 ? ".." : "[]");
                }
                Console.WriteLine();
            }
        }

        static int DropPackage(byte[,] stack, Package package, ref int height)
        {
            bool supportedLeft = false;
            bool supportedRight = false;
            int endpos = package.Startpos + package.Length;
            float midpos = package.Startpos + (package.Length / 2.0f);
            for (int y = height - 1; y >= -1; y--)
            {
                for (int x = package.Startpos; x < endpos; x++)
                {
                    if (y == -1 || stack[y,x] != 0)
                    {
                        if (x < midpos) supportedLeft = true;
                        if (x > midpos - 1) supportedRight = true;
                    }
                }
                if (supportedLeft || supportedRight)
                {
                    var dropped = supportedLeft != supportedRight;
                    if (!dropped)
                    {
                        for (int x = package.Startpos; x < endpos; x++)
                        {
                            stack[y + 1, x] = 1;
                        }
                        if (y == height - 1) height++;
                        return 0;
                    }
                    return 1;
                }
            }
            throw new InvalidOperationException();
        }
    }
}
