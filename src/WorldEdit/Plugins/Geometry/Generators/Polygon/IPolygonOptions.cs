namespace ShapeGenerator.Generators
{
    public interface IPolygonOptions
    {
        Point Start { get; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        int Radius { get; set; }
        bool Fill { get; set; }
        int Height { get; set; }
        int Sides { get; set; }
        int Steps { get; set; }
        int StartingAngle { get; set; }
        string Block { get; set; }
    }
}