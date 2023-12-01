using System.ComponentModel.Design;
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
        Regex twoNumbers = new Regex(@"(one|two|three|four|five|six|seven|eight|nine|[1-9]).*(one|two|three|four|five|six|seven|eight|nine|[1-9])");
        Regex singleNumber = new Regex(@"(one|two|three|four|five|six|seven|eight|nine|[1-9])");
        foreach (var l in _input)
        {
            var firstMatch = "";
            var lastMatch = "";
            var match = twoNumbers.Match(l);
            firstMatch = match.Groups[1].Value;
            lastMatch = match.Groups[match.Groups.Count - 1].Value;
            if (match.Success == false)
            {
                match = singleNumber.Match(l);
                lastMatch = firstMatch = match.Groups[1].Value;
            }
            
            var firstNumber = 0;
            var lastNumber = 0;

            if (int.TryParse(firstMatch, out firstNumber))
            {
                sum += firstNumber * 10;
            }
            else
            {
                firstNumber = Array.IndexOf(WORDS, firstMatch);
                sum += firstNumber * 10;
            }

            if (int.TryParse(lastMatch, out lastNumber))
            {
                sum += lastNumber;
            }
            else
            {
                lastNumber = Array.IndexOf(WORDS, lastMatch);
                sum += lastNumber;
            }
        } 
            
        return new(sum.ToString());
    }
}
