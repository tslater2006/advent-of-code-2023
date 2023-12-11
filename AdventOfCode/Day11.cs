using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day11 : BaseDay
    {
        List<Point> Galaxies = new();
        List<int> EmptyColumns = new();
        List<int> EmptyRows = new();

        public Day11()
        {
            int[] GalaxyColumnCounts;
            var lines = File.ReadAllLines(InputFilePath);
            GalaxyColumnCounts = new int[lines[0].Length];
            for (var y = 0; y < lines.Length; y++)
            {
                var rowCount = 0;
                for (var x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        Galaxies.Add(new Point(x, y));
                        GalaxyColumnCounts[x]++;
                        rowCount++;
                    }
                }
                if (rowCount == 0) { EmptyRows.Add(y); }
            }
            for (var x = 0; x < GalaxyColumnCounts.Length; x++)
            {
                if (GalaxyColumnCounts[x] == 0) { EmptyColumns.Add(x); }
            }
        }

        private ulong GetDistanceBetweenGalaxies(ulong expansionRate)
        {
            ulong totalDistance = 0;
            /* Loop through every pair of galaxies, order doesn't matter */
            for (var x = 0; x < Galaxies.Count; x++)
            {
                for (var y = x + 1; y < Galaxies.Count; y++)
                {
                    var galaxy = Galaxies[x];
                    var galaxy2 = Galaxies[y];

                    // manhatan distance between the two galaxies
                    ulong distance = (ulong)(Math.Abs(galaxy.X - galaxy2.X) + Math.Abs(galaxy.Y - galaxy2.Y));

                    /* account for any empty columns between the galaxies */
                    ulong emptyColumnsBetween = (ulong)EmptyColumns.Where(c => c > Math.Min(galaxy.X, galaxy2.X) && c < Math.Max(galaxy.X, galaxy2.X)).Count();
                    distance += (ulong)(emptyColumnsBetween * (expansionRate - 1));

                    /* account for any empty rows between the galaxies */
                    ulong emptyRowsBetween = (ulong)EmptyRows.Where(r => r > Math.Min(galaxy.Y, galaxy2.Y) && r < Math.Max(galaxy.Y, galaxy2.Y)).Count();
                    distance += (ulong)(emptyRowsBetween * (expansionRate - 1));

                    totalDistance += distance;
                }
            }

            return totalDistance;
        }
        public override ValueTask<string> Solve_1()
        {
            return new(GetDistanceBetweenGalaxies(2).ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            return new(GetDistanceBetweenGalaxies(1000000).ToString());
        }
    }
}
