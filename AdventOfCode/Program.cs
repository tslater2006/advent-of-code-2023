
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
void PrintResults(int day, string part1, string part2, long ms)
{
    
    Console.WriteLine($"Day {day} Part 1: {part1} Part 2: {part2} ({ms - lastMs}ms)");
    lastMs = ms;
}

Stopwatch sw = new();
sw.Start();

var day1 = new AdventOfCode.Day01();

var day1p1 = await day1.Solve_1();
var day1p2 = await day1.Solve_2();

PrintResults(1, day1p1, day1p2, sw.ElapsedMilliseconds);

var day2 = new Day02();

var day2p1 = await day2.Solve_1();
var day2p2 = await day2.Solve_2();

PrintResults(2, day2p1, day2p2, sw.ElapsedMilliseconds);

var day3 = new Day03();

var day3p1 = await day3.Solve_1();
var day3p2 = await day3.Solve_2();

PrintResults(3, day3p1, day3p2, sw.ElapsedMilliseconds);

var day4 = new AdventOfCode.Day04();
var day4p1 = await day4.Solve_1();
var day4p2 = await day4.Solve_2();

PrintResults(4, day4p1, day4p2, sw.ElapsedMilliseconds);

var day5 = new AdventOfCode.Day05();
var day5p1 = await day5.Solve_1();
var day5p2 = await day5.Solve_2();

PrintResults(5, day5p1, day5p2, sw.ElapsedMilliseconds);

var day6 = new AdventOfCode.Day06();
var day6p1 = await day6.Solve_1();
var day6p2 = await day6.Solve_2();

PrintResults(6, day6p1, day6p2, sw.ElapsedMilliseconds);

var day7 = new AdventOfCode.Day07();
var day7p1 = await day7.Solve_1();
var day7p2 = await day7.Solve_2();

PrintResults(7, day7p1, day7p2, sw.ElapsedMilliseconds);

var day8 = new AdventOfCode.Day08();
var day8p1 = await day8.Solve_1();
var day8p2 = await day8.Solve_2();

PrintResults(8, day8p1, day8p2, sw.ElapsedMilliseconds);

Console.WriteLine("\nDone in: " + sw.ElapsedMilliseconds + "ms");
#endregion
#endif


