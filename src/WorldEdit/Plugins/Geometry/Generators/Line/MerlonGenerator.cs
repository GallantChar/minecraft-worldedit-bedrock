namespace ShapeGenerator.Generators
{
    public class MerlonGenerator : SquareGenerator
    {
        protected override bool TestForCoordinate(int x, int lowerX, int upperX, int z, int lowerZ, int upperZ,
            ISquareOptions opt, int y, int lowerY, int upperY)
        {
            return ((x - lowerX) % 2 ==0) && ((z - lowerZ) % 2 == 0);
            //|| x == upperX || z == lowerZ || z == upperZ || opt.Fill || y == lowerY || y == upperY;
        }
    }
}