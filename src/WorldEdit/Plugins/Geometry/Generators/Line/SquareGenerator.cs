using System.Collections.Generic;

namespace ShapeGenerator.Generators
{
    public class SquareGenerator : Generator, IGenerator
    {
        List<Line> IGenerator.Run(Options options)
        {
            if (!options.Fill)
            {
                return TransformToLines(Run(options), options);
            }
            return GenerateFillSquare(options);
        }

        public List<Point> Run(ISquareOptions options)
        {
            var opt = (ISquareOptions) options;
            var points = new List<Point>();

            var lowerX = opt.X - opt.Width/2;
            var lowerY = opt.Y;
            ;
            var lowerZ = opt.Z - opt.Length /2;
            var upperX = opt.X + opt.Width/2;
            var upperY = lowerY + opt.Height - 1;
            ;
            var upperZ = opt.Z + opt.Length /2;

            Swap(ref lowerY, ref upperY);
            Swap(ref lowerX, ref upperX);
            Swap(ref lowerZ, ref upperZ);

            for (var x = lowerX; x <= upperX; x++)
            {
                for (var y = lowerY; y <= upperY; y++)
                {
                    for (var z = lowerZ; z <= upperZ; z++)
                    {
                        if (TestForCoordinate(x, lowerX, upperX, z, lowerZ, upperZ, opt, y, lowerY, upperY))
                        {
                            points.Add(new Point {X = x, Y = y, Z = z});
                        }
                    }
                }
            }
            return points;
        }

        private static void Swap(ref int smaller, ref int larger)
        {
            if (larger < smaller)
            {
                var swap = larger;
                larger = smaller;
                smaller = swap;
            }
        }

        protected virtual bool TestForCoordinate(int x, int lowerX, int upperX, int z, int lowerZ, int upperZ,
            ISquareOptions opt, int y, int lowerY, int upperY)
        {
            return x == lowerX || x == upperX || z == lowerZ || z == upperZ || opt.Fill
                || (x <lowerX + opt.Thickness)
                || (x > upperX - opt.Thickness )
                || (z < lowerZ +opt.Thickness)
                || (z > upperZ - opt.Thickness)

                ;
        }

        private static List<Line> GenerateFillSquare(Options opt)
        {
            var lowerX = opt.X - opt.Width/2;
            var lowerY = opt.Y;
            ;
            var lowerZ = opt.Z - opt.Width/2;

            var upperX = opt.X + opt.Width/2;
            var upperY = lowerY + opt.Height - 1;
            ;
            var upperZ = opt.Z + opt.Width/2;
            return
                Generator.SplitLinesIntoMaxSizes(new List<Line>
                {
                    new Line
                    {
                        Block = opt.Block,
                        Start = new Point {X = lowerX, Y = lowerY, Z = lowerZ},
                        End = new Point {X = upperX, Y = upperY, Z = upperZ}
                    }
                });
        }
    }
}