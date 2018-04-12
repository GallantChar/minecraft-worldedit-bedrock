using System;
using System.Collections.Generic;

namespace ShapeGenerator.Generators.HouseGen
{
    public class House : IStructureBuilder
    {
        public const string ABOVEDOOR = "!!AboveDoor!!";

        public House(int rooms)
        {
            NumberOfRooms = rooms;
        }

        public int NumberOfRooms { get; set; }
        public string CeilingBlock { get; set; }
        public string AirBlock { get; set; }
        public string EmptyBlock { get; set; }
        public string WallBlock { get; set; }
        public string InsideBlock { get; set; }
        public string GlassBlock { get; set; }
        public string InnerDoor { get; set; }
        public int Floors { get; set; }

        //public enum WindowStyles
        //{
        //    Windows4x4,
        //    WindowsSpan
        //}

        //public WindowStyles WindowStyle { get; set; }

        public List<RoomPoint> RenderPoints()
        {
            /*
             * int rooms, int floors, string WALL, List<RoomPoint> points, string AIR,
            string EMPTY, string INSIDE,
            string ABOVEDOOR, string GLASS)
             */
            Structure rc = new Structure(NumberOfRooms)
            {
                CeilingBlock = CeilingBlock,
                RoofBlock = "dark_oak_stairs",
                SlabBlock = "wooden_slab 5",
                WallBlock = WallBlock,
                AirBlock = AirBlock,
                InsideBlock = InsideBlock,
                GlassBlock = GlassBlock,
                EmptyBlock = EmptyBlock,
                InnerDoor = "spruce_door",
                Floors = Floors
            };
            rc.JoinRooms();

            var rnd = new Random();
            //WindowStyle = rnd.NextEnum<WindowStyles>();

            // add doors and windows
            RoomPoint outerdoor = null;
            for (int floor = 0; floor < rc.Floors; floor++)
            {
                var y = floor * 6 + 2;

                    for (var x = 0; x < rc.dx; x++)
                    {
                        for (var z = 0; z < rc.dz; z++)
                        {
                            if (rc.GetPoint(x, y, z)?.BlockName != WallBlock) continue;
                            var emptyLeft =
                                x <= 0 || rc.GetPoint(x - 1, y, z) == null ||
                                rc.GetPoint(x - 1, y, z).BlockName.StartsWith(AirBlock);
                            var emptyBack =
                                z <= 0 || rc.GetPoint(x, y, z - 1) == null ||
                                rc.GetPoint(x, y, z - 1).BlockName.StartsWith(AirBlock);
                            var emptyRight = x >= rc.dx - 1 || rc.GetPoint(x + 1, y, z) == null ||
                                             rc.GetPoint(x + 1, y, z).BlockName.StartsWith(AirBlock);
                            var emptyFront = z >= rc.dz - 1 || rc.GetPoint(x, y, z + 1) == null ||
                                             rc.GetPoint(x, y, z + 1).BlockName.StartsWith(AirBlock);

                            var left = emptyLeft ? EmptyBlock : rc.GetPoint(x - 1, y, z).BlockName;
                            var right = emptyRight ? EmptyBlock : rc.GetPoint(x + 1, y, z).BlockName;
                            var front = emptyFront ? EmptyBlock : rc.GetPoint(x, y, z + 1).BlockName;
                            var back = emptyBack ? EmptyBlock : rc.GetPoint(x, y, z - 1).BlockName;

                            if (emptyLeft && right.Equals(InsideBlock) || emptyRight && left.Equals(InsideBlock))
                            {
                                if (outerdoor == null && y == 2 && front == WallBlock && back == WallBlock)
                                {
                                    rc.GetZPointRange(rc.GetPoint(x, y - 1, z), out var lower, out var higher);
                                    var doorZ = rnd.Next(lower + 2,lower + 3);
                                    outerdoor = rc.GetPoint(x, y - 1, doorZ);
                                    outerdoor.BlockName = $"birch_door {(emptyLeft ? 0 : 2)}";
                                    rc.GetPoint(x, y, doorZ).BlockName = $"birch_door {(emptyLeft ? 0 + 8 : 2 + 8)}";
                                    rc.GetPoint(x, y + 1, doorZ).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y - 1,doorZ+1).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y, doorZ + 1).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y + 1, doorZ + 1).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y - 1, doorZ - 1).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y, doorZ - 1).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(x, y + 1, doorZ - 1).BlockName = ABOVEDOOR;
                                    continue;
                                }

