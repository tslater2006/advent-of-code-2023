using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day13 : BaseDay
    {
        struct Pattern
        {
            public int[] Rows;
            public int[] Columns;
        }
        List<Pattern> Patterns = new();
        public Day13()
        {
            bool startNewPattern = true;
            int[] colVals = null;
            List<int> rowVals = new();
            foreach(var line in File.ReadAllLines(InputFilePath))
            {
                if (line == "")
                {
                    Patterns.Add(new() { Rows = rowVals.ToArray(), Columns = colVals });
                    startNewPattern = true;
                    continue;
                }

                if (startNewPattern)
                {
                    rowVals.Clear();
                    colVals = new int[line.Length];
                    startNewPattern = false;
                }

                int rowVal = 0;
                for(var x = 0;x < line.Length;x++)
                {
                    colVals[x] <<= 1;
                    if (line[x] == '#')
                        colVals[x] |= 1;
                    rowVal <<= 1;
                    if (line[x] == '#')
                        rowVal|= 1;
                }
                rowVals.Add(rowVal);
            }

            Patterns.Add(new() { Rows = rowVals.ToArray(), Columns = colVals });
        }

        private int GetReflectionIndex(int[] numbers, bool useTweak = false)
        {

            for (int x = 0; x < numbers.Length - 1; x++)
            {
                int diff = numbers[x] ^ numbers[x + 1];
                if (diff == 0 || (useTweak && (diff & (diff - 1)) == 0 ))
                {
                    int start = (int)x - 1;
                    int end = (int)x + 2;

                    while (start >= 0 && end < numbers.Length && (diff & (diff - 1)) == 0)
                    {
                        diff |= numbers[start] ^ numbers[end];
                        start--;
                        end++;
                    }

                    if ((!useTweak && diff==0) || (useTweak && diff > 0 && (diff & (diff - 1)) == 0))
                    {
                        return x + 1;
                    }
                }
            }

            return 0;
        }
        public override ValueTask<string> Solve_1()
        {
            int rows = 0;
            int columns = 0;

            foreach (var p in Patterns)
            {
                rows += GetReflectionIndex(p.Rows);
                columns += GetReflectionIndex(p.Columns);
            }

            var answer = columns + (100 * rows);

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            int rows = 0;
            int columns = 0;

            foreach (var p in Patterns)
            {
                rows += GetReflectionIndex(p.Rows, true);
                columns += GetReflectionIndex(p.Columns, true);
            }

            var answer = columns + (100 * rows);

            return new(answer.ToString());
        }
    }
}
