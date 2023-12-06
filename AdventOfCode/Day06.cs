using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day06 : BaseDay
    {
        struct Race
        {
            public ulong Time;
            public ulong Distance;
        }

        Race[] races;
        public Day06()
        {

        }
        public override ValueTask<string> Solve_1()
        {
            var lines = File.ReadAllLines(InputFilePath);
            var times = lines[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray();
            var distances = lines[1].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray();
            races = new Race[times.Length];
            for (int i = 0; i < races.Length; i++)
            {
                races[i] = new Race() { Time = times[i], Distance = distances[i] };
            }

            var answer = 1;
            foreach(var race in races)
            {
                var waysToWin = 0;
                for(ulong x = 0; x < race.Time; x++)
                {
                    if ((race.Time - x) * x > race.Distance) { 
                        waysToWin++; 
                    }
                }

                answer *= waysToWin;
            }
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var lines = File.ReadAllLines(InputFilePath);
            var time = ulong.Parse(lines[0].Split(':')[1].Replace(" ", ""));
            var distances = ulong.Parse(lines[1].Split(':')[1].Replace(" ",""));

            /* Formula for winning is (t - h) * h > d 
             * this can be turned in to an equation (t - h) * h = d, that is, what values lead to a tie
             * this can be rearranged to h^2 - th + d = 0 and use the quadratic formula to find both tie points
             * everything between is a win */

            var lowTie = (time - (ulong)(Math.Sqrt(time * time - 4 * distances))) / 2;
            var highTie = (time + (ulong)(Math.Sqrt(time * time - 4 * distances))) / 2;

            var answer = highTie - lowTie;

            return new(answer.ToString());
        }
    }
}
