using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    struct Point3D
    {
        public int X;
        public int Y;
        public int Z;

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

    }

    class SandBlock
    {
        public static int BlockCount = 0;
        public (int min, int max) XBounds => (Math.Min(StartPoint.X, EndPoint.X), Math.Max(StartPoint.X, EndPoint.X));
        public (int min, int max) YBounds => (Math.Min(StartPoint.Y, EndPoint.Y), Math.Max(StartPoint.Y, EndPoint.Y));
        public (int min, int max) ZBounds => (Math.Min(StartPoint.Z, EndPoint.Z), Math.Max(StartPoint.Z, EndPoint.Z));

        public Point3D StartPoint;
        public Point3D EndPoint;
        public int BlockNum;

        public int MinZ;
        public int MaxZ;

        public bool IsOnGround => EndPoint.Z == 1;

        bool RangeOverlaps((int min, int max) b1, (int min, int max) b2)
        {
            if (b1.max < b2.min || b2.max < b1.min) return false;
            return true;
        }

        public bool IntersectsOnXY(SandBlock other)
        {
            /* check for not overlaps */
            return RangeOverlaps(XBounds, other.XBounds) && RangeOverlaps(YBounds, other.YBounds);
        }
        public bool IsDirectlyAbove(SandBlock other)
        {
            return (IntersectsOnXY(other) && MinZ == other.MaxZ + 1);
        }
        public bool IsDirectlyBelow(SandBlock other)
        {
            return (IntersectsOnXY(other) && MaxZ == other.MinZ - 1);
        }
        public SandBlock(Point3D startPoint, Point3D endPoint)
        {
            BlockNum = BlockCount++;
            StartPoint = startPoint;
            EndPoint = endPoint;
            MinZ = Math.Min(StartPoint.Z, EndPoint.Z);
            MaxZ = Math.Max(StartPoint.Z, EndPoint.Z);
        }

        public void MoveDown(int amount = 1)
        {
            StartPoint.Z -= amount;
            EndPoint.Z -= amount;
            MinZ -= amount;
            MaxZ -= amount;
        }

        public override string ToString()
        {
            return $"({BlockNum}) - {StartPoint}~{EndPoint}";
        }
    }
    internal class Day22 : BaseDay
    {
        List<SandBlock> Blocks = new List<SandBlock>();
        Dictionary<SandBlock, List<SandBlock>> BlockToBelowMap = new();
        Dictionary<SandBlock, List<SandBlock>> BlockToAboveMap = new();

        public Day22()
        {
            var lines = File.ReadAllLines(InputFilePath);

            foreach (var line in lines)
            {
                var parts = line.Split('~');
                var start = parts[0].Split(',');
                var end = parts[1].Split(',');

                var block = new SandBlock
                (
                    new Point3D
                    {
                        X = int.Parse(start[0]),
                        Y = int.Parse(start[1]),
                        Z = int.Parse(start[2]),
                    },
                    new Point3D
                    {
                        X = int.Parse(end[0]),
                        Y = int.Parse(end[1]),
                        Z = int.Parse(end[2]),
                    }
                );

                Blocks.Add(block);
            }
            
        }       

        public override ValueTask<string> Solve_1()
        {
            bool madeMoves = true;
            Blocks = Blocks.OrderBy(b => b.MinZ).ToList();
            while (madeMoves)
            {
                madeMoves = false;
                foreach (var block in Blocks)
                {
                    if (block.IsOnGround)
                    {
                        continue;
                    }

                    var possibleBlocksBelow = Blocks.Where(b => b.IntersectsOnXY(block) && b.MaxZ < block.MinZ);
                    var highestZBelowBlock = 0;
                    if (possibleBlocksBelow.Any())
                    {
                        highestZBelowBlock = possibleBlocksBelow.Max(b => b.MaxZ);
                    }

                    var zDelta = block.MinZ - highestZBelowBlock - 1;
                    if (zDelta > 0)
                    {
                        block.MoveDown(zDelta);
                        madeMoves = true;
                    }
                }
            }
            Blocks = Blocks.OrderBy(b => b.MinZ).ToList();

            List<SandBlock> removeable = new();

            foreach (var block in Blocks)
            {
                BlockToBelowMap[block] = Blocks.Where(b => b.IsDirectlyBelow(block)).ToList();
                BlockToAboveMap[block] = Blocks.Where(b => b.IsDirectlyAbove(block)).ToList();
            }

            for(var x = 0; x <  Blocks.Count; x++)
            {
                var block = Blocks[x];

                /* default to removeable until we prove we cant (ie, there's a block above us that only we are holding up) */
                var canRemove = true;

                /* We have blocks above us */
                if (BlockToAboveMap[block].Count > 0)
                {

                    /* Do the blocks above me only have 1 block below them (me) */
                    var blocksAboveMe = BlockToAboveMap[block];
                    foreach (var blockAbove in blocksAboveMe)
                    {
                        if (BlockToBelowMap[blockAbove].Count == 1)
                        {
                            /* If this is the case, we cant be safely removed */
                            canRemove = false;
                            break;
                        }
                    }
                }

                if (canRemove)
                {
                    removeable.Add(block);
                }
            }
            return new(removeable.Count.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var answer = 0;
            /* renumber the blocks */
            for(var x = 0; x < Blocks.Count; x++)
            {
                Blocks[x].BlockNum = x;
            }

            foreach(var block in Blocks.Skip(0))
            {
                if (!BlockToAboveMap[block].Select(b => BlockToBelowMap[b].Count).Any(b => b == 1))
                {
                    continue;
                }
                HashSet<SandBlock> fallingList = new();

                Queue<SandBlock> queue = new();
                queue.Enqueue(block);

                while(queue.Count > 0)
                {
                    var currentBlock = queue.Dequeue();
                    fallingList.Add(currentBlock);

                    var blocksAbove = BlockToAboveMap[currentBlock];
                    foreach(var blockAbove in blocksAbove)
                    {
                        var blocksBelow = BlockToBelowMap[blockAbove];
                        if (blocksBelow.Count == 1)
                        {
                            if (blockAbove.BlockNum == 1234)
                            {
                                Debugger.Break();
                            }
                            if (!queue.Contains(blockAbove))
                                queue.Enqueue(blockAbove);
                        } else
                        {
                            var alreadyFallingCount = blocksBelow.Where(b => fallingList.Contains(b)).Count();
                            var queuedBelowCount = blocksBelow.Where(b => queue.Contains(b)).Count();
                            if (queuedBelowCount + alreadyFallingCount == blocksBelow.Count)
                            {
                                if (!queue.Contains(blockAbove))
                                    queue.Enqueue(blockAbove);                                
                            }
                        }
                    }
                }
                var totalFallForThisBlock = 0;
                foreach (var checkBlock in Blocks)
                {
                    var blocksBelow = BlockToBelowMap[checkBlock];
                    if (blocksBelow.Count == 0) continue;
                    var isFalling = true;

                    foreach(var b in blocksBelow)
                    {
                        if (!fallingList.Contains(b))
                        {
                            isFalling = false;
                            break;
                        }
                    }
                    if (isFalling)
                        totalFallForThisBlock++;
                }
                answer += totalFallForThisBlock;
            }


            return new(answer.ToString());
        }
    }
}
