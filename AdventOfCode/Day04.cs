using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day04 : BaseDay
    {
        int totalCards = 0;
        int[] cardMatchCounts;
        Dictionary<int, int> memoizedCardCounts = new();

        public Day04()
        {
            var lines = File.ReadAllLines(InputFilePath);
            totalCards = lines.Length;
            cardMatchCounts = new int[totalCards];
            for (var x = 0; x < lines.Length; x++)
            {
                var line = lines[x];
                var matchCount = 0;

                int charIndex = 0;
                while (line[charIndex] != ':')
                {
                    charIndex++;
                }
                charIndex+= 2;
                HashSet<int> winningNumbers = new();
                while (line[charIndex] != '|')
                {
                    winningNumbers.Add((byte)line[charIndex++] << 8 | (byte)line[charIndex++]);
                    charIndex++;
                }
                charIndex += 2;
                while (charIndex < line.Length)
                {
                    var b1 = (byte)line[charIndex++];
                    if (charIndex >= line.Length)
                    {
                        break;
                    }
                    var b2 = (byte)line[charIndex++];
                    charIndex++;
                    var drawnNum = b1 << 8 | b2;
                    if (winningNumbers.Contains(drawnNum))
                    {
                        matchCount++;
                    }
                }
                    
                cardMatchCounts[x] = matchCount;
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            foreach(var count in cardMatchCounts)
            {
                if (count > 0)
                {
                    answer += 1 << count - 1;
                }
            }
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            Queue<int> cardNumsToProcess = new();
            int cardsProcessed = 0;
            for(var x = 0; x < totalCards; x++)
            {
                cardsProcessed += ProcessCard(x);
            }
            
            return new(cardsProcessed.ToString());
        }

        private int ProcessCard(int index)
        {
            if (memoizedCardCounts.ContainsKey(index))
            {
                return memoizedCardCounts[index];
            }
            int count = 1;
            for (var x = index + 1; x < (index + 1) + cardMatchCounts[index]; x++)
            {
                count += ProcessCard(x);
            }
            memoizedCardCounts[index] = count;
            return count;
        }   
    }
}
