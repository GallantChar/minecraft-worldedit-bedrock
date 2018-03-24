using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators
{
    public class SphereGenerator : Generator, IGenerator
    {
        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        public List<Point> Run(Options options)
        {
            var points = new List<Point>();
            if (options.Radius == 1)
            {
                points.Add(new Point { X = options.X, Y = options.Y, Z = options.Z });
                return points;
            }

            var d = options.Radius * options.Radius;
            for (int y = 0; y <= options.Radius; y++) // bottom to top
            {
                for (int x = 0; x <= options.Radius + options.Thickness; x++)
                {
                    for (int z = 0; z <= options.Radius + options.Thickness; z++)
                    {

                        var distance = x * x + z * z + y * y;
                        if ((!options.Fill && Math.Abs(d - distance) < options.Radius + options.Thickness)
                            || (options.Fill && distance <= d + options.Thickness))
                        {
                            points.Add(new Point { X = options.X + x, Y = options.Y + y, Z = options.Z + z });
                            points.Add(new Point { X = options.X - x, Y = options.Y + y, Z = options.Z - z });
                            points.Add(new Point { X = options.X - x, Y = options.Y + y, Z = options.Z + z });
                            points.Add(new Point { X = options.X + x, Y = options.Y + y, Z = options.Z - z });
                            points.Add(new Point { X = options.X + x, Y = options.Y - y, Z = options.Z + z });
                            points.Add(new Point { X = options.X - x, Y = options.Y - y, Z = options.Z - z });
                            points.Add(new Point { X = options.X - x, Y = options.Y - y, Z = options.Z + z });
                            points.Add(new Point { X = options.X + x, Y = options.Y - y, Z = options.Z - z });
                        }
                    }
                }
            }

            return points;
        }
    }
}