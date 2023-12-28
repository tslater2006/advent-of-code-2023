using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics.X86;


namespace AdventOfCode
{
    internal class Day24 : BaseDay
    {
        record Vector3D(decimal X, decimal Y, decimal Z);
        record Vector2D(decimal X, decimal Y);
        record Projected2D(Vector2D Position, Vector2D Velocity);

        class Hailstone
        {
            public Vector3D Position;
            public Vector3D Velocity;
            public decimal A;
            public decimal B;
            public decimal C;
            public Hailstone(decimal x, decimal y, decimal z, decimal vx, decimal vy, decimal vz)
            {
                Position = new Vector3D(x, y, z);
                Velocity = new Vector3D(vx, vy, vz);

                A = vy;
                B = -vx;
                C = vy * x - vx * y;
            }

            public override string ToString()
            {
                return $"Hailstone [a={A}, b={B}, c={C}]";
            }

            public Projected2D Project(Func<Vector3D, (decimal, decimal)> proj)
            {
                return new Projected2D(new Vector2D(proj(Position).Item1, proj(Position).Item2), new Vector2D(proj(Velocity).Item1, proj(Velocity).Item2));
            }

        }
        List<Hailstone> Hailstones = new();
        public Day24()
        {
            foreach (var line in File.ReadAllLines(InputFilePath))
            {
                var values = line.Replace('@', ',').Split(',').Select(i => decimal.Parse(i.Trim())).ToArray();
                Hailstones.Add(new Hailstone(values[0], values[1], values[2], values[3], values[4], values[5]));
            }

        }

        /* Part 1 implementation based on HyperNeutrino's vide here: https://www.youtube.com/watch?v=guOyA7Ijqgk */
        public override ValueTask<string> Solve_1()
        {
            var min = 200000000000000;
            var max = 400000000000000;

            var total = 0;
            for (var i = 0; i < Hailstones.Count; i++)
            {
                var hs1 = Hailstones[i];
                for (var j = 0; j < i; j++)
                {
                    var hs2 = Hailstones[j];

                    /* Skip if lines are parallel */
                    if (hs1.A * hs2.B == hs1.B * hs2.A)
                    {
                        continue;
                    }

                    var x = (hs1.C * hs2.B - hs2.C * hs1.B) / (hs1.A * hs2.B - hs2.A * hs1.B);
                    var y = (hs2.C * hs1.A - hs1.C * hs2.A) / (hs1.A * hs2.B - hs2.A * hs1.B);

                    if (x >= min && x <= max && y >= min && y <= max)
                    {
                        if ((x - hs1.Position.X) * hs1.Velocity.X >= 0 &&
                            (y - hs1.Position.Y) * hs1.Velocity.Y >= 0 &&
                            (x - hs2.Position.X) * hs2.Velocity.X >= 0 &&
                            (y - hs2.Position.Y) * hs2.Velocity.Y >= 0)
                        {
                            total++;
                        }
                    }
                }
            }

            return new(total.ToString());
        }

        bool Hits(Projected2D p, Vector2D pos)
        {
            if (pos == null) return false;

            var d = (pos.X - p.Position.X) * p.Velocity.Y - (pos.Y - p.Position.Y) * p.Velocity.X;
            return Math.Abs(d) < (decimal)0.0001;
        }

        // returns the position where the path of the particles meet
        Vector2D Intersection(Projected2D p1, Projected2D p2)
        {

            var (a11, a12, a21, a22) = (
                p1.Velocity.Y, -p1.Velocity.X,
                p2.Velocity.Y, -p2.Velocity.X
            );;

            var b = new Vector2D(
                p1.Velocity.Y * p1.Position.X - p1.Velocity.X * p1.Position.Y,
                p2.Velocity.Y * p2.Position.X - p2.Velocity.X * p2.Position.Y
            );

            var determinant = a11 * a22 - a12 * a21;
            if (determinant == 0)
            {
                return null; //particles don't meet
            }

            var (i11, i12, i21, i22) = (
                a22 / determinant, -a12 / determinant,
                -a21 / determinant, a11 / determinant
            );

            return new Vector2D(
                 i11 * b.X + i12 * b.Y,
                 i21 * b.X + i22 * b.Y
             );
        }

        Vector2D Solve2D(Projected2D[] particles)
        {
            var translateV = (Projected2D p, Vector2D vel) =>
                new Projected2D(p.Position, new Vector2D(p.Velocity.X - vel.X, p.Velocity.Y - vel.Y));

            var s = 500; //arbitrary limits for the brute force that worked for me.
            for (var v1 = -s; v1 < s; v1++)
            {
                for (var v2 = -s; v2 < s; v2++)
                {
                    var vel = new Vector2D(v1, v2);

                    // p0 and p1 are linearly independent (for me) => stone != null
                    var stone = Intersection(
                        translateV(particles[0], vel),
                        translateV(particles[1], vel)
                    );

                    if (particles.All(p => Hits(translateV(p, vel), stone)))
                    {
                        return stone;
                    }
                }
            }
            return null;
        }

        /* Part 2 implemented based on code provided by encse at https://aoc.csokavar.hu/?day=24 (https://github.com/encse/adventofcode/blob/master/2023/Day24/Solution.cs)*/
        public override ValueTask<string> Solve_2()
        {
            
            var stoneXY = Solve2D(Hailstones.Select(h => h.Project(v => (v.X, v.Y))).ToArray());
            var stoneXZ = Solve2D(Hailstones.Select(h => h.Project(v => (v.X, v.Z))).ToArray());

            var answer = Math.Round(stoneXY.X + stoneXY.Y + stoneXZ.Y);


            return new(answer.ToString());
        }
    }
}
