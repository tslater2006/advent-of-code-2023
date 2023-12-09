using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day09 : BaseDay
    {
        List<List<int>> Histories = new();
        public Day09()
        {
            foreach(var line in File.ReadAllLines(InputFilePath))
            {
                Histories.Add(line.Split(" ").Select(s => int.Parse(s)).ToList());
            }
        }

        private List<List<int>> GetStepsToZeros(List<int> history)
        {
            List<List<int>> allSteps = new();
            List<int> currentStep = history;
            allSteps.Add(currentStep);
            while (true)
            {
                List<int> nextStep = new();
                var addedNonZero = false;
                for (var x = 0; x < currentStep.Count - 1; x++)
                {
                    var nextNum = currentStep[x + 1] - currentStep[x];
                    nextStep.Add(nextNum);

                    if (!addedNonZero && nextNum != 0) { addedNonZero = true; }
                }
                allSteps.Add(nextStep);
                if (!addedNonZero) { break; }
                currentStep = nextStep;
            }

            return allSteps;
        }

        private int ExtrapolatePrevious(List<int> history)
        {
            var allSteps = GetStepsToZeros(history);
            allSteps.Last().Insert(0, 0);

            for(var x = allSteps.Count - 2; x >= 0; x--)
            {
                allSteps[x].Insert(0, allSteps[x][0] - allSteps[x + 1][0]);
            }


            return history.First();
        }
        private int ExtrapolateNext(List<int> history)
        {
            var allSteps = GetStepsToZeros(history);

            allSteps.Last().Add(0);
            for(var x = allSteps.Count - 2; x >= 0; x--)
            {
                allSteps[x].Add(allSteps[x].Last() + allSteps[x + 1].Last());
            }

            return history.Last();
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = Histories.Select(h => ExtrapolateNext(h)).Sum();
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = Histories.Select(h => ExtrapolatePrevious(h)).Sum();
            return new(answer.ToString());
        }
    }
}
