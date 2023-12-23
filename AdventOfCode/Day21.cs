using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day21 : BaseDay
    {
        Dictionary<Point, char> Map = new();
        Point StartLocation = new(0, 0);
        int Size; // Assumes grid is square...
        int Part1Steps = 64;
        int Part2Steps = 26501365;
        public Day21()
        {
            var lines = File.ReadAllLines(InputFilePath);
            Size = lines.Length;
            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == 'S')
                    {
                        StartLocation = new Point(x, y);
                        Map[new Point(x, y)] = '.'; // replace S with .
                    }
                    else
                    {
                        Map[new Point(x, y)] = line[x];
                    }
                }
            }
        }

        public int FindGardensAtStepCount(Point start, int stepCount)
        {
            List<Point> foundPoints = new();
            HashSet<Point> seenPoints = new();
            Queue<(Point, int)> pointsToCheck = new();
            Dictionary<Point, int> pointToSteps = new();
            pointsToCheck.Enqueue((start, 0));

            while(pointsToCheck.Count > 0)
            {
                var (point, steps) = pointsToCheck.Dequeue();
                if (seenPoints.Contains(point))
                    continue;
                seenPoints.Add(point);
                if (steps <= stepCount) {
                    pointToSteps.Add(point, steps);
                }
                foreach (var p in point.GetOrthoganalNeighbors())
                {
                    if (Map.ContainsKey(p) && Map[p] == '.')
                        pointsToCheck.Enqueue((p, steps + 1));
                }

                if (steps == stepCount || (steps < stepCount && (stepCount - steps) % 2 == 0))
                {
                    if (Map[point] == '.')
                        foundPoints.Add(point);
                }

            }
            return foundPoints.Count;
        }

        public override ValueTask<string> Solve_1()
        {
            var ans = FindGardensAtStepCount(StartLocation, Part1Steps);
            return new(ans.ToString());
            
        }

        public override ValueTask<string> Solve_2()
        {
            /* Part 2 implementation based on HyperNeutrino's video here: https://www.youtube.com/watch?v=9UOMZSL0JTg */
            var gridWidth = (Part2Steps / Size) - 1;
            var odd = Math.Pow((gridWidth / 2) * 2 + 1, 2);
            var even = Math.Pow(((gridWidth + 1) / 2 * 2), 2);

            var oddPoints = FindGardensAtStepCount(StartLocation, Size * 2 + 1);
            var evenPoints = FindGardensAtStepCount(StartLocation, Size * 2);
            double ans = odd * oddPoints + even * evenPoints;

            var cornerT = FindGardensAtStepCount(new Point(Size - 1, StartLocation.Y), Size - 1);
            var cornerR = FindGardensAtStepCount(new Point(StartLocation.X, 0), Size - 1);
            var cornerB = FindGardensAtStepCount(new Point(0, StartLocation.Y), Size - 1);
            var cornerL = FindGardensAtStepCount(new Point(StartLocation.X, Size - 1), Size - 1);
            ans += cornerT + cornerR + cornerB + cornerL;

            var smallTR = FindGardensAtStepCount(new Point(Size - 1, 0), (Size / 2) - 1);
            var smallTL = FindGardensAtStepCount(new Point(Size - 1, Size - 1), (Size / 2) - 1);
            var smallBR = FindGardensAtStepCount(new Point(0, 0), (Size / 2) - 1);
            var smallBL = FindGardensAtStepCount(new Point(0, Size - 1), (Size / 2) - 1);
            ans += (ulong)(gridWidth + 1) * (ulong)(smallTR + smallBR + smallBL + smallTL);

            var largeTR = FindGardensAtStepCount(new Point(Size - 1, 0), (Size * 3 / 2) - 1);
            var largeTL = FindGardensAtStepCount(new Point(Size - 1, Size - 1), (Size * 3 / 2) - 1);
            var largeBR = FindGardensAtStepCount(new Point(0, 0), (Size * 3 / 2) - 1);
            var largeBL = FindGardensAtStepCount(new Point(0, Size - 1), (Size * 3 / 2) - 1);
            ans += (ulong)(gridWidth) * (ulong)(largeTR + largeBR + largeBL + largeTL);

            return new(ans.ToString());
        }
    }
}
