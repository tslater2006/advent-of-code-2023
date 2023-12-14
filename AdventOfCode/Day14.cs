using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day14 : BaseDay
    {
        char[,] Grid;
        int GridWidth => Grid.GetLength(0);
        int GridHeight => Grid.GetLength(1);
        string[] inputLines;
        public Day14()
        {
            inputLines = File.ReadAllLines(InputFilePath);
        }

        private void InitGrid()
        {
            Grid = new char[inputLines[0].Length, inputLines.Length];

            for (var y = 0; y < inputLines.Length; y++)
            {
                for (var x = 0; x < inputLines[y].Length; x++)
                {
                    Grid[x, y] = inputLines[y][x];
                }
            }
        }
        private string GridToString()
        {
            StringBuilder sb = new();
            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
                {
                    sb.Append(Grid[x, y]);
                }
            }
            return sb.ToString();
        }
        private void PrintGrid()
        {
            Console.WriteLine();
            /* print grid to console */
            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
                {
                    Console.Write(Grid[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public enum TiltDirection
        {
            North,
            East,
            South,
            West
        }
        private void TiltBoard(TiltDirection direction)
        {
            if (direction == TiltDirection.North || direction == TiltDirection.South)
            {
                int[] blockedIndexes = new int[GridWidth];
                for (var x = 0; x < GridWidth; x++)
                {
                    blockedIndexes[x] = direction == TiltDirection.North ? -1 : GridHeight;
                }

                var yStart = direction == TiltDirection.North ? 0 : GridHeight - 1;
                var yEnd = direction == TiltDirection.North ? GridHeight : -1;
                var yIncrement = direction == TiltDirection.North ? 1 : -1;

                for (var y = yStart; y != yEnd; y += yIncrement)
                {
                    for (var x = 0; x < GridWidth; x++)
                    {
                        if (Grid[x, y] == '#')
                        {
                            blockedIndexes[x] = y;
                        }
                        else if (Grid[x, y] == 'O')
                        {
                            Grid[x, y] = '.';
                            Grid[x, blockedIndexes[x] + yIncrement] = 'O';
                            blockedIndexes[x] += yIncrement;
                        }
                    }
                }
            }

            if (direction == TiltDirection.East || direction == TiltDirection.West)
            {
                int[] blockedIndexes = new int[GridHeight];
                for (var x = 0; x < GridHeight; x++)
                {
                    blockedIndexes[x] = direction == TiltDirection.West ? -1 : GridWidth;
                }

                var xStart = direction == TiltDirection.West ? 0 : GridWidth - 1;
                var xEnd = direction == TiltDirection.West ? GridWidth : -1;
                var xIncrement = direction == TiltDirection.West ? 1 : -1;

                for (var x = xStart; x != xEnd; x += xIncrement)
                {
                    for (var y = 0; y < GridHeight; y++)
                    {
                        if (Grid[x, y] == '#')
                        {
                            blockedIndexes[y] = x;
                        }
                        else if (Grid[x, y] == 'O')
                        {
                            Grid[x, y] = '.';
                            Grid[blockedIndexes[y] + xIncrement, y] = 'O';
                            blockedIndexes[y] += xIncrement;
                        }
                    }
                }   

            }
        }

        private void RunFullCycle()
        {
            TiltBoard(TiltDirection.North);
            TiltBoard(TiltDirection.West);
            TiltBoard(TiltDirection.South);
            TiltBoard(TiltDirection.East);
        }

        private int GetNorthBeamLoad()
        {
            var answer = 0;
            for (var y = 0; y < GridHeight; y++)
            {
                for (var x = 0; x < GridWidth; x++)
                {
                    if (Grid[x, y] == 'O')
                    {
                        answer += GridHeight - y;
                    }
                }
            }

            return answer;
        }

        public override ValueTask<string> Solve_1()
        {
            InitGrid();
            TiltBoard(TiltDirection.North);
            Console.WriteLine();
            var answer = GetNorthBeamLoad();
            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = 0;
            List<(string, int)> seenBoards = new List<(string,int)>();
            InitGrid();

            while(true)
            {
                RunFullCycle();
                var load = GetNorthBeamLoad();
                var key = (GridToString(), load);
                if (seenBoards.Contains(key))
                {
                    var patternStart = seenBoards.IndexOf(key) - 1;
                    var patternEnd = seenBoards.Count;
                    var patternLength = patternEnd - patternStart - 1;

                    answer = seenBoards[patternStart + ((1000000000 - patternStart - 1) % patternLength)].Item2;
                    break;
                }
                else
                {
                    seenBoards.Add(key);
                }
            }

            return new(answer.ToString());
        }
    }
}
