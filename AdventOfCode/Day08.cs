using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day08 : BaseDay
    {
        Dictionary<string, (string Left, string Right)> Map = new();
        string Directions = "";

        public Day08()
        {
            var lines = File.ReadAllLines(InputFilePath);

            Directions = lines[0];
            Regex mapLine = new Regex(@"([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)");
            for(var x = 2; x < lines.Length; x++)
            {
                var match = mapLine.Match(lines[x]);
                var key = match.Groups[1].Value;
                var left = match.Groups[2].Value;
                var right = match.Groups[3].Value;
                Map.Add(key, (left, right));
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var stepCount = stepsToZ("AAA", "ZZZ");
            return new(stepCount.ToString());
        }

        private long stepsToZ(string startLocation, string endLocation)
        {
            var location = startLocation;
            var stepCount = 0;
            var directionIndex = 0;
            while (!location.EndsWith(endLocation))
            {
                if (directionIndex == Directions.Length)
                {
                    directionIndex = 0;
                }

                var direction = Directions[directionIndex++];
                switch (direction)
                {
                    case 'L':
                        location = Map[location].Left;
                        break;
                    case 'R':
                        location = Map[location].Right;
                        break;
                }
                stepCount++;
            }
            return stepCount;
        }

        public override ValueTask<string> Solve_2()
        {
            List<long> stepCounts = new();
            foreach(var k in Map.Keys.Where(k => k.EndsWith("A")))
            {
                var steps = stepsToZ(k, "Z");
                stepCounts.Add(steps);
            }

            /* Cheating way to get the LCM here because my input seems to have lengths 
             * that are only Directions.Length * some number as prime factors 
             */
            stepCounts = stepCounts.Select(c => c / Directions.Length).ToList();
            stepCounts.Add(Directions.Length);
            var answer = stepCounts.Aggregate(1.0, (prod, next) => prod * next);

            return new(answer.ToString());
        }
    }
}
