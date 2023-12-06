using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    struct MapEntry
    {
        public long DestinationStart;
        public long SourceStart;
        public long SourceEnd;

        public override string ToString()
        {
            return $"({DestinationStart}, {SourceStart}, {SourceEnd})";
        }
    }

    struct Range
    {
        public long Start;
        public long End;
        public override string ToString()
        {
            return $"({Start}, {End})";
        }
        public List<Range> ApplyMapping(List<MapEntry> maps)
        {
            /* Note: maps are already ordered by SourceStart in ascending order */

            List<Range> newRanges = new();
            foreach (var map in maps)
            {
                var delta = map.DestinationStart - map.SourceStart;

                if (Start > map.SourceEnd) { continue; }

                /* If the entire range is befor the current map, just pass this range through */
                if (Start < map.SourceStart && End < map.SourceStart)
                {
                    newRanges.Add(this);
                    break;
                }

                /* range starts before map, but crosses into the map */
                if (Start < map.SourceStart && End >= map.SourceStart)
                {
                    newRanges.Add(new Range() { Start = Start, End = map.SourceStart - 1 });
                    Start = map.SourceStart;
                }

                /* current range ends before the map does */
                if (End <= map.SourceEnd)
                {
                    newRanges.Add(new Range() { Start = Start + delta, End = End + delta });
                    break;
                }

                /* current range ends after the map, need to split it into part that is inside the map and after */
                if (End > map.SourceEnd)
                {
                    newRanges.Add(new Range() { Start = Start + delta, End = map.SourceEnd + delta });
                    Start = map.SourceEnd + 1;
                }

            }

            /* If the last map ends before the end of the range, add the rest of the range */
            var lastMap = maps[maps.Count - 1];
            if (Start > lastMap.SourceEnd)
            {
                newRanges.Add(this);
            }

            /* If no maps were applied, just return the original range */
            if (newRanges.Count == 0) { newRanges.Add(this); }

            return newRanges;
        }
    }

    public class Day05 : BaseDay
    {
        List<MapEntry>[] Maps = new List<MapEntry>[7];
        List<long> Seeds = new();
        public Day05()
        {
            var lines = File.ReadAllLines(InputFilePath);

            Regex regex = new Regex(@"(\d+)");
            foreach(Match match in regex.Matches(lines[0])){
                Seeds.Add(long.Parse(match.Value));
            }
            var mapIndex = -1;
            Regex mapRegex = new Regex(@"(\d+) (\d+) (\d+)");
            foreach(var line in lines.Skip(1))
            {
                var matches = mapRegex.Matches(line);
                if (matches.Count == 1)
                {
                    MapEntry nextEntry = new();

                    nextEntry.DestinationStart = long.Parse(matches[0].Groups[1].Value);
                    nextEntry.SourceStart = long.Parse(matches[0].Groups[2].Value);
                    nextEntry.SourceEnd = nextEntry.SourceStart + long.Parse(matches[0].Groups[3].Value) - 1;

                    Maps[mapIndex].Add(nextEntry);
                } else
                {
                    if (line.EndsWith("map:"))
                    {
                        mapIndex++;
                        Maps[mapIndex] = new();
                    }
                }
            }
            foreach(var map in Maps)
            {
                map.Sort((a, b) => a.SourceStart.CompareTo(b.SourceStart));
            }
        }

        private long GetSeedLocation(long seed)
        {
            var currentValue = seed;

            for(var x =0; x < Maps.Length; x++)
            {
                var correctMapping = Maps[x].Where(m => currentValue >= m.SourceStart && currentValue <= m.SourceEnd).FirstOrDefault();
                if ( correctMapping.Equals(default(MapEntry))) 
                {
                    continue;
                }

                currentValue = correctMapping.DestinationStart + (currentValue - correctMapping.SourceStart);
            }

            return currentValue;
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = Seeds.Select(Seeds => GetSeedLocation(Seeds)).Min();
            return new(answer.ToString());
        }
        

        public override ValueTask<string> Solve_2()
        {
             List<Range> ranges = new();

             for(var x = 0; x < Seeds.Count-1; x+=2)
             {
                 var range = new Range();
                 range.Start = Seeds[x];
                 range.End = range.Start + Seeds[x+1] - 1;
                 ranges.Add(range);
             }
            var count = 0;
             foreach(var map in Maps)
             {
                 List<Range> newRanges = new();
                 foreach (var range in ranges)
                 {
                    count++;
                     newRanges.AddRange(range.ApplyMapping(map));
                 }
                 ranges = newRanges;
             }
             var answer = ranges.Min(r => r.Start);
             return new(answer.ToString());
        }
    }
}
