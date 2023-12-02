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
        foreach (var l in _input)
        {
            List<int> digits = new();
            for (var i = 0; i < l.Length; i++)
            {
                if (char.IsDigit(l[i]))
                {
                    digits.Add(l[i] - '0');
                }
            }
            sum += digits[0] * 10 + digits[^1];
        }

        return new(sum.ToString()); 
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;
        var words = new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        foreach (var l in _input)
        {
            List<int> digits = new();
            for (var i = 0; i < l.Length; i++)
            {
                if (char.IsDigit(l[i]))
                {
                    digits.Add(l[i] - '0');
                }
                foreach(var w in words)
                {
                    if (l[i..].StartsWith(w))
                    {
                        digits.Add(Array.IndexOf(words, w) + 1);
                    }
                }   
            }
            sum += digits[0] * 10 + digits[^1];
        }  
        return new(sum.ToString());
    }
}
