using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators.Patterns
{
    public class CastleWithVines : ITransformer
    {
        public static string Name = "castle+vines";

        public void Transform(List<Point> points)
        {
            var rnd = new Random();
            var lowOrder = points.Min(p => p.Order);
            var hiOrder = points.Max(p => p.Order);
            for (var order = lowOrder; order <= hiOrder; order++)
            {
                var layer = points.Where(p => p.Order == order);
                if (layer == null || !layer.Any()) continue;
                var lx = layer.Min(p => p.X);
                var ly = layer.Min(p => p.Y);
                var lz = layer.Min(p => p.Z);
                var hx = layer.Max(p => p.X);
                var hy = layer.Max(p => p.Y);
                var hz = layer.Max(p => p.Z);
                var dx = hx - lx + 1;
                var dy = hy - ly + 1;
                var dz = hz - lz + 1;
                var matrix = new Point[dx, dy, dz];

                foreach (var p in layer)
                {
                    matrix[p.X - lx, p.Y - ly, p.Z - lz] = p;
                }

                for (var x = 0; x < dx; x++)
                {
                    for (var z = 0; z < dz; z++)
                    {
                        for (var y = 0; y < dy; y++)
                        {
                            if (matrix[x, y, z] == null) continue;
                            if (matrix[x, y, z].BlockName != "castle") continue;
                            matrix[x, y, z].BlockName = "stone 0";

                            var side = x == 0 || z == 0 || x == dx || z == dz;
                            var corner = x == 0 && z == 0 || x == dx && z == dz;

                            var emptyLeft = x > 0 && (matrix[x - 1, y, z] == null ||
                                            matrix[x - 1, y, z].BlockName.StartsWith("air"));
                            var emptyBack = z > 0 && (matrix[x, y, z - 1] == null ||
                                            matrix[x, y, z - 1].BlockName.StartsWith("air"));
                            var emptyRight = x + 1 < dx && (matrix[x + 1, y, z] == null ||
                                             matrix[x + 1, y, z].BlockName.StartsWith("air"));
                            var emptyFront = z + 1 < dz && (matrix[x, y, z + 1] == null ||
                                             matrix[x, y, z + 1].BlockName.StartsWith("air"));

                            corner = corner || ((emptyLeft || emptyRight) && (emptyFront || emptyBack));
                            side = side || corner || emptyLeft || emptyRight || emptyFront || emptyBack;

                            if (corner)
                            {
                                if (y == 0)
                                {
                                    matrix[x, y, z] = matrix[x, y, z].Clone(1);
                                    matrix[x, y, z].BlockName = "stonebrick " + (rnd.NextDouble() < 0.8D ? "0" : "2");
                                    points.Add(matrix[x, y, z]);
                                    continue;
                                }

                                if (matrix[x, y - 1, z].BlockName.StartsWith("stonebrick"))
                                {
                                    matrix[x, y, z] = matrix[x, y, z].Clone(1);
                                    matrix[x, y, z].BlockName = "stone 0";
                                    points.Add(matrix[x, y, z]);
                                    continue;
                                }

                                matrix[x, y, z] = matrix[x, y, z].Clone(1);
                                matrix[x, y, z].BlockName = "stonebrick " + (rnd.NextDouble() < 0.9D ? "0" : "2");
                                points.Add(matrix[x, y, z]);
                                continue;
                            }

                            if (!side) continue;


                            int px = 0, py = 0, pz = 0;
                            var vine = new Point();
                            if (emptyLeft)
                            {
                                px = x - 1; py = y; pz = z;
                                vine = (matrix[x - 1, y, z] ?? new Point { X = matrix[x, y, z].X - 1, Y = matrix[x, y, z].Y, Z = matrix[x, y, z].Z }).Clone(1);
                                vine.BlockName = "vine 8"; // east
                            }
                            else if (emptyRight)
                            {
                                px = x + 1; py = y; pz = z;
                                vine = (matrix[x + 1, y, z] ?? new Point { X = matrix[x, y, z].X + 1, Y = matrix[x, y, z].Y, Z = matrix[x, y, z].Z }).Clone(1);
                                vine.BlockName = "vine 2"; // west

                            }
                            else if (emptyBack)
                            {
                                px = x; py = y; pz = z - 1;
                                vine = (matrix[x, y, z - 1] ?? new Point { X = matrix[x, y, z].X, Y = matrix[x, y, z].Y, Z = matrix[x, y, z - 1].Z }).Clone(1);
                                vine.BlockName = "vine 1"; // south

                            }
                            else if (emptyFront)
                            {
                                px = x; py = y; pz = z + 1;
                                vine = (matrix[x, y, z + 1] ?? new Point { X = matrix[x, y, z].X, Y = matrix[x, y, z].Y, Z = matrix[x, y, z].Z + 1 }).Clone(1);
                                vine.BlockName = "vine 4"; // north
                            }
                            else continue;

                            emptyLeft = px > 0 && (matrix[px - 1, py, pz] == null ||
                                                      matrix[px - 1, py, pz].BlockName.StartsWith("air"));
                            emptyBack = pz > 0 && (matrix[px, py, pz - 1] == null ||
                                                      matrix[px, py, pz - 1].BlockName.StartsWith("air"));
                            emptyRight = px + 1 < dx && (matrix[px + 1, py, z] == null ||
                                                            matrix[px + 1, py, z].BlockName.StartsWith("air"));
                            emptyFront = pz + 1 < dz && (matrix[px, py, pz + 1] == null ||
                                                            matrix[px, py, pz + 1].BlockName.StartsWith("air"));

                            var vineChance = rnd.NextDouble();
                            if (!emptyLeft && matrix[px - 1, py, pz].BlockName.StartsWith("vine")) vineChance += 0.2D;
                            if (!emptyRight && matrix[px + 1, py, pz].BlockName.StartsWith("vine")) vineChance += 0.2D;
                            if (!emptyFront && matrix[px, py, pz - 1].BlockName.StartsWith("vine")) vineChance += 0.6D;
                            if (!emptyBack && matrix[px, py, pz + 1].BlockName.StartsWith("vine")) vineChance += 0.6D;
                            if (vineChance >= 0.8D)
                            {
                                points.Add(vine);
                            }

                        }
                    }
                }
            }
        }
    }
}