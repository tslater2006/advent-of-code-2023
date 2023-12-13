using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day12 : BaseDay
    {
        List<(string springs, int[] groups)> Records;

        public Day12()
        {
            Records = File.ReadAllLines(InputFilePath).Select(line =>
            {
                var parts = line.Split(" ");
                return (parts[0], parts[1].Split(',').Select(i => int.Parse(i)).ToArray());
            }).ToList();
        }

        Dictionary<(int charIndex, int groupIndex, int count), ulong> memoizeCache = new();
        ulong getCombinations(string springs, int[] groups, int charIndex, int groupIndex, int count)
        {
            if (memoizeCache.ContainsKey((charIndex, groupIndex, count)))
            {
                return memoizeCache[(charIndex, groupIndex, count)];
            }

            if (charIndex == springs.Length)
            {
                if (groupIndex == groups.Length && count == 0)
                {
                    return 1;
                } else if (groupIndex == groups.Length-1 && groups[groupIndex] == count)
                {
                    return 1;
                } else
                {
                    return 0;
                }
            }
            ulong answer = 0;
            
            /* Handle if we put a '.' here because it is a dot or a ? */
            if (springs[charIndex] == '.' || springs[charIndex] == '?')
            {
                if (count == 0)
                {
                    answer += getCombinations(springs, groups, charIndex + 1, groupIndex, count);
                } else if (groupIndex < groups.Length && groups[groupIndex] == count)
                {
                    answer += getCombinations(springs, groups, charIndex + 1, groupIndex + 1, 0);
                }
            }

            /* Handle if we put a '#' here because it is a # or a ? */
            if (springs[charIndex] == '#' || springs[charIndex] == '?')
            {
                if (groupIndex < groups.Length && count + 1 <= groups[groupIndex])
                {
                    answer += getCombinations(springs, groups, charIndex + 1, groupIndex, count + 1);
                }
            }
            memoizeCache.Add((charIndex, groupIndex, count), answer);
            return answer;
        }


        public override ValueTask<string> Solve_1()
        {
            ulong answer = 0;
            foreach(var r in Records)
            {
                answer += getCombinations(r.springs, r.groups, 0, 0, 0);
                memoizeCache.Clear();
            }
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            ulong answer = 0;
            foreach (var r in Records)
            {
                answer += getCombinations(string.Join("?", Enumerable.Repeat(r.springs, 5)), Enumerable.Repeat(r.groups, 5).SelectMany(i => i).ToArray(), 0, 0, 0);
                memoizeCache.Clear();
            }
            return new(answer.ToString());
        }
    }
}
