using System;
using ShapeGenerator.Generators;

namespace ShapeGenerator
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point Clone()
        {
            return new Point { X = X, Y = Y, Z = Z };
        }
        private static double Distance(int centerX, int centerZ, int centerY, int x, int z, int y)
        {
            return Math.Round(Math.Sqrt(Math.Pow(centerX - x, 2) + Math.Pow(centerZ - z, 2) + Math.Pow(centerY - y, 2)), 0);
        }
        public double Distance(Point from)
        {
            return Distance(X, Z, Y, from.X, from.Z, from.Y);
        }
        public double Distance2D(Point from)
        {
            return Distance(X, Z, 0, from.X, from.Z, 0);
        }

        public Point Offset(int x, int y, int z)
        {
            return new Point { X = X + x, Y = Y + y, Z = Z + z };
        }
        public bool Equals(Point o)
        {
            return X == o.X && Y == o.Y && Z == o.Z;
        }
        public bool Equals(int ox, int oy, int oz)
        {
            return X == ox && Y == oy && Z == oz;
        }

        public Point Subtract(Point p)
        {
            return new Point {X = X - p.X, Y = Y - p.Y, Z = Z - p.Z};
        }

        public static Point MidPoint(Point p1, Point p2, Point delta = null)
        {
            if (delta == null)
            {
                delta = p2.Subtract(p1);
            }

            var mid = p1.Clone();
            var x = (double) mid.X + (delta.X != 0 ? (double) delta.X / 2 : 0);
            var y = (double) mid.Y + (delta.Y != 0 ? (double) delta.Y / 2 : 0);
            var z = (double) mid.Z + (delta.Z != 0 ? (double) delta.Z / 2 : 0);

            mid.X = Convert.ToInt32(delta.X > 0 ? Math.Ceiling(x) : Math.Truncate(x));
            mid.Y = Convert.ToInt32(delta.Y > 0 ? Math.Ceiling(y) : Math.Truncate(y));
            mid.Z = Convert.ToInt32(delta.Z> 0 ? Math.Ceiling(z) : Math.Truncate(z));

            return mid;
        }

    }
}