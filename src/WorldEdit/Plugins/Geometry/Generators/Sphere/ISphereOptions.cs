namespace ShapeGenerator
{
    public interface ISphereOptions
    {
        int Radius { get; set; }
        Point Start { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        string Block { get; set; }
    }
}