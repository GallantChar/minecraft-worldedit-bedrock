using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators.HouseGen
{
    public class Room : IStructureBuilder
    {
        public enum DoorFacing
        {
            East = 0,
            South = 1,
            West = 2,
            North = 3
        }
        
        private static readonly Random rnd = new Random();

        private readonly string floorBlock;
        public bool HasWindows;

        public void JoinToRoom(Direction direction, Room room, bool bothdirections = false)
        {
            switch (direction)
            {
                case Direction.North:
                    North = room;
                    if (bothdirections)
                    {
                        room.JoinToRoom(Direction.South, this);
                        room.OffsetZ = OffsetZ + (Length - 1);
                    }
                    break;
                case Direction.East:
                    East = room;
                    if (bothdirections)
                    {
                        room.JoinToRoom(Direction.West, this);
                        room.OffsetX = OffsetX + (Width - 1);
                    }
                    break;
                case Direction.South:
                    South = room;
                    if (bothdirections)
                    {
                        room.JoinToRoom(Direction.North, this);
                        room.OffsetZ = OffsetZ - (room.Length - 1);
                    }
                    break;
                case Direction.West:
                    West = room;
                    if (bothdirections)
                    {
                        room.OffsetX = OffsetX - (room.Width - 1);
                        room.JoinToRoom(Direction.East, this);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public Room(int index, Structure structure, int floors = 0)
        {
            RoomIndex = index;

            Width = rnd.Next(5, 20);
            Length = rnd.Next(5, 20);

            if (floors == 0)
            {
                Floors = 1;
                if (Width + Length >= 25 && rnd.Next(100) < 90)
                {
                    Floors++;
                }

                if (Width + Length >= 30 && rnd.Next(100) < 20)
                {
                    Floors++;
                }
            }
            else
            {
                Floors = floors;
            }

            Height = Floors * 6;

            //matrix = new RoomPoint[Width, Height, Length];
            Structure = structure;

            DoubleDoors = Width + Length >= 25;

            floorBlock = RandomBuildingBlock();

            HasWindows = true; //rnd.Next(100) < 80;
        }

        public int RoomIndex { get; set; }
        public int Width { get; set; }

        public int Length { get; set; }
        public int Height { get; set; }
        public int OffsetX { get; set; }

        public int OffsetZ { get; set; }
        public int OffsetY { get; set; }

        public int Floors { get; set; }
        public Structure Structure { get; set; }
        private bool DoubleDoors { get; }

        public Room North { get; set; }
        public Room South { get; set; }
        public Room East { get; set; }
        public Room West { get; set; }
        

        public List<RoomPoint> RenderPoints()
        {
            //if (_shape == RoomShape.Circular) return RenderCircularRoom();

            var points = new List<RoomPoint>();

            for (int x = 1; x < Width - 1; x++)
            {
                for (int z = 1; z < Length - 1; z++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        SetBlock(points, x, y, z, Structure.InsideBlock);
                    }

                    for (int floor = 0; floor < Floors; floor++)
                    {
                        var y = floor * 6;
                        SetBlock(points, x, y, z, floorBlock);
                        SetBlock(points, x, y + 5, z, Structure.CeilingBlock);
                    }
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetBlock(points, x, y, 0, Structure.WallBlock);
                    SetBlock(points, x, y, Length - 1, Structure.WallBlock);
                }

                for (int z = 0; z < Length; z++)
                {
                    SetBlock(points, 0, y, z, Structure.WallBlock);
                    SetBlock(points, Width - 1, y, z, Structure.WallBlock);
                }
            }
            

            if (South != null)
            {
                var doorX = Math.Min(South.Width, Width) / 2;
                var doorZ = 0;
                AddDoors(points, doorX, doorZ, DoorFacing.South, Direction.South);
            }
            if (North != null)
            {
                var doorX = Math.Min(North.Width, Width) / 2;
                var doorZ = Length-1;
                AddDoors(points, doorX, doorZ, DoorFacing.North, Direction.North);
            }
            if (East != null)
            {
                var doorZ = Math.Min(East.Length, Length) / 2;
                var doorX = Width -1;
                AddDoors(points, doorX, doorZ, DoorFacing.East, Direction.East);
            }
            if (West != null)
            {
                var doorZ = Math.Min(West.Length, Length) / 2;
                var doorX = 0;
                AddDoors(points, doorX, doorZ, DoorFacing.West, Direction.West);
            }

            return points;
        }

        private void AddDoors(List<RoomPoint> points, int doorX, int doorZ, DoorFacing door, Direction direction)
        {
            var doorAonLeft = true;
            if (DoubleDoors)
            {
                var doorX2 = doorX;
                var doorZ2 = doorZ;
                switch (direction)
                {
                    case Direction.North: //south
                        doorX2--;
                        doorAonLeft = door == DoorFacing.North;
                        break;
                    case Direction.East: //east;
                        doorZ2++;
                        doorAonLeft = door == DoorFacing.East;
                        break;
                    case Direction.South:
                        doorX2++;
                        doorAonLeft = door == DoorFacing.North;
                        break;
                    case Direction.West:
                        doorZ2--;
                        doorAonLeft = door == DoorFacing.West;
                        break;

                }
                SetBlock(points, doorX2, 1, doorZ2, $"{Structure.InnerDoor} {(int)door}", 1);
                SetBlock(points, doorX2, 2, doorZ2, $"{Structure.InnerDoor} {(doorAonLeft ? 9 : 8)}", 2);
            }
            SetBlock(points, doorX, 1, doorZ, $"{Structure.InnerDoor} {(int)door}", 3);
            SetBlock(points, doorX, 2, doorZ, $"{Structure.InnerDoor} {(doorAonLeft ? 8 : 9)}", 4);

        }


        public static string RandomBuildingBlock()
        {
            var val = rnd.Next(rnd.Next(100) < 30 ? 6 : 20);

            switch (val)
            {
                case 0: return "planks 0";
                case 1: return "planks 1";
                case 2: return "planks 2";
                case 3: return "planks 3";
                case 4: return "planks 4";
                case 5: return "planks 5";
                case 6: return "concrete 12"; //brown
                case 7: return "concrete 7"; //dark gray
                case 8: return "concrete 13"; //dark green
                case 9: return "stained_hardened_clay 0"; //white terracotta
                case 10: return "stained_hardened_clay 1"; //orange terracotta
                case 11: return "stained_hardened_clay 8"; //light gray terracotta
                case 12: return "stained_hardened_clay 10"; //purple terracotta
                default: return "planks 0";
            }
        }

        private void SetBlock(List<RoomPoint> points, int x, int y, int z, string block, int order = 0)
        {
            var point = new RoomPoint
            {
                X = x + OffsetX,
                Z = z + OffsetZ,
                Y = y + OffsetY,
                BlockName = block,
                Order = order,
                RoomIndex = RoomIndex
            };
            points.Add(point);
        }
    }
}