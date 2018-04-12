namespace ShapeGenerator.Generators
{
    public interface IHouseOptions
    { 
        Point Start { get; set; }
        int X { get; set; }
        int Z { get; set; }
        int Y { get; set; }
        string Block { get; set; }
        int Height { get; set; }
    }
}