using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    struct Game
    {
        public Game() { }
        public int Id;
        public List<CubeSet> CubeSets = new();
    }
    struct CubeSet
    {
        public int Red;
        public int Green;
        public int Blue;
    }

    internal class Day02 : BaseDay
    {
        private readonly string[] _input;
        private List<Game> _games = new();
        public Day02()
        {
            _input = File.ReadAllLines(InputFilePath);
            foreach (var l in _input)
            {
                var parts = l.Split(':');

                var game = new Game();
                game.Id = int.Parse(parts[0].Replace("Game ", ""));
                var sets = parts[1].Split(';');

                foreach (var s in sets)
                {
                    var cubeSet = new CubeSet();
                    var colors = s.Split(',');
                    foreach(var c in colors)
                    {
                        var colorParts = c.Trim().Split(' ');
                        switch (colorParts[1])
                        {
                            case "red":
                                cubeSet.Red = int.Parse(colorParts[0]);
                                break;
                            case "green":
                                cubeSet.Green = int.Parse(colorParts[0]);
                                break;
                            case "blue":
                                cubeSet.Blue = int.Parse(colorParts[0]);
                                break;

                        }
                    }
                    game.CubeSets.Add(cubeSet);
                }
                _games.Add(game);
            }
        }

        public override ValueTask<string> Solve_1()
        {
            var answer = _games.Where(g => g.CubeSets.All(s => s.Red <= 12 && s.Green <= 13 && s.Blue <= 14)).Sum(g => g.Id);

            return new(answer.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = _games.Sum(g => g.CubeSets.Max(s => s.Red) * g.CubeSets.Max(s => s.Green) * g.CubeSets.Max(s => s.Blue));
            return new(answer.ToString());
        }
    }
}
