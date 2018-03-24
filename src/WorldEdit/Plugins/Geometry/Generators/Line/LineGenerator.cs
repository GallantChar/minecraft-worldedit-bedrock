using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators
{
    public class LineGenerator : Generator, IGenerator
    {
        private const double ToleranceToFindPoint = 0.01;
        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        public List<Point> Run(ILineOptions options)
        {
            var opt = (ILineOptions) options;
            
            return DrawLine(opt.Start, opt.End);
        }

        public static List<Point> DrawLine(Point start, Point end)
        {
            var points = new List<Point>();
            points.Add(start);
            points.Add(end);

            Stack<Point[]> stack = new Stack<Point[]>();
            stack.Push(new[] {start, end});
            do
            {
                var p = stack.Pop();
                var delta = p[1].Subtract(p[0]);

                if (Math.Abs(delta.X) < 2 && Math.Abs(delta.Y) < 2 && Math.Abs(delta.Z) < 2) continue;

                var midPoint = Point.MidPoint(p[0], p[1], delta);
                points.Add(midPoint);
                stack.Push(new[] {p[0], midPoint});
                stack.Push(new[] {midPoint, p[1]});
            } while (stack.Any());

            return points;
        }

        /*
        private static bool PointLiesOnLine(int x1, int y1, int z1, int x2, int y2, int z2, int x, int y, int z)
        {
            var AB = Math.Sqrt((x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1) + (z2 - z1)*(z2 - z1));
            var AP = Math.Sqrt((x - x1)*(x - x1) + (y - y1)*(y - y1) + (z - z1)*(z - z1));
            var PB = Math.Sqrt((x2 - x)*(x2 - x) + (y2 - y)*(y2 - y) + (z2 - z)*(z2 - z));
            var difference = AB - (AP + PB);
            if (Math.Abs(difference) < ToleranceToFindPoint)
                return true;
            return false;
        }
        */
    }
}