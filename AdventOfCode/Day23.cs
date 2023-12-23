using AdventOfCode.Common;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    /* Implementation based on the wonderful explanation from HyperNeutrino: https://www.youtube.com/watch?v=NTLYL7Mg2jU */
    internal class Day23 : BaseDay
    {
        char[,] Grid = null;
        Dictionary<Point, Dictionary<Point, int>> Graph = new();
        int Width;
        int Height;
        Point start = new(1, 0);
        Point end = new(0, 0);
        public Day23()
        {
            var lines = File.ReadAllLines(InputFilePath);
            Grid = new char[lines.Length, lines[0].Length];
            Width = lines[0].Length;
            Height = lines.Length;
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    Grid[y, x] = lines[y][x];
                }
            }

            end = new(Grid.GetLength(1) - 2, Grid.GetLength(0) - 1);
        }

        HashSet<Point> seen = new();

        double dfs(Point p)
        {
            if (p == end) return 0;

            double max = double.NegativeInfinity;

            seen.Add(p);
            foreach (var pt in Graph[p])
            {
                if (seen.Contains(pt.Key)) continue;
                max = Math.Max(max, dfs(pt.Key) + pt.Value);
            }
            seen.Remove(p);
            return max;
        }

        public override ValueTask<string> Solve_1()
        {
            List<Point> branchPoints = new();
            branchPoints.Add(start);
            branchPoints.Add(end);
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (Grid[y, x] == '#') continue;
                    var currentPoint = new Point(x, y);
                    var neighborCount = 0;
                    foreach (var n in currentPoint.GetOrthoganalNeighbors(new(0, 0, Width, Height)))
                    {
                        if (Grid[n.Y, n.X] != '#') neighborCount++;
                    }
                    if (neighborCount >= 3)
                    {
                        branchPoints.Add(currentPoint);
                    }
                }
            }

            /* create directed graph */
            Stack<(Point, int)> stack = new();
            Dictionary<char, (int deltaX, int deltaY)> directions = new()
            {
                ['>'] = (1, 0),
                ['<'] = (-1, 0),
                ['^'] = (0, -1),
                ['v'] = (0, 1)
            };
            foreach (var branchPoint in branchPoints)
            {
                stack.Push((branchPoint, 0));
                var seen = new HashSet<Point>();
                seen.Add(branchPoint);
                while (stack.Count > 0)
                {
                    var (point, distance) = stack.Pop();

                    if (distance != 0 && branchPoints.Contains(point))
                    {
                        if (!Graph.ContainsKey(branchPoint))
                        {
                            Graph[branchPoint] = new();
                        }
                        Graph[branchPoint][point] = distance;
                        continue;
                    }

                    if (Grid[point.Y, point.X] == '.')
                    {
                        foreach (var neighbor in point.GetOrthoganalNeighbors(new(0, 0, Width, Height)))
                        {
                            if (Grid[neighbor.Y, neighbor.X] == '#') continue;
                            if (!seen.Contains(neighbor))
                            {
                                seen.Add(neighbor);
                                stack.Push((neighbor, distance + 1));
                            }
                        }
                        continue;
                    }
                    else
                    {
                        var direction = directions[Grid[point.Y, point.X]];
                        var newPoint = new Point(point.X + direction.deltaX, point.Y + direction.deltaY);
                        if (Grid[newPoint.Y, newPoint.X] == '#') continue;
                        if (!seen.Contains(newPoint))
                        {
                            seen.Add(newPoint);
                            stack.Push((newPoint, distance + 1));
                        }
                    }
                }
            }

            var answer = dfs(start);

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            Graph = new();
            List<Point> branchPoints = new();
            branchPoints.Add(start);
            branchPoints.Add(end);
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (Grid[y, x] == '#') continue;
                    var currentPoint = new Point(x, y);
                    var neighborCount = 0;
                    foreach (var n in currentPoint.GetOrthoganalNeighbors(new(0, 0, Width, Height)))
                    {
                        if (Grid[n.Y, n.X] != '#') neighborCount++;
                    }
                    if (neighborCount >= 3)
                    {
                        branchPoints.Add(currentPoint);
                    }
                }
            }

            /* create directed graph */
            Stack<(Point, int)> stack = new();
            foreach (var branchPoint in branchPoints)
            {
                stack.Push((branchPoint, 0));
                var seen = new HashSet<Point>();
                seen.Add(branchPoint);
                while (stack.Count > 0)
                {
                    var (point, distance) = stack.Pop();

                    if (distance != 0 && branchPoints.Contains(point))
                    {
                        if (!Graph.ContainsKey(branchPoint))
                        {
                            Graph[branchPoint] = new();
                        }
                        Graph[branchPoint][point] = distance;
                        continue;
                    }
                    foreach (var neighbor in point.GetOrthoganalNeighbors(new(0, 0, Width, Height)))
                    {
                        if (Grid[neighbor.Y, neighbor.X] == '#') continue;
                        if (!seen.Contains(neighbor))
                        {
                            seen.Add(neighbor);
                            stack.Push((neighbor, distance + 1));
                        }
                    }
                    continue;
                }
            }
            var answer = dfs(start);
            return new(answer.ToString());
        }
    }
}
