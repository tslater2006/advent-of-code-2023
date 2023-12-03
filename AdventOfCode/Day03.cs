using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{ 
    struct Part
    {
        public int Number;
        public List<Point2D> Points;
    }
    struct Symbol
    {
        public Point2D Point;
        public char Type;
    }
    internal class Day03 : BaseDay
    {
        
        List<Part> Parts = new();
        List<Symbol> Symbols = new();

        public Day03()
        {
            string[] lines = File.ReadAllLines(InputFilePath);
            for(var y = 0; y < lines.Length; y++)
            {
                var runningPartNumber = 0;
                Part part = new Part() { Points = new() };
                for (var x = 0; x < lines[y].Length; x++)
                {
                    if (char.IsDigit(lines[y][x]))
                    {
                        part.Points.Add(new Point2D { X = x, Y = y });
                        runningPartNumber *= 10;
                        runningPartNumber += lines[y][x] - '0';
                    } else
                    {
                        if (runningPartNumber > 0)
                        {
                            part.Number = runningPartNumber;
                            Parts.Add(part);
                            runningPartNumber = 0;
                            part = new Part() { Points = new() };
                        }
                        if (lines[y][x] != '.')
                        {
                            Symbols.Add(new Symbol { Point = { X = x, Y = y }, Type = lines[y][x] });
                        }
                    }
                    
                }
                if (runningPartNumber > 0)
                {
                    part.Number = runningPartNumber;
                    Parts.Add(part);
                    runningPartNumber = 0;
                }
            }
        }
        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            foreach(var part in Parts)
            {
                foreach (var point in part.Points.SelectMany(p => p.PointsAround()))
                {
                    if (Symbols.Where(s => s.Point.X == point.X && s.Point.Y == point.Y).Any())
                    {
                        answer += part.Number;
                        break;
                    }
                }
            }
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = 0;
            var possibleGears = Symbols.Where(s => s.Type == '*');

            foreach(var candidate in possibleGears)
            {
                var touchingParts = Parts.Where(p => p.Points.SelectMany(p => p.PointsAround()).Contains(candidate.Point)).ToList();
                if (touchingParts.Count == 2) 
                {
                    answer += (touchingParts[0].Number * touchingParts[1].Number);
                }
            }
            return new(answer.ToString());
        }
    }
}
