using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    public class Castle : WeightedBlocks
    {
        public static string Name = "castle";

        public Castle()
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
            return GetBlock();
        }
    }
}
