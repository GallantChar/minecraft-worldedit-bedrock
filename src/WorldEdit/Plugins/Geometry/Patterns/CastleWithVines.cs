using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    public class CastleWithVines : WeightedBlocks
    {
        public static string Name = "castle+vines";

        public CastleWithVines()
        {
            Blocks.Add(new WeightedBlock("stonebrick 0", 10));
            Blocks.Add(new WeightedBlock("stone 0", 80));
            Blocks.Add(new WeightedBlock("stonebrick 2", 7));
            Blocks.Add(new WeightedBlock("stonebrick 1", 3));
        }

        public override string GetBlock(List<Point> allPoints, Point currentPoint)
        {
            if (currentPoint.BlockName != Name) return currentPoint.BlockName;

            // alternating stone on corners
            if (IsSideCornerPoint(allPoints, currentPoint))
            {
                var below = allPoints.FirstOrDefault(b =>
                    b.X == currentPoint.X && b.Y == currentPoint.Y - 1 && b.Z == currentPoint.Z);
                if (below == null)
                {
                    currentPoint.BlockName = "stonebrick " + (RandomGenerator.NextDouble() < 0.8D ? "0" : "2");
                    
                } else if (below.BlockName.StartsWith("stonebrick"))
                {
                    currentPoint.BlockName = "stone 0";
                }
                else
                {
                    currentPoint.BlockName = "stonebrick " + (RandomGenerator.NextDouble() < 0.9D ? "0" : "2");
                }
                return currentPoint.BlockName;
            }
            // top
            var above = allPoints.FirstOrDefault(b =>
                b.X == currentPoint.X && b.Y == currentPoint.Y + 1 && b.Z == currentPoint.Z);
            if (above == null || IsAir(above))
            {
                currentPoint.BlockName = "stonebrick " + (RandomGenerator.NextDouble() < 0.8D ? "0" : "2");
            }

            // wall side...

            if (IsSidePoint(allPoints, currentPoint))
            {
                var vineChance = RandomGenerator.NextDouble();
                int facing = 0;
                Point vpoint = null;
                var left = allPoints.FirstOrDefault(b => b.X == currentPoint.X - 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
                if (left == null || IsAir(left))
                {
                    vpoint = left ?? new Point { X = currentPoint.X - 1, Y = currentPoint.Y, Z = currentPoint.Z };
                    facing = 8; // east
                }

                var right = allPoints.FirstOrDefault(b => b.X == currentPoint.X + 1 && b.Y == currentPoint.Y && b.Z == currentPoint.Z);
                if (right == null || IsAir(right))
                {
                    vpoint = right ?? new Point { X = currentPoint.X + 1, Y = currentPoint.Y, Z = currentPoint.Z };
                    facing = 2; // west
                }

                var front = allPoints.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z + 1);
                if (front == null || IsAir(front))
                {
                    vpoint = front ?? new Point { X = currentPoint.X, Y = currentPoint.Y, Z = currentPoint.Z + 1 };
                    facing = 4; // north

                }

                var back = allPoints.FirstOrDefault(b => b.X == currentPoint.X && b.Y == currentPoint.Y && b.Z == currentPoint.Z - 1);
                if (back == null || IsAir(back))
                {
                    vpoint = back ?? new Point { X = currentPoint.X, Y = currentPoint.Y, Z = currentPoint.Z - 1 };
                    facing = 1; // south
                }

                if (vpoint != null)
                {
                    left = allPoints.FirstOrDefault(b => b.X == vpoint.X - 1 && b.Y == vpoint.Y && b.Z == vpoint.Z);
                    right = allPoints.FirstOrDefault(b => b.X == vpoint.X + 1 && b.Y == vpoint.Y && b.Z == vpoint.Z);
                    above = allPoints.FirstOrDefault(b => b.X == vpoint.X && b.Y == vpoint.Y + 1 && b.Z == vpoint.Z);
                    var below = allPoints.FirstOrDefault(b =>
                        b.X == vpoint.X && b.Y == vpoint.Y - 1 && b.Z == vpoint.Z);

                    if (left != null && left.BlockName.StartsWith("vine")) vineChance += 0.2D;
                    if (right != null && right.BlockName.StartsWith("vine")) vineChance += 0.2D;
                    if (above != null && above.BlockName.StartsWith("vine")) vineChance += 0.6D;
                    if (below != null && below.BlockName.StartsWith("vine")) vineChance += 0.6D;

                    // 20% chance unless surrounded by more vines...
                    if (vineChance >= 0.8D)
                    {
                        var vine = vpoint.Clone();
                        vine.BlockName = $"vine {facing}";
                        allPoints.Add(vine);
                    }
                }
            }

            return GetBlock();
        }
    }
}
