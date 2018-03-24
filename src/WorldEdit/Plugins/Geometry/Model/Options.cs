using ShapeGenerator.Generators;

namespace ShapeGenerator
{
    public class Options : ISphereOptions, ICircleOptions, ILineOptions, ISquareOptions, IPolygonOptions
    {
        public Shape Shape { get; set; }
        public int Height { get; set; }
        public bool Fill { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Z2 { get; set; }
        public Point Start => new Point {X = X, Y = Y, Z = Z};
        public Point End => new Point {X = X2, Y = Y2, Z = Z2};
        public int StartingAngle { get; set; }
        public int Steps { get; set; }
        public int Radius { get; set; }
        public int Sides { get; set; }
        public string Block { get; set; } = "stone";
        public int Thickness { get; set; } = 1;
        public bool Merlon { get; set; } = false;
        public int Length { get; set; }
        public int Width { get; set; }
    }
}