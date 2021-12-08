using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Luke8
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public void Solve()
        {
            var lines = File.ReadAllLines("input.txt");
            int numCoords = 200;
            var coords = lines.Take(numCoords).Select(l =>
            {
                var pair = l.Trim('(', ')').Split(',').Select(s => int.Parse(s)).ToArray();
                return new Point(pair[0], pair[1]);
            }).ToArray();
            var route = lines.Skip(numCoords).Select(l => int.Parse(l)).ToArray();
            var maxX = coords.Max(c => c.X);
            var maxY = coords.Max(c => c.Y);
            var arr = new int[maxX + 1, maxY + 1];
            TravelRoute(arr, coords, route);
            var rect = FindMaxRect(arr);
            Console.WriteLine($"{rect.Left},{rect.Top} {rect.Right},{rect.Bottom}");

        }
        private Rectangle FindMaxRect(int[,] arr)
        {
            var dimx = arr.GetLength(0);
            var dimy = arr.GetLength(1);
            Rectangle bounds = Rectangle.Empty;
            var maxValue = arr.Cast<int>().Max();
            for (int x = 0; x < dimx; x++)
            {
                for (int y = 0; y < dimy; y++)
                {
                    if (arr[x,y] == maxValue)
                    {
                        if (bounds.IsEmpty) bounds = new Rectangle(x, y, 0, 0);
                        else bounds = Rectangle.Union(bounds, new Rectangle(x, y, 0, 0));
                    }
                }
            }
            return bounds;
        }

        private void TravelRoute(int[,] arr, Point[] coords, int[] route)
        {
            Point current = coords[route[0]];
            for (int i = 1; i < route.Length; i++)
            {
                int fromX, toX, fromY, toY;
                var next = coords[route[i]];
                if (current.X < next.X)
                {
                    fromX = current.X;
                    toX = next.X;
                }
                else
                {
                    fromX = next.X + 1;
                    toX = current.X + 1;
                }
                if (current.Y < next.Y)
                {
                    fromY = current.Y;
                    toY = next.Y;
                }
                else
                {
                    fromY = next.Y + 1;
                    toY = current.Y + 1;
                }
                for (int x = fromX; x < toX; x++)
                {
                    for (int y = fromY; y < toY; y++)
                    {
                        arr[x, y]++;
                    }
                }
                current = next;
            }
        }
    }
}
