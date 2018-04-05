using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators
{
    public class MazeGenerator : Generator, IGenerator
    {
        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        public List<Point> Run(IMazeOptions options)
        {
            List<Point> points = new List<Point>();
            return points;
        }
    }
}