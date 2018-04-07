namespace ShapeGenerator
{
    interface IOptions
    {
        Point Start { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        string Block { get; set; }
    }
}
