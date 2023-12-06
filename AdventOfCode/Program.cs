
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

Stopwatch sw = new();
sw.Start();

var day1 = new AdventOfCode.Day01();

var day1p1 = await day1.Solve_1();
var day1p2 = await day1.Solve_2();

Console.WriteLine("Day 1 Part 1: " + day1p1);
Console.WriteLine("Day 1 Part 2: " + day1p2);

var day2 = new Day02();

var day2p1 = await day2.Solve_1();
var day2p2 = await day2.Solve_2();

Console.WriteLine("Day 2 Part 1: " + day2p1);
Console.WriteLine("Day 2 Part 2: " + day2p2);

var day3 = new Day03();

var day3p1 = await day3.Solve_1();
var day3p2 = await day3.Solve_2();

Console.WriteLine("Day 3 Part 1: " + day3p1);
Console.WriteLine("Day 3 Part 2: " + day3p2);

var day4 = new AdventOfCode.Day04();
var day4p1 = await day4.Solve_1();
var day4p2 = await day4.Solve_2();

Console.WriteLine("Day 4 Part 1: " + day4p1);
Console.WriteLine("Day 4 Part 2: " + day4p2);

var day5 = new AdventOfCode.Day05();
var day5p1 = await day5.Solve_1();
var day5p2 = await day5.Solve_2();

Console.WriteLine("Day 5 Part 1: " + day5p1);
Console.WriteLine("Day 5 Part 2: " + day5p2);

Console.WriteLine("Done in: " + sw.ElapsedMilliseconds + "ms");
#endregion
#endif


