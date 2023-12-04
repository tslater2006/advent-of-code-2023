using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var answer = cards.Sum(c => { var matches = c.MatchingNumbers(); if (matches == 0) { return 0; } else { return 1 << matches - 1; } });
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            Queue<int> cardNumsToProcess = new();

            for(var x = 0; x < cards.Count; x++)
            {
                cardNumsToProcess.Enqueue(x);
            }
            int cardsProcessed = 0;
            while (cardNumsToProcess.Count > 0)
            {
                int index = cardNumsToProcess.Dequeue();
                cardsProcessed++;
                for (var x = index + 1; x < (index+1) + cardMatchCounts[index]; x++)
                {
                    cardNumsToProcess.Enqueue(x);
                }
            }
            
            return new(cardsProcessed.ToString());
        }
    }
}
