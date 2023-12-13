using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day13 : BaseDay
    {
        enum ReflectionType
        {
            Row, Column, None
        }
        class Pattern
        {
            public List<int> Rows = new();
            public int[] Columns;
            public Size Size;
        }
        List<Pattern> Patterns = new();
        public Day13()
        {
            bool startNewPattern = true;
            Pattern currentPattern = null;
            int[] colVals = null;
            foreach(var line in File.ReadAllLines(InputFilePath))
            {
                if (line == "")
                {
                    startNewPattern = true;
                    continue;
                }

                if (startNewPattern)
                {
                    currentPattern = new Pattern() { Size = new Size(line.Length, 0) };
                    colVals = new int[line.Length];
                    currentPattern.Columns = colVals;
                    Patterns.Add(currentPattern);
                    startNewPattern = false;
                }

                currentPattern.Size.Height++;

                var rowVal = 0;
                for(var x = 0;x < line.Length;x++)
                {
                    colVals[x] <<= 1;
                    if (line[x] == '#')
                        colVals[x] |= 1;
                    rowVal <<= 1;
                    if (line[x] == '#')
                        rowVal|= 1;
                }
                currentPattern.Rows.Add(rowVal);
            }
        }
        private bool WithinOneBit(int a, int b)
        {
            var bitDiff = (a ^ b);
            return (bitDiff & (bitDiff - 1)) == 0;
        }
        private (ReflectionType Type, int Index) GetReflectionIndex(Pattern p, bool useTweak = false)
        {
            var tweakCount = 0;
            var tweakIsUsed = false;
            /* check rows */
            for(var x = 0; x < p.Rows.Count - 1; x++)
            {
                if (p.Rows[x] == p.Rows[x + 1] || (WithinOneBit(p.Rows[x], p.Rows[x+1]) && !tweakIsUsed))
                {
                    // possible reflection point
                    var start = x;
                    var end = x + 1;
                    var reflectionFailed = false;

                    while (!reflectionFailed && start >= 0 && end < p.Rows.Count)
                    {
                        if (p.Rows[start] != p.Rows[end])
                        {
                            if (useTweak && !tweakIsUsed)
                            {
                                var bitDiff = (p.Rows[start] ^ p.Rows[end]);
                                if (!WithinOneBit(p.Rows[start], p.Rows[end]))
                                {
                                    reflectionFailed = true;
                                    tweakIsUsed = false;
                                }
                                else
                                {

                                    tweakIsUsed = true;
                                }
                            } else
                            {
                                reflectionFailed = true;
                                tweakIsUsed = false;
                            }
                        }
                        start--;
                        end++;
                    }

                    if ((start == -1 || end == p.Rows.Count) && reflectionFailed == false && (useTweak == false || useTweak && tweakIsUsed))
                        return (ReflectionType.Row, x+1);
                }
            }

            tweakIsUsed = false;
            /* check columns */
            for(var x = 0; x < p.Columns.Length - 1; x++)
            {
                if (p.Columns[x] == p.Columns[x + 1] || (WithinOneBit(p.Columns[x], p.Columns[x + 1]) && !tweakIsUsed))
                {
                    // possible reflection point
                    var start = x;
                    var end = x + 1;
                    var reflectionFailed = false;

                    while (!reflectionFailed && start >= 0 && end < p.Columns.Length)
                    {
                        if (p.Columns[start] != p.Columns[end])
                        {
                            if (useTweak && !tweakIsUsed)
                            {
                                
                                if (!WithinOneBit(p.Columns[start], p.Columns[end]))
                                {
                                    reflectionFailed = true;
                                    tweakIsUsed = false;
                                }
                                else
                                {
                                    tweakIsUsed = true;
                                }
                            }
                            else
                            {
                                reflectionFailed = true;
                                tweakIsUsed = false;
                            }
                        }
                        start--;
                        end++;
                    }

                    if ((start== -1 || end == p.Columns.Length) && reflectionFailed == false && (useTweak == false || useTweak && tweakIsUsed))
                        return (ReflectionType.Column, x+1);
                }
            }

            return (ReflectionType.None,0);
        }

        public override ValueTask<string> Solve_1()
        {
            var rows = 0;
            var cols = 0;

            foreach (var p in Patterns)
            {
                var result = GetReflectionIndex(p);
                switch (result.Type)
                {
                    case ReflectionType.None:
                        continue;
                    case ReflectionType.Row:
                        rows += result.Index;
                        break;
                    case ReflectionType.Column:
                        cols += result.Index;
                        break;
                }
            }

            var answer = cols + (100 * rows);

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var rows = 0;
            var cols = 0;

            foreach (var p in Patterns)
            {
                var result = GetReflectionIndex(p, true);
                switch (result.Type)
                {
                    case ReflectionType.None:
                        continue;
                    case ReflectionType.Row:
                        rows += result.Index;
                        break;
                    case ReflectionType.Column:
                        cols += result.Index;
                        break;
                }
            }

            var answer = cols + (100 * rows);
            return new(answer.ToString());
        }
    }
}
