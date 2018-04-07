namespace ShapeGenerator.Generators
{
    public interface ISquareOptions
    {
        Point Start { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        bool Fill { get; set; }
        int Length { get; set; }
        string Block { get; set; }
        int Thickness { get; set; }
        bool Merlon { get; set; } 
    }
}