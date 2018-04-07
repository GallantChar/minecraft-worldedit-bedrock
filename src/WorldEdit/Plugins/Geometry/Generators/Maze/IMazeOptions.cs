namespace ShapeGenerator.Generators
{
    public interface IMazeOptions
    {
        int Length { get; set; }
        int Width { get; set; }
        int Thickness { get; set; }
        int InnerThickness { get; set; }
        int Height { get; set; }
        Point Start { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        string Block { get; set; }
    }
}