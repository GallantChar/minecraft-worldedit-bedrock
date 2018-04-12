namespace ShapeGenerator.Generators.HouseGen
{
    public class RoomPoint : Point
    {
        public int RoomIndex { get; set; }

        public new RoomPoint Clone(int increaseOrder = 0)
        {
            return new RoomPoint { X = X, Y = Y, Z = Z, Order = Order + increaseOrder, BlockName = BlockName, RoomIndex = RoomIndex };
        }
    }
}