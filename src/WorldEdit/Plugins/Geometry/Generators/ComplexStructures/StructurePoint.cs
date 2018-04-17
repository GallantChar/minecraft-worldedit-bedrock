namespace ShapeGenerator.Generators.ComplexStructures
{

    public enum BlockType
    {
        Air,
        Inside,
        Wall,
        Floor,
        Window,
        Ceiling,
        Door,
        StepFull
    }

    public class StructurePoint : Point
    {
        public BlockType BlockType { get; set; }

        public Structure Structure { get; set; }

        public new StructurePoint Clone(int increaseOrder = 0)
        {
            return new StructurePoint { X=X, Y=Y, Z=Z, Order = Order + increaseOrder, BlockType = BlockType, Structure = Structure };
        }
    }
}