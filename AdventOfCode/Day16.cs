using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    public enum Direction
    {
        NORTH, EAST, SOUTH, WEST
    }
    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.NORTH => Direction.SOUTH,
                Direction.EAST => Direction.WEST,
                Direction.SOUTH => Direction.NORTH,
                Direction.WEST => Direction.EAST,
                _ => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
            };
        }
        public static Direction TurnCW(this Direction direction)
        {
            return direction switch
            {
                Direction.NORTH => Direction.EAST,
                Direction.EAST => Direction.SOUTH,
                Direction.SOUTH => Direction.WEST,
                Direction.WEST => Direction.NORTH,
                _ => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
            };
        }
        public static Direction TurnCCW(this Direction direction)
        {
            return direction switch
            {
                Direction.NORTH => Direction.WEST,
                Direction.EAST => Direction.NORTH,
                Direction.SOUTH => Direction.EAST,
                Direction.WEST => Direction.SOUTH,
                _ => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
            };
        }
    }

    internal class Day16 : BaseDay
    {

        class LightRay
        {
            public Point StartPosition;
            public Direction StartDirection;
            public Point Position;
            public Direction Direction;
            public int Width;
            public int Height;
            public HashSet<Point> VisitedPoints;
            public HashSet<Point> PassthroughPipes;

            public void PrintVisitGrid()
            {
                for(var y = 0; y < Height; y++)
                {
                    for(var x = 0; x < Width; x++)
                    {
                        if (VisitedPoints.Contains(new Point(x, y)))
                        {
                            Console.Write('X');
                        } else
                        {
                            Console.Write('.');
                        }
                    }
                    Console.WriteLine();
                }   
            }

            public int GetVisitCount()
            {
                return VisitedPoints.Count;
            }

            public LightRay(Point position, Direction direction, int width, int height)
            {
                StartPosition = position;
                StartDirection = direction;
                Width = width;
                Height = height;
                VisitedPoints.Add(StartPosition);
            }

            public IEnumerable<LightRay> Split()
            {
                LightRay ray1 = new LightRay(Position, Direction.TurnCW(), Width, Height);
                yield return ray1;

                LightRay ray2 = new LightRay(Position, Direction.TurnCCW(), Width, Height);
                yield return ray2;
            }

            public bool Move()
            {
                if (Position.X == 0 || Position.X == Width - 1 || Position.Y == 0 || Position.Y == Height - 1)
                {
                    return false;
                }

                switch (Direction)
                {
                    case Direction.NORTH:
                        Position.Y--;
                        break;
                    case Direction.EAST:
                        Position.X++;
                        break;
                    case Direction.SOUTH:
                        Position.Y++;
                        break;
                    case Direction.WEST:
                        Position.X--;
                        break;
                }

                return VisitedPoints.Add(new Point(Position.X, Position.Y));
                
            }
        }


        char[,] Grid;
        int Width;
        int Height;

        public Day16()
        {
            var lines = File.ReadAllLines(InputFilePath);
            Width = lines[0].Length;
            Height = lines.Length;

            Grid = new char[lines[0].Length, lines.Length];
            for(var y = 0; y < lines.Length; y++)
            {
                for(var x = 0; x < lines[y].Length; x++)
                {
                    Grid[x, y] = lines[y][x];
                }
            }
        }
        Dictionary<Point, LightRay> SplitPointCache = new();

        private void RunRay(LightRay startRay, List<(Point,Direction)> LoopDetect)
        {
            var visited = new HashSet<(Point, Direction)>();
            var rays = new Queue<LightRay>();
            rays.Enqueue(startRay);

            while(rays.Count > 0)
            {
                var ray = rays.Dequeue(); ;
                if (!visited.Add((ray.Position, ray.Direction)))
                {
                    continue;
                }

                RunRaySegment(ray);
            }

        }

        private void RunRaySegment(LightRay ray)
        {
            
        }

        public override ValueTask<string> Solve_1()
        {
            var startRay = new LightRay(new Point(0, 0), Direction.EAST, Width, Height);
            RunRay(startRay, new());
            //startRay.PrintVisitGrid();
            var answer = startRay.GetVisitCount();
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            return new(maxVisitCount.ToString());
        }
    }
}
