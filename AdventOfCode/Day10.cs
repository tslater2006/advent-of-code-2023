﻿using AdventOfCode.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Day10 : BaseDay
    {
        Dictionary<Point, PipeType> Map = new();
        Dictionary<PipeType, (Point Delta, PipeType[] AllowedTypes)[]> Neighbors = new();
        List<Point> LoopPoints = new();
        int MapWidth;
        int MapHeight;
        Point StartPoint;

        (Point, PipeType[]) northConnection = (new Point(0, -1), new[] { PipeType.SouthAndEast, PipeType.SouthAndWest, PipeType.Vertical });
        (Point, PipeType[]) southConnection = (new Point(0, 1), new[] { PipeType.NorthAndEast, PipeType.NorthAndWest, PipeType.Vertical });
        (Point, PipeType[]) eastConnection = (new Point(1, 0), new[] { PipeType.NorthAndWest, PipeType.SouthAndWest, PipeType.Horizontal });
        (Point, PipeType[]) westConnection = (new Point(-1, 0), new[] { PipeType.NorthAndEast, PipeType.SouthAndEast, PipeType.Horizontal });

        enum PipeType : int
        {
            Vertical = (byte)'|',
            Horizontal = (byte)'-',
            NorthAndEast = (byte)'L',
            NorthAndWest = (byte)'J',
            SouthAndWest = (byte)'7',
            SouthAndEast = (byte)'F',
            Start = (byte)'S',
            Ground = (byte)'.',
            Outside = (byte)'~',
            Pipe = (byte)'#'

        }
        
        enum Direction
        {
            North,
            East,
            South,
            West
        }

        private Direction GetDirectionToPoint(Point fromPoint, Point toPoint)
        {
            if (fromPoint.X == toPoint.X)
            {
                if (fromPoint.Y > toPoint.Y) { return Direction.South; }
                else { return Direction.North; }
            } else
            {
                if (fromPoint.X > toPoint.X) { return Direction.East; }
                else { return Direction.West; }
            }
        }   

        public Day10()
        {
            var lines = File.ReadAllLines(InputFilePath);
            MapHeight = lines.Length;
            MapWidth = lines[0].Length;

            for(var y = 0; y < lines.Length; y++)
            {
                for(var x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == 'S')
                    {
                        StartPoint = new Point(x, y);
                    }
                    Map.Add(new Point(x, y), (PipeType)lines[y][x]);
                }
            }


            Neighbors.Add(PipeType.Vertical, new[] { northConnection, southConnection });
            Neighbors.Add(PipeType.Horizontal, new[] { eastConnection, westConnection });
            Neighbors.Add(PipeType.NorthAndEast, new[] { northConnection, eastConnection });
            Neighbors.Add(PipeType.NorthAndWest, new[] { northConnection, westConnection });
            Neighbors.Add(PipeType.SouthAndWest, new[] { southConnection, westConnection });
            Neighbors.Add(PipeType.SouthAndEast, new[] { southConnection, eastConnection });
            Neighbors.Add(PipeType.Ground, new (Point p, PipeType[] allowedTypes)[] { });
            Neighbors.Add(PipeType.Start, new[] {northConnection, eastConnection, southConnection, westConnection});
            

        }
        public override ValueTask<string> Solve_1()
        {
            LoopPoints.Add(StartPoint);

            Point previousPoint = StartPoint;
            Point currentPoint = StartPoint;
            foreach (var neighbor in Neighbors[Map[StartPoint]])
            {
                var testPoint = new Point(StartPoint.X + neighbor.Delta.X, StartPoint.Y + neighbor.Delta.Y);
                var directionToTestPoint = GetDirectionToPoint(fromPoint: StartPoint, toPoint: testPoint);
                if (directionToTestPoint == Direction.South)
                {
                    Neighbors[Map[testPoint]].Contains(northConnection);
                    currentPoint = testPoint;
                    break;
                } else if (directionToTestPoint == Direction.North)
                {
                    Neighbors[Map[testPoint]].Contains(southConnection);
                    currentPoint = testPoint;
                    break;
                } else if (directionToTestPoint == Direction.East)
                {
                    Neighbors[Map[testPoint]].Contains(westConnection);
                    currentPoint = testPoint;
                    break;
                } else if (directionToTestPoint == Direction.West)
                {
                    Neighbors[Map[testPoint]].Contains(eastConnection);
                    currentPoint = testPoint;
                    break;
                }
            }
            
            while(currentPoint != StartPoint)
            {
                LoopPoints.Add(currentPoint);
                var directionToPreviousPoint = GetDirectionToPoint(fromPoint: currentPoint, toPoint: previousPoint);
                foreach (var neighbor in Neighbors[Map[currentPoint]])
                {
                    var testPoint = new Point(currentPoint.X + neighbor.Delta.X, currentPoint.Y + neighbor.Delta.Y);
                    if (testPoint == previousPoint) { continue; }
                    previousPoint = currentPoint;
                    currentPoint = testPoint;
                    break;
                }
            }
            var maxDistance = (LoopPoints.Count / 2); /* half the loop length */
            return new(maxDistance.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            /* We make a 3x3 scaled version of the map, to account for being able to fit bewteen the pipes */
            Dictionary<Point, PipeType> ScaledMap = new();
            foreach (var p in LoopPoints)
            {
                switch (Map[p])
                {
                    case PipeType.Vertical:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 2), PipeType.Pipe);
                        break;
                    case PipeType.Horizontal:
                        ScaledMap.Add(new Point(p.X * 3, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 2, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    case PipeType.NorthAndEast:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 2, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    case PipeType.NorthAndWest:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    case PipeType.SouthAndWest:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 2), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    case PipeType.SouthAndEast:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 2), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 2, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    case PipeType.Start:
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 1, p.Y * 3 + 2), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3, p.Y * 3 + 1), PipeType.Pipe);
                        ScaledMap.Add(new Point(p.X * 3 + 2, p.Y * 3 + 1), PipeType.Pipe);
                        break;
                    default:
                        break;

                }
            }

            Stack<Point> stack = new();
            stack.Push(new Point(-1,-1));
            ScaledMap.Add(new Point(-1, -1), PipeType.Outside);
            while (stack.Count > 0)
            {
                var p = stack.Pop();
                foreach (var n in p.GetNeighborsWithinRectangle(new Rectangle(-1, -1, MapWidth * 3 + 2, MapHeight * 3 + 2)))
                {
                    if (!ScaledMap.ContainsKey(n))
                    {
                        ScaledMap.Add(n, PipeType.Outside);
                        stack.Push(n);
                        continue;
                    }       
                }
            }

            /* loop through Map and check if each point is in the ScaledMap */
            var mapPointsInFlood = 0;
            for(var y = 0; y < MapHeight; y++)
            {
                for(var x = 0; x < MapWidth; x++)
                {
                    if (ScaledMap.ContainsKey(new Point(x * 3 + 1, y * 3 + 1)))
                    {
                        mapPointsInFlood++;
                    }
                }
            }

            var missing = MapWidth * MapHeight - mapPointsInFlood;
            return new(missing.ToString());
        }
    }
}
