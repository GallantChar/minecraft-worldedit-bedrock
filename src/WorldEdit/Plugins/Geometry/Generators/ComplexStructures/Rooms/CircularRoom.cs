using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.ComplexStructures.Rooms
{
    public class CircularRoom : SquareRoom
    {
        public int Radius { get; set; }
        public override void Build()
        {
            Radius = Math.Max(Length, Width)/2;
            if (Radius <= 1) return;

            var diameter = Radius*2;
            Width = diameter;
            Length = diameter;
            SetBounds(Width, Length, Height);

            Radius--;
            var d = Radius * Radius;
            Radius++;

            for (int x = 0; x <= Radius; x++)
            {
                for (int z = 0; z <= Radius; z++)
                {
                    var distance = x * x + z * z;
                    for (int y = 0; y < Height; y++)
                    {
                        if (Math.Abs(d - distance) < Radius)
                        {
                            Matrix[Radius + x, y, Radius + z] = new StructurePoint { X = Radius + x, Y = y, Z = Radius + z, Structure = this, BlockType = BlockType.Wall };
                            Matrix[x - Radius, y, Radius - z] = new StructurePoint { X = Radius - x, Y = y, Z = Radius - z, Structure = this, BlockType = BlockType.Wall };
                            Matrix[x - Radius, y, Radius + z] = new StructurePoint { X = Radius - x, Y = y, Z = Radius + z, Structure = this, BlockType = BlockType.Wall };
                            Matrix[Radius + x, y, Radius - z] = new StructurePoint { X = Radius + x, Y = y, Z = Radius - z, Structure = this, BlockType = BlockType.Wall };

                        }
                        else if (distance < d)
                        {
                            Matrix[Radius + x, y, Radius + z] = new StructurePoint { X = Radius + x, Y = y, Z = Radius + z, Structure = this, BlockType = BlockType.Inside };
                            Matrix[Radius - x, y, Radius - z] = new StructurePoint { X = Radius - x, Y = y, Z = Radius - z, Structure = this, BlockType = BlockType.Inside };
                            Matrix[Radius - x, y, Radius + z] = new StructurePoint { X = Radius - x, Y = y, Z = Radius + z, Structure = this, BlockType = BlockType.Inside };
                            Matrix[Radius + x, y, Radius - z] = new StructurePoint { X = Radius + x, Y = y, Z = Radius - z, Structure = this, BlockType = BlockType.Inside };
                        }
                    }
                }
            }
        }
    }
}
