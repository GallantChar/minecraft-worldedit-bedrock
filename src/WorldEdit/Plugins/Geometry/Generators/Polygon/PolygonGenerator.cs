using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators
{
    public class PolygonGenerator : Generator, IGenerator
    {
        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        public List<Point> Run(IPolygonOptions options)
        {
            if (options.Sides < 3)
            {
                throw new ArgumentException("Polygon must have 3 sides or more.");
            }

            List<Point> shapePoints = new List<Point>();
            float step = 360.0f / options.Sides;

            float end = 360.0f + options.StartingAngle;
            if (options.Steps < options.Sides)
            {
                end = step * options.Steps + options.StartingAngle;
            }
            
            float angle = options.StartingAngle; 
            for (double i = options.StartingAngle; i < end; i += step) 
            {
                shapePoints.Add(DegreesToXZ(angle, options.Radius, new Point{X=options.X, Z=options.Z, Y=options.Y})); 
                angle += step;
            }

            if (end >= 360.0f + options.StartingAngle)
            {
                shapePoints.Add(shapePoints[0].Clone());
            }

            var points = new List<Point>();
            for (var i = 0; i < shapePoints.Count-1; i++)
            {
                points.AddRange(LineGenerator.DrawLine(shapePoints[i], shapePoints[i+1]));
            }

            if (options.Fill)
            {
                var filledPoints = new List<Point>();
                foreach (var edgePoint in points)
                {
                    filledPoints.AddRange(LineGenerator.DrawLine(options.Start, edgePoint));
                }

                points = filledPoints;
            }
                
            return points;
        }

        // <summary>
        /// Calculates a point that is at an angle from the origin (0 is to the right)
        /// </summary>
        private Point DegreesToXZ(float degrees, float radius, Point origin)
        {
            Point point = new Point();
            double radians = degrees * Math.PI / 180.0;

            return new Point
            {
                X= (int) Math.Truncate(Math.Cos(radians) * radius + origin.X),
                Y = origin.Y,
                Z= (int) Math.Truncate(Math.Sin(-radians) * radius + origin.Z)
            };
            
        }
       
    }
}