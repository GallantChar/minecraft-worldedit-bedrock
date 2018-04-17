using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.ComplexStructures.Rooms
{
    public class SquareRoom : Room
    {
        public override string Name => "SquareRoom";

        public override void Build()
        {
            Width = Math.Max(Length, Width);
            Length = Width;
            base.Build();
        }
    }
}
