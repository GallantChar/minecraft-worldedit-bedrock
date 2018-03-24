using System;
using System.Collections.Generic;

namespace ShapeGenerator.Generators
{
    public class CircleGenerator : Generator, IGenerator
    {
        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        public List<Point> Run(ICircleOptions options)
        {
            var points = new List<Point>();
            if (options.Radius == 1)
            {
                points.Add(new Point {X = options.X, Y = options.Y, Z = options.Z});
                return points;
            }

            var d = options.Radius * options.Radius;
            for (int x = 0; x <= options.Radius + options.Thickness; x++)
            {
                for (int z = 0; z <= options.Radius + options.Thickness; z++)
                {
                    var distance = x * x + z * z;
                    for (int y = 0; y < options.Height; y++)
                    {
                        if (!options.Fill && Math.Abs(d - distance) < options.Radius + options.Thickness ||
                            options.Fill && distance < d + options.Thickness)
                        {
                            points.Add(new Point {X = options.X + x, Y = options.Y + y, Z = options.Z + z});
                            points.Add(new Point {X = options.X - x, Y = options.Y + y, Z = options.Z - z});
                            points.Add(new Point {X = options.X - x, Y = options.Y + y, Z = options.Z + z});
                            points.Add(new Point {X = options.X + x, Y = options.Y + y, Z = options.Z - z});
                        }
                    }
                }
            }

            return points;
        }
    }
}