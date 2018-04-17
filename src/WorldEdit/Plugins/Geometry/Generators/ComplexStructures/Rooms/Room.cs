using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.ComplexStructures.Rooms
{
    public class Room : Structure
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string WallBlock { get; set; }

        public virtual string Name => "Room";

        public virtual void Build()
        {
            SetBounds(Width, Length, Height);

            for (int x = 1; x < Width - 1; x++)
            {
                for (int z = 1; z < Length - 1; z++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        Matrix[x,y,z] = new StructurePoint { X = x, Y = y, Z = 0, Structure = this, BlockType = BlockType.Inside };
                    }

                    Matrix[x, 0, z] = new StructurePoint { X = x, Y = 0, Z = z, Structure = this, BlockType = BlockType.Floor };
                    Matrix[x, Height-1, z] = new StructurePoint { X = x, Y = Height-1, Z = z, Structure = this, BlockType = BlockType.Ceiling };
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Matrix[x, y, 0] = new StructurePoint { X = x, Y = y, Z = 0, Structure = this, BlockType = BlockType.Wall };
                    Matrix[x, y, Length - 1] = new StructurePoint { X = x, Y = y, Z = Length - 1, Structure = this, BlockType = BlockType.Wall };
                }

                for (int z = 0; z < Length; z++)
                {
                    Matrix[0, y, z] = new StructurePoint { X = 0, Y = y, Z = z, Structure = this, BlockType = BlockType.Wall };
                    Matrix[Width-1, y, z] = new StructurePoint { X = Width-1, Y = y, Z = z, Structure = this, BlockType = BlockType.Wall };
                }
            }
        }
    }
}
