using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{ 
    struct Part
    {
        public int Number;
        public Rectangle BoundingBox;
    }
    struct Symbol
    {
        public Point Location;
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
                Part part = new Part();
                for (var x = 0; x < lines[y].Length; x++)
                {
                    if (char.IsDigit(lines[y][x]))
                    {
                        if (runningPartNumber == 0)
                        {
                            part.BoundingBox = new Rectangle(x, y, 1, 1);
                        }
                        runningPartNumber *= 10;
                        runningPartNumber += lines[y][x] - '0';
                    } else
                    {
                        if (runningPartNumber > 0)
                        {
                            part.BoundingBox.Width = x - part.BoundingBox.X;

                            /* Pad the bounding box by 1 unit in each direction */
                            part.BoundingBox.X -= 1;
                            part.BoundingBox.Y -= 1;
                            part.BoundingBox.Width += 2;
                            part.BoundingBox.Height += 2;


                            part.Number = runningPartNumber;
                            Parts.Add(part);
                            runningPartNumber = 0;
                            part = new Part();
                        }
                        if (lines[y][x] != '.')
                        {
                            Symbols.Add(new Symbol { Location = { X = x, Y = y }, Type = lines[y][x] });
                        }
                    }
                    
                }
                if (runningPartNumber > 0)
                {
                    part.BoundingBox.X -= 1;
                    part.BoundingBox.Y -= 1;
                    part.BoundingBox.Width += 2;
                    part.BoundingBox.Height += 2;
                    part.Number = runningPartNumber;
                    Parts.Add(part);
                    runningPartNumber = 0;
                }
            }
        }
        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            foreach (var part in Parts)
            {
                if (Symbols.Where(s => part.BoundingBox.Contains(s.Location)).Any())
                {
                    answer += part.Number;
                }
            }
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = 0;
            foreach (var symbol in Symbols.Where(s => s.Type == '*'))
            {
                var neighboringParts = Parts.Where(p => p.BoundingBox.Contains(symbol.Location)).ToList();
                if (neighboringParts.Count == 2)
                {
                    answer += neighboringParts[0].Number * neighboringParts[1].Number;
                }
            }
            return new(answer.ToString());
        }
    }
}
