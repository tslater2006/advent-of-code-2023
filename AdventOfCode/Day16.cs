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

    internal class Day16 : BaseDay
    {
        enum Direction
        {
            NORTH, EAST, SOUTH, WEST
        }

        class LightRay
        {
            public Point Position;
            public Direction Direction;
            public int Width;
            public int Height;
            bool[,] VisitGrid;

            public void PrintVisitGrid()
            {
                for(var y = 0; y < Height; y++)
                {
                    for(var x = 0; x < Width; x++)
                    {
                        Console.Write(VisitGrid[x, y] ? 'X' : '.');
                    }
                    Console.WriteLine();
                }   
            }

            public int GetVisitCount()
            {
                var count = 0;
                for(var x = 0; x < Width; x++)
                {
                    for(var y = 0; y < Height; y++)
                    {
                        if (VisitGrid[x, y])
                        {
                            count++;
                        }
                    }
                }

                return count;
            }

            public LightRay(Point position, Direction direction, int width, int height)
            {
                Position = position;
                Direction = direction;
                Width = width;
                Height = height;
                VisitGrid = new bool[width, height];
            }

            public IEnumerable<LightRay> Split()
            {
                LightRay ray1 = new LightRay(Position, Direction, Width, Height);
                ray1.TurnCW();
                yield return ray1;

                LightRay ray2 = new LightRay(Position, Direction, Width, Height);
                ray2.TurnCCW();
                yield return ray2;
            }

            public void TurnCW()
            {
                Direction = (Direction)(((int)Direction + 1) % 4);
            }

            public void TurnCCW()
            {
                Direction = (Direction)(((int)Direction + 3) % 4);
            }

            public bool Move()
            {
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

                if(Position.X < 0 || Position.X >= Width || Position.Y < 0 || Position.Y >= Height)
                {
                    return false;
                } else
                {
                    VisitGrid[Position.X, Position.Y] = true;
                    return true;
                }
            }

            public void MergeVisitsFrom(LightRay other)
            {
                for(var x = 0; x < Width; x++)
                {
                    for(var y = 0; y < Height; y++)
                    {
                        if (!VisitGrid[x, y])
                        {
                            VisitGrid[x, y] = other.VisitGrid[x, y];
                        }
                    }
                }
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

        private void RunRay(LightRay ray, List<(Point,Direction)> LoopDetect)
        {
            while (ray.Move())
            {
                switch (Grid[ray.Position.X, ray.Position.Y])
                {
                    case '.':
                        continue;
                    case '/':
                        if (ray.Direction == Direction.EAST || ray.Direction == Direction.WEST)
                        {
                            ray.TurnCCW();
                        }
                        else
                        {
                            ray.TurnCW();
                        }
                        break;
                    case '\\':
                        if (ray.Direction == Direction.EAST || ray.Direction == Direction.WEST)
                        {
                            ray.TurnCW();
                        }
                        else
                        {
                            ray.TurnCCW();
                        }
                        break;
                    case '|':
                        if (ray.Direction == Direction.NORTH || ray.Direction == Direction.SOUTH)
                        {
                            continue;
                        }
                        else
                        {
                            if (LoopDetect.Contains((ray.Position,ray.Direction)))
                            {
                                return;
                            }
                            else
                            {
                                LoopDetect.Add((ray.Position, ray.Direction));
                                if (SplitPointCache.ContainsKey(ray.Position))
                                {
                                    ray.MergeVisitsFrom(SplitPointCache[ray.Position]);
                                    return;
                                }
                                else
                                {
                                    foreach (var r in ray.Split())
                                    {
                                        RunRay(r, LoopDetect);
                                        ray.MergeVisitsFrom(r);
                                    }

                                    if (SplitPointCache.ContainsKey(ray.Position))
                                    {
                                        SplitPointCache[ray.Position].MergeVisitsFrom(ray);
                                    }
                                    else
                                    {
                                        SplitPointCache.Add(ray.Position, ray);
                                    }

                                    return;
                                }
                            }
                        }
                    case '-':
                        if (ray.Direction == Direction.EAST || ray.Direction == Direction.WEST)
                        {
                            continue;
                        }
                        else
                        {
                            if (LoopDetect.Contains((ray.Position, ray.Direction)))
                            {
                                return;
                            }
                            else
                            {
                                LoopDetect.Add((ray.Position, ray.Direction));
                                if (SplitPointCache.ContainsKey(ray.Position))
                                {
                                    ray.MergeVisitsFrom(SplitPointCache[ray.Position]);
                                    return;
                                }
                                else
                                {
                                    foreach (var r in ray.Split())
                                    {
                                        RunRay(r, LoopDetect);
                                        ray.MergeVisitsFrom(r);
                                    }

                                    if (SplitPointCache.ContainsKey(ray.Position))
                                    {
                                        SplitPointCache[ray.Position].MergeVisitsFrom(ray);
                                    }
                                    else
                                    {
                                        SplitPointCache.Add(ray.Position, ray);
                                    }
                                    return;
                                }
                            }
                        }
                }
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var startRay = new LightRay(new Point(-1, 0), Direction.EAST, Width, Height);
            RunRay(startRay, new());
            //startRay.PrintVisitGrid();
            var answer = startRay.GetVisitCount();
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var rayCount = 0;

            /* start a ray from each point along the border of the grid and find which one returns the largest visit count */
            var maxVisitCount = 0;
            for(var x = 0; x < Width; x++)
            {
                var ray = new LightRay(new Point(x, -1), Direction.SOUTH, Width, Height);
                RunRay(ray, new());
                maxVisitCount = Math.Max(maxVisitCount, ray.GetVisitCount());
                Console.WriteLine();
                ray.PrintVisitGrid();

                ray = new LightRay(new Point(x, Height), Direction.NORTH, Width, Height);
                RunRay(ray, new());
                maxVisitCount = Math.Max(maxVisitCount, ray.GetVisitCount());
                Console.WriteLine();
                ray.PrintVisitGrid();
                rayCount += 2;
            }

            for (var y = 0; y < Height; y++)
            {
                var ray = new LightRay(new Point(-1, y), Direction.EAST, Width, Height);
                RunRay(ray, new());
                maxVisitCount = Math.Max(maxVisitCount, ray.GetVisitCount());
                Console.WriteLine();
                ray.PrintVisitGrid();
                ray = new LightRay(new Point(Width, y), Direction.WEST, Width, Height);
                RunRay(ray, new());
                maxVisitCount = Math.Max(maxVisitCount, ray.GetVisitCount());
                Console.WriteLine();
                ray.PrintVisitGrid();
                rayCount += 2;
            }


            return new(maxVisitCount.ToString());
        }
    }
}
