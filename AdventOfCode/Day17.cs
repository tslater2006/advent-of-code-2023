using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Implemented base on the HyperNeutrino solution details in this video: https://www.youtube.com/watch?v=2pDSooPLLkI */
namespace AdventOfCode
{
    internal class Day17 : BaseDay
    {
        int[,] Grid;
        public Day17()
        {
            /* Input file consists of a grid of single digit numbers, no spaces. parse this to a 2d array of ints */
            var lines = File.ReadAllLines(InputFilePath);
            Grid = new int[lines[0].Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    Grid[x, y] = lines[x][y] - '0';
                }
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = 0;
            var priorityQueue = new PriorityQueue<(int hl, int x, int y, int dx, int dy, int steps), int>();
            priorityQueue.Enqueue((0, 0, 0, 0, 0, 0), 0);
            var seen = new HashSet<(int x, int y, int dx, int dy, int steps)>();


            while (priorityQueue.Count > 0)
            {
                (int heatLoss, int x, int y, int deltaX, int deltaY, int steps) = priorityQueue.Dequeue();

                if (x == Grid.GetLength(0) - 1 && y == Grid.GetLength(1) - 1)
                {
                    answer = heatLoss;
                    break;
                }

                if (seen.Contains((x, y, deltaX, deltaY, steps)))
                    continue;

                seen.Add((x, y, deltaX, deltaY, steps));

                if (steps < 3 && (deltaX, deltaY) != (0,0))
                {
                    var newX = x + deltaX;
                    var newY = y + deltaY;
                    if (newX >= 0 && newX < Grid.GetLength(0) && newY >= 0 && newY < Grid.GetLength(1))
                    {
                        priorityQueue.Enqueue((heatLoss + Grid[newX, newY], newX, newY, deltaX, deltaY, steps + 1), heatLoss + Grid[newX, newY]);
                    }
                }

                foreach((int ndx, int ndy) in new (int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) })
                {
                    if ((ndx, ndy) != (deltaX, deltaY) && (ndx, ndy) != (-deltaX, -deltaY))
                    {
                        var newX = x + ndx;
                        var newY = y + ndy;
                        if (newX >= 0 && newX < Grid.GetLength(0) && newY >= 0 && newY < Grid.GetLength(1))
                        {
                            priorityQueue.Enqueue((heatLoss + Grid[newX, newY], newX, newY, ndx, ndy, 1), heatLoss + Grid[newX, newY]);
                        }
                    }
                }
               
            }

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = 0;
            var priorityQueue = new PriorityQueue<(int hl, int x, int y, int dx, int dy, int steps), int>();
            priorityQueue.Enqueue((0, 0, 0, 0, 0, 0), 0);
            var seen = new HashSet<(int x, int y, int dx, int dy, int steps)>();


            while (priorityQueue.Count > 0)
            {
                (int heatLoss, int x, int y, int deltaX, int deltaY, int steps) = priorityQueue.Dequeue();

                if (x == Grid.GetLength(0) - 1 && y == Grid.GetLength(1) - 1 && steps >= 4)
                {
                    answer = heatLoss;
                    break;
                }

                if (seen.Contains((x, y, deltaX, deltaY, steps)))
                    continue;

                seen.Add((x, y, deltaX, deltaY, steps));

                if (steps < 10 && (deltaX, deltaY) != (0, 0))
                {
                    var newX = x + deltaX;
                    var newY = y + deltaY;
                    if (newX >= 0 && newX < Grid.GetLength(0) && newY >= 0 && newY < Grid.GetLength(1))
                    {
                        priorityQueue.Enqueue((heatLoss + Grid[newX, newY], newX, newY, deltaX, deltaY, steps + 1), heatLoss + Grid[newX, newY]);
                    }
                }
                if (steps >= 4 || (deltaX, deltaY) == (0, 0))
                {
                    foreach ((int ndx, int ndy) in new (int, int)[] { (0, 1), (0, -1), (1, 0), (-1, 0) })
                    {
                        if ((ndx, ndy) != (deltaX, deltaY) && (ndx, ndy) != (-deltaX, -deltaY))
                        {
                            var newX = x + ndx;
                            var newY = y + ndy;
                            if (newX >= 0 && newX < Grid.GetLength(0) && newY >= 0 && newY < Grid.GetLength(1))
                            {
                                priorityQueue.Enqueue((heatLoss + Grid[newX, newY], newX, newY, ndx, ndy, 1), heatLoss + Grid[newX, newY]);
                            }
                        }
                    }
                }

            }

            return new(answer.ToString());
        }
    }
}
