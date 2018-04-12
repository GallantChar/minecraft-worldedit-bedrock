using System;
using System.Collections.Generic;
using ShapeGenerator.Generators.HouseGen;

namespace ShapeGenerator.Generators
{
    public class Roof : IStructureBuilder
    {
        public RoomPoint OuterDoor;

        public Roof(Structure rc = null)
        {
            Structure = rc;
        }

        public Structure Structure { get; set; }

        public string RoofBlock { get; set; }

        public string SlabBlock { get; set; }

        public string WallBlock { get; set; }

        public List<RoomPoint> RenderPoints()
        {
            var rnd = new Random();
            var roofPoints = new List<RoomPoint>();
            if (OuterDoor != null && Structure.dx - OuterDoor.X > Structure.dz - OuterDoor.Z ||
                OuterDoor == null && rnd.Next(100) < 60)
            {
                for (var x = 0; x < Structure.dx; x++)
                {
                    Structure.GetZPointRange(new RoomPoint {X = x, Y = 0, Z = 0}, out var lower, out var higher);
                    if (lower == -1) continue;
                    int rise = 0;
                    var middle = lower + (higher - lower) / 2;
                    var middleOffset = (higher - lower) % 2;

                    //side 1
                    for (var z = lower; z < middle + middleOffset; z++)
                    {
                        roofPoints.AddRange(GetRoofPoints(ref rise, x, z, $"{RoofBlock} 2"));
                    }

                    //side 2
                    rise = 0;
                    for (var z = higher; z > middle; z--)
                    {
                        roofPoints.AddRange(GetRoofPoints(ref rise, x, z, $"{RoofBlock} 3"));
                    }

                    if (middleOffset != 0) continue;
                    var p = Structure.GetPoint(x, 0, middle);
                    if (p == null)
                    {
                        continue;
                    }

                    var y = Structure.GetPointHeight(p) + (higher - lower) / 2;
                    roofPoints.AddRange(GetRoofSupport(p, y));
                    roofPoints.Add(new RoomPoint {X = x, Y = y, Z = middle, Order = 0, BlockName = SlabBlock});
                }
            }
            else
            {
                for (var z = 0; z < Structure.dz; z++)
                {
                    Structure.GetXPointRange(new RoomPoint {X = 0, Y = 0, Z = z}, out var lower, out var higher);
                    if (lower == -1) continue;
                    int rise = 0;
                    var middle = lower + (higher - lower) / 2;
                    var middleOffset = (higher - lower) % 2;

                    // side 1
                    for (var x = lower; x < middle + middleOffset; x++)
                    {
                        roofPoints.AddRange(GetRoofPoints(ref rise, x, z, $"{RoofBlock} 0"));
                    }

                    // side 2
                    rise = 0;
                    for (var x = higher; x > middle; x--)
                    {
                        roofPoints.AddRange(GetRoofPoints(ref rise, x, z, $"{RoofBlock} 1"));
                    }

                    // middle
                    if (middleOffset != 0) continue;
                    var p = Structure.GetPoint(middle, 0, z);
                    if (p == null)
                    {
                        continue;
                    }

                    var y = Structure.GetPointHeight(p) + (higher - lower) / 2;
                    roofPoints.AddRange(GetRoofSupport(p, y));
                    roofPoints.Add(new RoomPoint {X = middle, Y = y, Z = z, Order = 0, BlockName = SlabBlock});
                }
            }

            return roofPoints;
        }


        private IEnumerable<RoomPoint> GetRoofPoints(ref int rise, int x, int z,
            string block)
        {
            var points = new List<RoomPoint>();
            var p = Structure.matrix[x, 0, z];
            if (p == null)
            {
                rise = 0;
                return points;
            }

            var y = Structure.GetPointHeight(p) + rise;

            points.AddRange(GetRoofSupport(p, y));

            rise++;

            points.Add(new RoomPoint {X = x, Y = y, Z = z, Order = 0, BlockName = block});
            return points;
        }

        /// <summary>
        ///     GetRoofSupport: Adds the supporting columns at the edge of the roof down to the ceiling of the highest room.  This
        ///     prevents open air.
        /// </summary>
        private IEnumerable<RoomPoint> GetRoofSupport(RoomPoint p, int y)
        {
            var points = new List<RoomPoint>();

            bool addSupport = Structure.GetPoint(p.X, 0, p.Z).BlockName == WallBlock;
            if (!addSupport)
            {
                Structure.GetZPointRange(p, out var lowerZ, out var higherZ);
                Structure.GetXPointRange(p, out var lowerX, out var higherX);

                var left = Structure.GetPoint(p.X - 1, 0, p.Z);
                var right = Structure.GetPoint(p.X + 1, 0, p.Z);
                var front = Structure.GetPoint(p.X, 0, p.Z + 1);
                var back = Structure.GetPoint(p.X, 0, p.Z - 1);

                addSupport = left == null || right == null || front == null || back == null;

                if (!addSupport)
                {
                    var h = Structure.GetPointHeight(p);
                    addSupport = h != Structure.GetPointHeight(left) ||
                                 h != Structure.GetPointHeight(right) ||
                                 h != Structure.GetPointHeight(front) ||
                                 h != Structure.GetPointHeight(back);
                }

                if (!addSupport)
                {
                    addSupport = CompareRange(left, lowerX, higherX, lowerZ, higherZ) ||
                                 CompareRange(right, lowerX, higherX, lowerZ, higherZ) ||
                                 CompareRange(front, lowerX, higherX, lowerZ, higherZ) ||
                                 CompareRange(back, lowerX, higherX, lowerZ, higherZ);
                }
            }

            if (addSupport)
            {
                for (var wallY = y - 1; wallY >= Structure.GetPointHeight(p); wallY--)
                {
                    points.Add(new RoomPoint {X = p.X, Y = wallY, Z = p.Z, Order = 0, BlockName = Structure.WallBlock});
                }
            }

            return points;
        }

        private bool CompareRange(RoomPoint point, int lowerX, int higherX, int lowerZ, int higherZ)
        {
            Structure.GetXPointRange(point, out var compareX, out var compreHighX);
            if (lowerX != compareX || compreHighX != higherX) return true;

            Structure.GetZPointRange(point, out var compareZ, out var compreHighZ);
            return lowerZ != compareZ || compreHighZ != higherZ;
        }


        /*
        /// <summary>
        /// NormalizePoints: Converts points list to matrix.  Matrix takes more memory, but is a lot faster to process and loop through
        /// </summary>
        /// <param name="points">List of room points.</param>
        /// <param name="dx">max x for matrix</param>
        /// <param name="dy">max y for matrix</param>
        /// <param name="dz">max z for matrix</param>
        /// <returns></returns>
        private static RoomPoint[,,] NormalizePoints(List<RoomPoint> points, out int dx, out int dy, out int dz)
        {
            var lx = points.Min(p => p.X);
            var ly = points.Min(p => p.Y);
            var lz = points.Min(p => p.Z);
            var hx = points.Max(p => p.X);
            var hy = points.Max(p => p.Y);
            var hz = points.Max(p => p.Z);
            dx = hx - lx + 1;
            dy = hy - ly + 1;
            dz = hz - lz + 1;
            var matrix = new RoomPoint[dx, dy, dz];

            foreach (var p in points)
            {
                p.X -= lx;
                p.Y -= ly;
                p.Z -= lz;
                matrix[p.X, p.Y, p.Z] = p.Clone();
            }

            return matrix;
        }
        */
    }
}