using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators.HouseGen
{

    public enum Direction
    {
        North=0,
        East=1,
        South=2,
        West=3
    }

    public class Structure
    {
        public Structure()
        {
        }

        public Structure(int rooms)
        {
            int floors = 0;
            this.rooms = new Room[rooms];
            for (var i = 0; i < this.rooms.Length; i++)
            {
                this.rooms[i] = new Room(i, this, Floors);
                floors = Math.Max(floors, this.rooms[i].Floors);
            }

            Floors = floors;
        }

        public RoomPoint[,,] matrix { get; private set; }
        public int dx { get; set; }
        public int dy { get; set; }
        public int dz { get; set; }
        public Room[] rooms { get; }
        public int Floors { get; set; }

        public string CeilingBlock { get; set; }
        public string RoofBlock { get; set; }
        public string SlabBlock { get; set; }
        public string WallBlock { get; set; }
        public string InnerDoor { get; set; }
        public string InsideBlock { get; set; }
        public string AirBlock { get; set; }
        public string GlassBlock { get; set; }
        public string EmptyBlock { get; set; }

        public void SetBounds(int dx, int dy, int dz)
        {
            if (matrix == null)
            {
                matrix = new RoomPoint[dx, dy, dz];
                this.dx = dx;
                this.dy = dy;
                this.dz = dz;
                return;
            }

            var newMatrix = new RoomPoint[dx, dy, dz];
            for (var x = 0; x < this.dx; x++)
            for (var y = 0; y < this.dy; y++)
            for (var z = 0; z < this.dz; z++)
                if (x < dx && y < dy && z < dz)
                    newMatrix[x, y, z] = matrix[x, y, z];

            this.dx = dx;
            this.dy = dy;
            this.dz = dz;

            matrix = newMatrix;
        }

        public List<RoomPoint> ToRoomPoints()
        {
            var points = new List<RoomPoint>();
            for (var x = 0; x < dx; x++)
            for (var y = 0; y < dy; y++)
            for (var z = 0; z < dz; z++)
                if (matrix[x, y, z] != null)
                    points.Add(matrix[x, y, z]);

            return points;
        }

        public void ReplaceBlocksByName(string oldName, string newName)
        {
            for (var x = 0; x < dx; x++)
            for (var y = 0; y < dy; y++)
            for (var z = 0; z < dz; z++)
                if (matrix[x, y, z] != null)
                    matrix[x, y, z].BlockName = matrix[x, y, z].BlockName.Replace(oldName, newName);
        }

        public int GetPointHeight(RoomPoint p)
        {
            return rooms[p.RoomIndex].Height;
        }

        public RoomPoint GetPoint(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= dx || y >= dx || z >= dz) return null;
            return matrix[x, y, z];
        }

        public void AddPoints(IEnumerable<RoomPoint> points)
        {
            var lx = points.Min(p => p.X);
            var ly = points.Min(p => p.Y);
            var lz = points.Min(p => p.Z);
            var hx = points.Max(p => p.X);
            var hy = points.Max(p => p.Y);
            var hz = points.Max(p => p.Z);

            if (lx < 0 || lz < 0 || ly < 0 || hx >= dx || hy >= dy || hz >= dz)
            {
                var ndx = Math.Max(hx - lx + 1, dx);
                var ndy = Math.Max(hy - ly + 1, dy);
                var ndz = Math.Max(hz - lz + 1, dz);
                SetBounds(ndx, ndy, ndz);
            }

            foreach (var p in points)
            {
                var np = p.Clone();
                np.X -= lx;
                np.Y -= ly;
                np.Z -= lz;
                matrix[np.X, np.Y, np.Z] = np;
            }
        }

        public void GetXPointRange(RoomPoint currentPoint, out int lower,
            out int higher, int? dx = null)
        {
            GetPointRange(currentPoint, dx ?? this.dx, null, out lower, out higher);
        }

        public void GetZPointRange(RoomPoint currentPoint, out int lower,
            out int higher, int? dz = null)
        {
            GetPointRange(currentPoint, null, dz ?? this.dz, out lower, out higher);
        }

        public void GetPointRange(RoomPoint currentPoint, int? dx, int? dz, out int lower,
            out int higher)
        {
            lower = -1;
            higher = -1;

            for (var pos = 0; pos < (dx ?? dz); pos++)
            {
                if (matrix[dz.HasValue ? currentPoint.X : pos, 0, dx.HasValue ? currentPoint.Z : pos] == null) continue;
                if (lower == -1) lower = pos;
                higher = pos;
            }
        }


        public void JoinRooms()
        {
            var rnd = new Random();
            if (rooms.Length > 1)
            {
                var joinedRooms = new bool[rooms.Length, 4];
                for (var roomIndex = 1; roomIndex < rooms.Length; roomIndex++)
                {

                    // Find joinable rooms
                    Direction side;
                    int oppRoomIndex;

                    do
                    {
                        side = rnd.NextEnum<Direction>();
                        oppRoomIndex = rnd.Next(roomIndex - 1);
                    } while (joinedRooms[oppRoomIndex, (int)side]);

                    // get room and opposite side and room
                    var oppSide = OppositeDirection(side);
                    var room = rooms[roomIndex];
                    var oppRoom = rooms[oppRoomIndex];

                    // set rooms as joined
                    joinedRooms[oppRoomIndex, (int)side] = true;
                    joinedRooms[roomIndex, (int)oppSide] = true;
                    
                    //join rooms
                    oppRoom.JoinToRoom(oppSide, room, true);

                    //Mark Overlapping Rooms Joined
                    joinedRooms = MarkOverlappingRoomsJoined(joinedRooms, room, oppRoom, side, oppSide, roomIndex, oppRoomIndex);

                    // This is more for troubleshooting roomgen
                    Console.WriteLine(
                        $"Room {oppRoomIndex} ({oppRoom.Width}x{oppRoom.Height}) {oppSide.ToString()} Side is joined to Room {roomIndex} ({room.Width}x{room.Height}) {side.ToString()} Side");
                    
                    if (room.Floors > Floors) Floors = room.Floors;
                }
            }

            //taller overrides shorter rooms
            var points = new List<RoomPoint>();
            foreach (var r in rooms.OrderBy(r => r.Height))
            {
                var renderedRoom = r.RenderPoints();
                renderedRoom.Where(p => p.BlockName == CeilingBlock).ToList()
                    .ForEach(p => p.BlockName = $"concrete {r.RoomIndex}");
                points.AddRange(renderedRoom);
            }

            AddPoints(points);
        }

        private static bool[,] MarkOverlappingRoomsJoined(bool[,] joinedRooms, Room room, Room oppRoom, Direction side,
            Direction oppSide, int roomIndex, int oppRoomIndex)
        {
            var sideA = ShiftCounterClockwise(oppSide);
            var sideB = ShiftCounterClockwise(side);
            if (oppSide == Direction.North || oppSide == Direction.South)
            {
                MarkOverlappingRoomsJoined(ref joinedRooms, room.Width, oppRoom.Width, roomIndex,
                    oppRoomIndex, sideA, sideB);
            }
            else
            {
                MarkOverlappingRoomsJoined(ref joinedRooms, room.Length, oppRoom.Length, roomIndex,
                    oppRoomIndex, sideA, sideB);
            }

            return joinedRooms;
        }

        private static Direction OppositeDirection(Direction side)
        {
            return (Direction)((int)side < 2 ? (int)side + 2 : (int)side - 2);
        }

        private static Direction ShiftCounterClockwise(Direction side)
        {
            return (Direction)((int)side < 3 ? (int)side + 1 : (int)side - 1);
        }

        private static void MarkOverlappingRoomsJoined(ref bool[,] joinedRooms, int sizeA, int sizeB, int indexA,
            int indexB, Direction sideA, Direction sideB)
        {
            if (sizeA == sizeB)
            {
                return;
            }
            if (sizeA > sizeB)
            {
                joinedRooms[indexA, (int)sideA] = true;
                joinedRooms[indexB, (int)sideB] = true;
            }
            else
            {
                joinedRooms[indexA, (int)sideB] = true;
                joinedRooms[indexB, (int)sideA] = true;
            }
        }
    }
}