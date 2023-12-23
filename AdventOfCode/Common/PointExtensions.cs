using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common
{
    public static class PointExtensions
    {
        public static IEnumerable<Point> GetNeighbors(this Point point)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Skip the point itself
                    yield return new Point(point.X + x, point.Y + y);
                }
            }
        }
        public static IEnumerable<Point> GetOrthoganalNeighbors(this Point point)
        {
            var orthoDirections = new[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
            foreach(var direction in orthoDirections)
            {
                yield return new Point(point.X + direction.X, point.Y + direction.Y);;
            }
        }
        public static IEnumerable<Point> GetOrthoganalNeighbors(this Point point, Rectangle rectangle)
        {
            var orthoDirections = new[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
            foreach (var direction in orthoDirections)
            {
                var newPoint = new Point(point.X + direction.X, point.Y + direction.Y);
                if (rectangle.Contains(newPoint))
                {
                    yield return newPoint;
                }
            }
        }

        public static IEnumerable<Point> GetNeighbors(this Point point, Rectangle rectangle)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Skip the point itself
                    var neighbor = new Point(point.X + x, point.Y + y);
                    if (rectangle.Contains(neighbor))
                    {
                        yield return neighbor;
                    }
                }
            }
        }
    }
}
