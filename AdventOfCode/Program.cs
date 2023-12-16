
#if !AOT
if (args.Length == 0)
{
    await Solver.SolveLast(opt => { opt.ClearConsole = false; opt.ShowConstructorElapsedTime = true; });
}
else if (args.Length == 1 && args[0].Contains("all", StringComparison.CurrentCultureIgnoreCase))
{
    await Solver.SolveAll(opt =>
    {
        opt.ShowConstructorElapsedTime = true;
        opt.ShowTotalElapsedTimePerDay = true;
        opt.ShowConstructorElapsedTime = true;
    });
}
else
{
    var indexes = args.Select(arg => uint.TryParse(arg, out var index) ? index : uint.MaxValue);

    await Solver.Solve(indexes.Where(i => i < uint.MaxValue));
}

#else 
#region AOT
using AdventOfCode;
using System.Diagnostics;
long lastMs = 0;
List<(string,string)> answers = new List<(string,string)>();
void PrintTimes(int day, string part1, string part2, long ms)
{
    answers.Add((part1, part2));
    Console.WriteLine($"Day {day} ({ms - lastMs}ms)");
    lastMs = ms;
}

Stopwatch sw = new();
sw.Start();

var day1 = new Day01();

var day1p1 = await day1.Solve_1();
var day1p2 = await day1.Solve_2();

PrintTimes(1, day1p1, day1p2, sw.ElapsedMilliseconds);

var day2 = new Day02();

var day2p1 = await day2.Solve_1();
var day2p2 = await day2.Solve_2();

PrintTimes(2, day2p1, day2p2, sw.ElapsedMilliseconds);

var day3 = new Day03();

var day3p1 = await day3.Solve_1();
var day3p2 = await day3.Solve_2();

PrintTimes(3, day3p1, day3p2, sw.ElapsedMilliseconds);

var day4 = new Day04();
var day4p1 = await day4.Solve_1();
var day4p2 = await day4.Solve_2();

PrintTimes(4, day4p1, day4p2, sw.ElapsedMilliseconds);

var day5 = new Day05();
var day5p1 = await day5.Solve_1();
var day5p2 = await day5.Solve_2();

PrintTimes(5, day5p1, day5p2, sw.ElapsedMilliseconds);

var day6 = new Day06();
var day6p1 = await day6.Solve_1();
var day6p2 = await day6.Solve_2();

PrintTimes(6, day6p1, day6p2, sw.ElapsedMilliseconds);

var day7 = new Day07();
var day7p1 = await day7.Solve_1();
var day7p2 = await day7.Solve_2();

PrintTimes(7, day7p1, day7p2, sw.ElapsedMilliseconds);

var day8 = new Day08();
var day8p1 = await day8.Solve_1();
var day8p2 = await day8.Solve_2();

PrintTimes(8, day8p1, day8p2, sw.ElapsedMilliseconds);

var day9 = new Day09();
var day9p1 = await day9.Solve_1();
var day9p2 = await day9.Solve_2();

PrintTimes(9, day9p1, day9p2, sw.ElapsedMilliseconds);

var day10 = new Day10();
var day10p1 = await day10.Solve_1();
var day10p2 = await day10.Solve_2();

PrintTimes(10, day10p1, day10p2, sw.ElapsedMilliseconds);

var day11 = new Day11();
var day11p1 = await day11.Solve_1();
var day11p2 = await day11.Solve_2();

PrintTimes(11, day11p1, day11p2, sw.ElapsedMilliseconds);

var day12 = new Day12();
var day12p1 = await day12.Solve_1();
var day12p2 = await day12.Solve_2();

PrintTimes(12, day12p1, day12p2, sw.ElapsedMilliseconds);

var day13 = new Day13();
var day13p1 = await day13.Solve_1();
var day13p2 = await day13.Solve_2();

PrintTimes(13, day13p1, day13p2, sw.ElapsedMilliseconds);

var day14 = new Day14();
var day14p1 = await day14.Solve_1();
var day14p2 = await day14.Solve_2();

PrintTimes(14, day14p1, day14p2, sw.ElapsedMilliseconds);

var day15 = new Day15();
var day15p1 = await day15.Solve_1();
var day15p2 = await day15.Solve_2();

PrintTimes(15, day15p1, day15p2, sw.ElapsedMilliseconds);

Console.WriteLine("\nDone in: " + sw.ElapsedMilliseconds + "ms");

var day = 0;
foreach(var ans in answers)
{
    day++;
    Console.WriteLine($"Day {day}:\n\tPart 1: {ans.Item1}\n\tPart 2: {ans.Item2}");
}

#endregion
#endif


