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
            var answer = 0;
            var lines = File.ReadAllLines(InputFilePath);
            var time = ulong.Parse(lines[0].Split(':')[1].Replace(" ", ""));
            var distances = ulong.Parse(lines[1].Split(':')[1].Replace(" ",""));
            for(ulong x = 0; x < time; x++)
            {
                if ((time - x) * x > distances) { answer++; }
            }
            return new(answer.ToString());
        }
    }
}
