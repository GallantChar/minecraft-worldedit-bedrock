using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators.Patterns
{
    public class WeightedBlocks: IPattern
    {
        public List<WeightedBlock> Blocks { get; set; }

        private bool Sorted { get; set; }

        public static readonly Random Random = new Random();

        public WeightedBlocks()
        {
            Blocks = new List<WeightedBlock>();
            Sorted = false;
        }

        public virtual string GetBlock()
        {
            if (!Sorted) CalculateFrequencyAndSortBlocks();

            var chance = Random.NextDouble();
            var block = Blocks.FirstOrDefault(a=>a.CalculatedFrequency>=chance) ?? Blocks.LastOrDefault();

            return block?.BlockName;
        }

        private void CalculateFrequencyAndSortBlocks()
        {
            var total = Blocks.Sum(b => b.Frequency);
            Blocks.ForEach(a => a.CalculatedFrequency = a.Frequency / total);
            Blocks = Blocks.OrderBy(s => s.CalculatedFrequency).ToList();
            Sorted = true;
        }

        public virtual string GetBlock(List<Point> allPoints, Point currentPoint)
        {
            return GetBlock(); // default to random.
        }

        public bool IsSideCornerPoint(IEnumerable<Point> points, Point currentPoint)
        {
            var left = points.FirstOrDefault(b => b.X == currentPoint.X - 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (left != null && IsAir(left)) left = null;

            var back = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z - 1);
            if (back != null && IsAir(back)) back = null;
            if (left == null && back == null) return true;

            var right = points.FirstOrDefault(b => b.X == currentPoint.X + 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (right != null && IsAir(right)) right = null;
            if (right == null && back == null) return true;

            var front = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z + 1);
            if (front != null && IsAir(front)) front = null;
            return (left == null || right == null) && front == null;
        }

        public bool IsAir(Point point)
        {
            if (string.IsNullOrEmpty(point?.BlockName)) return true;
            var block = point.BlockName;
            return block == "air" || block == "air 0";
        }

        public bool IsSidePoint(IEnumerable<Point> points, Point currentPoint)
        {
            var left = points.FirstOrDefault(b => b.X == currentPoint.X - 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (left ==null || IsAir(left)) return true;
            var right = points.FirstOrDefault(b => b.X == currentPoint.X + 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (right == null || IsAir(right)) return true;
            var front = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z + 1);
            if (front == null || IsAir(front)) return true;
            var back = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z - 1);
            if (back == null || IsAir(back)) return true;
            return false;
        }

        public bool IsTopEdgePoint(IEnumerable<Point> points, Point currentPoint)
        {
            var top = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z);
            if (top != null && IsAir(top)) top = null;

            if (top != null) return false;

            var left = points.FirstOrDefault(b => b.X == currentPoint.X - 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (left != null && IsAir(left)) left = null;
            var right = points.FirstOrDefault(b => b.X == currentPoint.X + 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
            if (right != null && IsAir(right)) right = null;
            var front = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z + 1);
            if (front != null && IsAir(front)) front = null;
            var back = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z - 1);
            if (back != null && IsAir(back)) back = null;

            if (back != null && front != null && left != null && right != null) return false;

            if (left == null)
            {
                var upLeft = points.FirstOrDefault(b =>
                    b.X == currentPoint.X - 1 && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z);
                return upLeft == null || IsAir(upLeft);
            }

            if (right == null)
            {
                var upRight = points.FirstOrDefault(b =>
                    b.X == currentPoint.X + 1 && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z);
                return upRight == null || IsAir(upRight);
            }

            if (front == null)
            {
                var upFront = points.FirstOrDefault(b =>
                    b.X == currentPoint.X && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z + 1);
                return upFront == null || IsAir(upFront);
            }

            var upBack = points.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z - 1);
            return upBack == null || IsAir(upBack);
        }
    }
}
