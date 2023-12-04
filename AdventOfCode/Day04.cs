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
        struct Card
        {
            public int[] WinningNumbers;
            public int[] DrawnNumbers;

            public int MatchingNumbers()
            {
                int winningIndex = 0;
                int drawnIndex = 0;
                int matches = 0;
                while (winningIndex < WinningNumbers.Length && drawnIndex < DrawnNumbers.Length)
                {
                    while (winningIndex < WinningNumbers.Length && WinningNumbers[winningIndex] < DrawnNumbers[drawnIndex])
                    {
                        winningIndex++;
                    }
                    if (winningIndex == WinningNumbers.Length) { break; }
                    while (drawnIndex < DrawnNumbers.Length && DrawnNumbers[drawnIndex] < WinningNumbers[winningIndex])
                    {
                        drawnIndex++;
                    }
                    if (drawnIndex == DrawnNumbers.Length) { break; }
                    if (WinningNumbers[winningIndex] == DrawnNumbers[drawnIndex])
                    {
                        matches++;
                        winningIndex++;
                        drawnIndex++;
                    }
                }
                return matches;
            }
        }

        List<Card> cards = new();
        int[] cardMatchCounts;
        public Day04()
        {
            var lines = File.ReadAllLines(InputFilePath);

            foreach (var line in lines)
            {
                Card c = new Card();
                var cardHalves = line.Split(":")[1].Split("|");
                c.WinningNumbers = cardHalves[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).Order().ToArray();
                c.DrawnNumbers = cardHalves[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).Order().ToArray();
                cards.Add(c);
            }
            cardMatchCounts = new int[cards.Count];
            for (var x = 0; x < cards.Count; x++)
            {
                cardMatchCounts[x] = cards[x].MatchingNumbers();
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
            for(var x = 0; x < cards.Count; x++)
            {
                cardsProcessed += ProcessCard(x);
            }
            
            return new(cardsProcessed.ToString());
        }

        Dictionary<int, int> memoizedCardCounts = new();
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
