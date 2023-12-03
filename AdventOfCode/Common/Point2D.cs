using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common
{
    internal struct Point2D
    {
        public int X;
        public int Y;

        public IEnumerable<Point2D> PointsAround()
        {
            yield return new Point2D { X = X - 1, Y = Y - 1 };
            yield return new Point2D { X = X - 1, Y = Y };
            yield return new Point2D { X = X - 1, Y = Y + 1 };
            yield return new Point2D { X = X, Y = Y - 1 };
            yield return new Point2D { X = X, Y = Y + 1 };
            yield return new Point2D { X = X + 1, Y = Y - 1 };
            yield return new Point2D { X = X + 1, Y = Y };
            yield return new Point2D { X = X + 1, Y = Y + 1 };
        }
    }
}
