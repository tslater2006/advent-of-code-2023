using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day15 : BaseDay
    {
        class SHA1A
        {
            public int State;

            public SHA1A()
            {
                State = 0;
            }

            public void Update(char c)
            {
                State += c;
                State *= 17;
                State %= 256;
            }

            public void Reset()
            {
                State = 0;
            }

        }

        string InitSequence;
        public Day15()
        {
            InitSequence = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            SHA1A sha = new SHA1A();
            foreach(var c in InitSequence)
            {
                if (c == ',')
                {
                    answer += sha.State;
                    sha.Reset();
                } else
                {
                    sha.Update(c);
                }
            }

            answer += sha.State;

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            List<(string Label, int Strength)>[] HASHMAP = new List<(string Label, int Strength)>[256];
            for (int i = 0; i < 256; i++)
            {
                HASHMAP[i] = new();
            }

            var answer = 0;
            SHA1A sha = new SHA1A();
            var labelStart = 0;
            var boxIndex = 0;
            var label = "";

            for(var x = 0; x < InitSequence.Length; x++)
            {
                var c = InitSequence[x];
                switch(c)
                {
                    case ',':
                        labelStart = x + 1;
                        sha.Reset();
                        break;
                    case '=':
                        boxIndex = sha.State;
                        label = InitSequence[labelStart..(labelStart + (x - labelStart))];
                        var strength = InitSequence[++x] - 0x30;
                        int existingIndex = -1;
                        for(var i =0 ; i < HASHMAP[boxIndex].Count; i++)
                        {
                            if (HASHMAP[boxIndex][i].Label == label)
                            {
                                existingIndex = i;
                                break;
                            }
                        }

                        if (existingIndex >= 0)
                        {
                            HASHMAP[boxIndex][existingIndex] = (label, strength);
                            break;
                        }
                        else
                        {
                            HASHMAP[boxIndex].Add((label, strength));
                        }
                        break;
                    case '-':
                        boxIndex = sha.State;
                        label = InitSequence[labelStart..(labelStart+(x - labelStart))];
                        for(var i = 0; i < HASHMAP[boxIndex].Count; i++)
                        {
                            if (HASHMAP[boxIndex][i].Label == label)
                            {
                                HASHMAP[boxIndex].RemoveAt(i);
                                break;
                            }
                        }
                        break;
                    default:
                        sha.Update(c);
                        break;
                }
            }

            for(var x = 0; x < 256; x++)
            {
                for(var y = 0; y < HASHMAP[x].Count; y++)
                {
                    var focusPower = (x + 1) * (y + 1) * HASHMAP[x][y].Strength;
                    answer += focusPower;
                }
            }
            return new(answer.ToString());
        }
    }
}
