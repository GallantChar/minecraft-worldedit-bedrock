namespace ShapeGenerator
{
    public interface ICircleOptions
    {
        Shape Shape { get; set; }
        int Radius { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        int Height { get; set; }
        int Thickness { get; set; }
        bool Fill { get; set; }
        string Block { get; set; }
    }
}