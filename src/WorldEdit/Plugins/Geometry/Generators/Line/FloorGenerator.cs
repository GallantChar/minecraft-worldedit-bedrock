namespace ShapeGenerator.Generators
{
    public class FloorGenerator : SquareGenerator
    {
        protected override bool TestForCoordinate(int x, int lowerX, int upperX, int z, int lowerZ, int upperZ,
            ISquareOptions opt, int y, int lowerY, int upperY)
        {
            return y == lowerY;
        }
    }
}