                                // front to back
                                if ((front == GlassBlock || front == WallBlock) &&
                                    (back == GlassBlock || back == WallBlock) && z < rc.dz - 2 && z > 1)
                                {
                                    var front2 = rc.GetPoint(x, y, z + 2)?.BlockName ?? EmptyBlock;
                                    var back2 = rc.GetPoint(x, y, z - 2)?.BlockName ?? EmptyBlock;

                                    ApplyWindowStyle(rc, x, y, z, front2, back2);
                                }
                            }
                            else if (emptyFront && back.Equals(InsideBlock) || emptyBack && front.Equals(InsideBlock))
                            {
                                if (outerdoor == null && y == 2 && right == WallBlock && left == WallBlock)
                                {
                                    rc.GetXPointRange(rc.GetPoint(x, y - 1, z), out var lower, out var higher);
                                    var doorX = rnd.Next(lower + 2, lower + 3);
                                    outerdoor = rc.GetPoint(x, y - 1, z);
                                    outerdoor.BlockName = $"birch_door {(emptyLeft ? 1 : 3)}";
                                    rc.GetPoint(doorX, y, z).BlockName = $"birch_door {(emptyLeft ? 1 + 8 : 3 + 8)}";
                                    rc.GetPoint(doorX, y + 1, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX + 1, y - 1, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX + 1, y, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX + 1, y + 1, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX - 1, y - 1, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX - 1, y, z).BlockName = ABOVEDOOR;
                                    //rc.GetPoint(doorX - 1, y + 1, z).BlockName = ABOVEDOOR;
                                    continue;
                                }

                                // left to right
                                
                                if ((left == GlassBlock || left == WallBlock) &&
                                    (right == GlassBlock || right == WallBlock) && x < rc.dx - 2 && x > 1)
                                {
                                    var left2 = rc.GetPoint(x + 2, y, z)?.BlockName ?? EmptyBlock;
                                    var right2 = rc.GetPoint(x - 2, y, z)?.BlockName ?? EmptyBlock;

                                    ApplyWindowStyle(rc, x, y, z, left2, right2);
                                }
                            }
                        }
                }
            }

            rc.ReplaceBlocksByName(ABOVEDOOR, WallBlock);

            var points = rc.ToRoomPoints();

            // add roof
            var roof = new Roof(rc)
            {
                RoofBlock = "dark_oak_stairs",
                SlabBlock = "wooden_slab 5",
                WallBlock = WallBlock,
                OuterDoor = outerdoor
            };
            points = roof.RenderPoints();
            points.AddRange(rc.ToRoomPoints());

            rc.AddPoints(points);

            return rc.ToRoomPoints();
        }


        //TODO: Figure out how to get hte other window style logic to work right.
        private void ApplyWindowStyle(Structure rc, int x, int y, int z, string blockA, string blockB)
        {
            if (blockB == WallBlock && blockA == WallBlock)// && WindowStyle == WindowStyles.Windows4x4)
            {
                rc.GetPoint(x, y, z).BlockName = GlassBlock;
                rc.GetPoint(x, y+1, z).BlockName = GlassBlock;
            }
            //else if ((blockA == GlassBlock || blockA == WallBlock) &&
            //         (blockB == GlassBlock || blockB == WallBlock) && WindowStyle == WindowStyles.WindowsSpan)
            //{
            //    rc.GetPoint(x, y, z).BlockName = GlassBlock;
            //}
        }
    }
}