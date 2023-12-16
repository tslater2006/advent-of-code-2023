using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            public List<(Point, Direction)> Path = new();
            public LightRay(Point position, Direction direction)
            {
                Position = position;
                Direction = direction;
            }

            public LightRay[] Split()
            {
                return null;
            }

            public void TurnCW()
            {
                Direction = (Direction)(((int)Direction + 1) % 4);
            }

            public void TurnCCW()
            {
                Direction = (Direction)(((int)Direction + 3) % 4);
            }

            public void Move()
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
                Path.Add((Position, Direction));
            }

        }

        char[,] Grid;
        public Day16()
        {
            var lines = File.ReadAllLines(InputFilePath);
            Grid = new char[lines[0].Length, lines.Length];
            for(var y = 0; y < lines.Length; y++)
            {
                for(var x = 0; x < lines[y].Length; x++)
                {
                    Grid[x, y] = lines[y][x];
                }
            }
        }

        private int GetEnergizedCellCount(Point position, Direction direction)
        {
            Dictionary<Point, int> visitCount = new();
            HashSet<(Point, Direction)> loopDetection = new();
            var startingRay = new LightRay(position, direction);
            Stack<LightRay> rays = new();
            rays.Push(startingRay);

            while (rays.Count > 0)
            {
                var ray = rays.Pop();

                while (true)
                {
                    ray.Move();
                    if (ray.Position.X < 0 || ray.Position.X >= Grid.GetLength(0) || ray.Position.Y < 0 || ray.Position.Y >= Grid.GetLength(1))
                    {
                        break;
                    }
                    if (loopDetection.Contains((ray.Position, ray.Direction)))
                    {
                        break;
                    }
                    loopDetection.Add((ray.Position, ray.Direction));

                    if (visitCount.ContainsKey(ray.Position))
                    {
                        visitCount[ray.Position]++;
                    }
                    else
                    {
                        visitCount[ray.Position] = 1;
                    }

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
                                var splitRay = new LightRay(ray.Position, ray.Direction);
                                splitRay.TurnCW();
                                rays.Push(splitRay);
                                ray.TurnCCW();
                            }
                            break;
                        case '-':
                            if (ray.Direction == Direction.EAST || ray.Direction == Direction.WEST)
                            {
                                continue;
                            }
                            else
                            {
                                var splitRay = new LightRay(ray.Position, ray.Direction);
                                splitRay.TurnCW();
                                rays.Push(splitRay);
                                ray.TurnCCW();
                            }
                            break;
                    }
                }
            }

            return visitCount.Keys.Count;
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = GetEnergizedCellCount(new Point(-1, 0), Direction.EAST);
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var highestCount = 0;

            /* top and bottom rows */
            for(var x = 0; x < Grid.GetLength(0); x++)
            {
                var count = GetEnergizedCellCount(new Point(x, -1), Direction.SOUTH);
                if (count > highestCount) { highestCount = count; }
                count = GetEnergizedCellCount(new Point(x, Grid.GetLength(1)), Direction.NORTH);
                if (count > highestCount) { highestCount = count; }
            }

            /* left and right columns*/
            for(var y = 0; y < Grid.GetLength(1); y++)
            {
                var count = GetEnergizedCellCount(new Point(-1, y), Direction.EAST);
                if (count > highestCount) { highestCount = count; }
                count = GetEnergizedCellCount(new Point(Grid.GetLength(0), y), Direction.WEST);
                if (count > highestCount) { highestCount = count; }
            }


            return new(highestCount.ToString());
        }
    }
}
