using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day18 : BaseDay
    {
        struct PlanItem
        {
            public char Direction;
            public int Steps;
            public string Color;
        }
        PlanItem[] Plans;
        List<Point> LagoonPoints = new();
        ulong BoundaryPoints = 0;

        public Day18()
        {
            Plans = File.ReadAllLines(InputFilePath).Select(l =>
            {
                var parts = l.Split(' ');
                var direction = parts[0][0];
                var steps = int.Parse(parts[1]);
                return new PlanItem() { Direction = direction, Steps = steps, Color = parts[2][1..^1] };
            }).ToArray();
        }

        ulong GetAreaOfLagoon()
        {
            /* Use shoestring formula with polygon points (LagoonPoints) to calculate area */
            double area = 0;
            for (var i = 0; i < LagoonPoints.Count; i++)
            {
                var p1 = LagoonPoints[i];
                var p2 = LagoonPoints[(i + 1) % LagoonPoints.Count];
                area += ((long)p1.X * (long)p2.Y) - ((long)p1.Y * (long)p2.X);
            }
            area = Math.Abs(area / 2);

            /* Interior points via Pick's theorem that states: A = I + B/2 - 1 */
            /* rearranged to: I = A - B/2 + 1 */
            var interiorPoints = (ulong)(area - (BoundaryPoints / 2) + 1);
            return interiorPoints + BoundaryPoints;
        }

        public override ValueTask<string> Solve_1()
        {
            ulong answer = 0;
            var pos = new Point(0, 0);
            LagoonPoints.Add(pos);
            foreach (var plan in Plans)
            {
                switch (plan.Direction)
                {
                    case 'U':
                        pos.Y -= plan.Steps;
                        break;
                    case 'D':
                        pos.Y += plan.Steps;
                        break;
                    case 'L':
                        pos.X -= plan.Steps;
                        break;
                    case 'R':
                        pos.X += plan.Steps;
                        break;
                }
                BoundaryPoints += (ulong)plan.Steps;

                LagoonPoints.Add(pos);

            }
            answer = GetAreaOfLagoon();
            return new(answer.ToString());
        }


        public override ValueTask<string> Solve_2()
        {
            LagoonPoints = new();
            BoundaryPoints = 0;

            ulong answer = 0;
            var pos = new Point(0, 0);
            foreach (var plan in Plans)
            {
                var stepCount = int.Parse(plan.Color[1..^1], NumberStyles.HexNumber);
                switch (plan.Color[^1])
                {
                    case '0':
                        pos.X += stepCount;
                        break;
                    case '1':
                        pos.Y += stepCount;
                        break;
                    case '2':
                        pos.X -= stepCount;
                        break;
                    case '3':
                        pos.Y -= stepCount;
                        break;

                }
                BoundaryPoints += (ulong)stepCount;

                LagoonPoints.Add(pos);

            }
            answer = GetAreaOfLagoon();
            return new(answer.ToString());
        }
    }
}
