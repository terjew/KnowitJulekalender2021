using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Luke20
{
    public static class MatchExtensions
    {
        public static int GetInt(this Match m, string group)
        {
            return int.Parse(m.Groups[group].Value);
        }
        public static bool GetBool(this Match m, string group)
        {
            return int.Parse(m.Groups[group].Value) == 1;
        }
    }

    enum Direction { N, E, S, W}

    class Room
    {
        public bool[] Directions;
        private static Regex regex = new Regex(@"\((?<N>\d),(?<E>\d),(?<S>\d),(?<W>\d)\)");
        public static IEnumerable<Room> Parse(string line)
        {
            var matches = regex.Matches(line);
            return matches.Cast<Match>().Select(m =>
            {
                return new Room
                {
                    Directions = new[]
                    {
                        m.GetBool("N"),
                        m.GetBool("E"),
                        m.GetBool("S"),
                        m.GetBool("W"),
                    }
                };
            });
        }

    }

    class Maze
    {
        public Room[,] Rooms; //y,x
        public int Width;
        public int Height;

        public static Maze Parse(string[] lines)
        {
            var height = lines.Count(l => l.StartsWith("("));
            var width = lines[0].Count(c => c == '(');
            var rooms = new Room[height, width];
            for (int y = 0; y < height; y++)
            {
                var line = Room.Parse(lines[y]).ToArray();
                for (int x = 0; x < width; x++)
                {
                    rooms[y, x] = line[x];
                }
            }
            return new Maze { Rooms = rooms, Width = width, Height = height };
        }

        public Room Get(Point pos)
        {
            return Rooms[pos.Y, pos.X];
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
            var lines = File.ReadAllLines("maze.txt");
            var maze = Maze.Parse(lines);
            var solver = new LeftHandSolver();
            return solver.Solve(maze);
        }
    }

    class LeftHandSolver
    {
        Maze maze;
        Room room;
        Point pos;
        Point target;
        int orientation;

        public int Solve(Maze m)
        {
            maze = m;
            pos = new Point(0, 0);
            target = new Point(m.Width - 1, m.Height - 1);
            orientation = 2;
            room = m.Get(pos);
            int count = 0;
            while (pos != target)
            {
                if (Left()) 
                { 
                    TurnLeft(); 
                    MoveForward(); 
                }
                else if (Straight()) 
                { 
                    MoveForward(); 
                }
                else if (Right()) 
                { 
                    TurnRight(); 
                    MoveForward(); 
                }
                else 
                { 
                    TurnAround(); 
                    MoveForward(); 
                }
                count++;
            }
            return count;
        }

        int GetDirection(Direction direction)
        {
            int dir = (int)direction + orientation;
            return dir % 4;
        }

        private bool Left()
        {
            return room.Directions[GetDirection(Direction.W)];
        }

        private bool Right()
        {
            return room.Directions[GetDirection(Direction.E)];
        }

        private bool Straight()
        {
            return room.Directions[GetDirection(Direction.N)];
        }

        private void TurnLeft()
        {
            orientation --;
            if (orientation < 0) orientation += 4;
        }

        private void TurnRight()
        {
            orientation++;
            if (orientation > 3) orientation -= 4;
        }

        private void TurnAround()
        {
            TurnRight();
            TurnRight();
        }

        private void MoveForward()
        {
            Point[] moves = new Point[]
            {
                new Point(0,-1), new Point(1, 0), new Point(0, 1), new Point(-1, 0)
            };
            var offset = moves[orientation];
            pos = new Point(pos.X + offset.X, pos.Y + offset.Y);
            room = maze.Get(pos);
        }

    }
}
