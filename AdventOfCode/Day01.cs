using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string[] _input;
    string[] WORDS = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() {
        var sum = 0;
        foreach(var l in _input)
        {
            var firstDigit = int.Parse(l.Where(char.IsDigit).First().ToString());
            var lastDigit = int.Parse(l.Where(char.IsDigit).Last().ToString());
            sum += firstDigit * 10 + lastDigit;
        }

        return new(sum.ToString()); 
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;
        foreach(var l in _input)
        {
            var firstDigit = 0;
            for (var x = 0; x < l.Length; x++)
            {
                if (char.IsDigit(l[x]))
                {
                    firstDigit = int.Parse(l[x].ToString());
                    break;
                }
                for (var y = 0; y < WORDS.Length; y++)
                {
                    if (l.IndexOf(WORDS[y]) == x)
                    {
                        firstDigit = y;
                        break;
                    }
                }
                if (firstDigit != 0)
                {
                    break;
                }
            }

            var lastDigit = 0;
            for (var x = l.Length - 1; x >= 0; x--)
            {
                if (char.IsDigit(l[x]))
                {
                    lastDigit = int.Parse(l[x].ToString());
                    break;
                }
                for (var y = 0; y < WORDS.Length; y++)
                {
                    if (l.LastIndexOf(WORDS[y]) == x)
                    {
                        lastDigit = y;
                        break;
                    }
                }
                if (lastDigit != 0)
                {
                    break;
                }
            }
            sum += firstDigit * 10 + lastDigit;
        } 
            
        return new(sum.ToString());
    }
}
