using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.ComplexStructures.Rooms
{
    public class SprialStaircase : CircularRoom
    {

        public override void Build()
        {
            base.Build();
            if (Radius <= 1) return;
            var dr = Radius - 1;
            var d = dr * dr;

            int edgePoints = 0;
            for (int x = 0; x <= Width; x++)
            {
                for (int z = 0; z <= Length; z++)
                {
                    if (Matrix[x, 0, z].BlockType == BlockType.Wall)
                    {
                        edgePoints++;
                    }
                }
            }

            var center = new StructurePoint { X = Radius, Z = Radius };

            for (var m = 0; m <= edgePoints; m++)
            {
                double angle = -Math.PI / 2 + 2 * Math.PI * m / edgePoints;
                int x = Convert.ToInt32(Width * Math.Cos(angle));
                int z = Convert.ToInt32(Length * Math.Sin(angle));
                var y = m;
                while (y < Height) y -= Height;
                var p1 = center.Clone();
                p1.Y = y;

                var p2 = new StructurePoint { X = x, Z = z, Y = y };

                var block = BlockType.StepHalf;
                if (y%2==1) block = BlockType.StepFull;

                DrawLine(p1, p2, block);

                Matrix[x, y, z].BlockType = BlockType.Wall;
                Matrix[p2.X, p2.Y, p2.Z].BlockType = BlockType.Wall;
            }
        }

        private  void DrawLine(StructurePoint start, StructurePoint end, BlockType blockType)
        {
            Stack<Point[]> stack = new Stack<Point[]>();
            stack.Push(new[] { start, end });
            do
            {
                var p = stack.Pop();
                var delta = p[1].Subtract(p[0]);

                if (Math.Abs(delta.X) < 2 && Math.Abs(delta.Y) < 2 && Math.Abs(delta.Z) < 2) continue;

                var midPoint = Point.MidPoint(p[0], p[1], delta);
                Matrix[midPoint.X,midPoint.Y,midPoint.Z] = new StructurePoint { X = midPoint.X, Y = midPoint.Y, Z = midPoint.Z, Structure = this, BlockType = blockType };
                stack.Push(new[] { p[0], midPoint });
                stack.Push(new[] { midPoint, p[1] });
            } while (stack.Any());
        }
    }
}
