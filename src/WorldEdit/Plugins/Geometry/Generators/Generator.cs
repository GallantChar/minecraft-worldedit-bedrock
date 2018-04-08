using System;
using System.Collections.Generic;
using System.Linq;

namespace ShapeGenerator.Generators
{
    public class Generator
    {
        public static List<Line> LinesFromPoints(List<Point> points, Options options)
        {
            // Creates a 3D matrix and utilizes the matrix to create lines
            // this runs much faster than using linq to search the list.
            var lines = new List<Line>();
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

                            var start = matrix[x, y, z].Clone();
                            //var end = matrix[x, y, z];

                            int ex = x;
                            while (ex < dx - 1 && matrix[ex + 1, y, z] != null && matrix[ex + 1, y, z].BlockName == matrix[x, y, z].BlockName) ex++;

                            int ey = y;
                            while (ey < dy - 1 && matrix[x, ey + 1, z] != null && matrix[x, ey + 1, z].BlockName == matrix[x, y, z].BlockName) ey++;

                            int ez = z;
                            while (ez < dz - 1 && matrix[x, y, ez + 1] != null && matrix[x, y, ez + 1].BlockName == matrix[x, y, z].BlockName) ez++;

                            var max = Math.Max(Math.Max(ex, ez), ey);

                            if (max == ex)
                            {
                                var end = matrix[max, y, z].Clone();
                                lines.Add(new Line
                                {
                                    Start = start,
                                    End = end,
                                    Block = start.BlockName,
                                    Order = start.Order
                                });
                                for (var rx = x; rx <= max; rx++)
                                {
                                    matrix[rx, y, z] = null;
                                }
                            }
                            else if (max == ey)
                            {
                                var end = matrix[x, max, z].Clone();
                                lines.Add(new Line
                                {
                                    Start = start,
                                    End = end,
                                    Block = start.BlockName,
                                    Order = start.Order
                                });
                                for (var ry = y; ry <= max; ry++)
                                {
                                    matrix[x, ry, z] = null;
                                }
                            }
                            else
                            {
                                var end = matrix[x, y, max].Clone();
                                lines.Add(new Line
                                {
                                    Start = start,
                                    End = end,
                                    Block = start.BlockName,
                                    Order = start.Order
                                });
                                for (var rz = z; rz <= max; rz++)
                                {
                                    matrix[x, y, rz] = null;
                                }
                            }
                        }
                    }
                }
            }

            if (!lines.Any()) return lines;

            var maxOrder = lines.Max(p => p.Order);
            for (int order = 0; order <= maxOrder; order++)
            {
                ConsolidateLines(lines, order);
            }

            return lines;
        }

        private static void ConsolidateLines(List<Line> lines, int order)
        {
            var small = lines.Where(l => l.Order == order);
            SquashLines(lines, small.OrderBy(a => a.Start.X).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.Y));
            SquashLines(lines, small.OrderBy(a => a.Order).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.X).ThenBy(a => a.Start.Z));
            SquashLines(lines, small.OrderBy(a => a.Order).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.X));
            SquashLines(lines, small.OrderBy(a => a.Order).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.X).ThenBy(a => a.Start.Y));
            SquashLines(lines, small.OrderBy(a => a.Order).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.X));
            SquashLines(lines, small.OrderBy(a => a.Order).ThenBy(a => a.Start.X).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.Z));

            var split = SplitLinesIntoMaxSizes(lines);
            lines.Clear();
            lines.AddRange(split);
        }

        public static List<Line> SplitLinesIntoMaxSizes(List<Line> lines)
        {
            var output = new List<Line>();

            foreach (var line in lines)
            {
                if (line.AreaSmallerThan(32768))
                {
                    // if (line.IsSmallerThen(20))

                    output.Add(line);
                }
                else //need to split the line into  smaller segments.
                {
                    output.AddRange(line.SplitToAMaxSize(31));
                }
            }

            return output;
        }

        public static List<Line> SquashLines(List<Line> master, IEnumerable<Line> list = null)
        {
            if (list == null) list = master;
            var lookat = list.ToList();

            for (var i = 0; i < lookat.Count - 1; i++)
            {
                for (var j = i + 1; j < lookat.Count; j++)
                {
                    if (!lookat[i].CanCombine(lookat[j])) continue;
                    lookat[i].Combine(lookat[j]);
                    master.Remove(lookat[j]);
                    lookat.RemoveAt(j);
                    i--;
                    break;
                }
            }

            return master;
        }

        public List<Line> TransformToLines(List<Point> points, Options options)
        {
            // set patterns

            points.Where(p => string.IsNullOrEmpty(p.BlockName)).ToList().ForEach(p => p.BlockName = options.Block);

            points = points.Distinct().ToList();

            foreach (var transformer in Patterns.Patterns.Transformers)
                transformer.Value.Transform(points);

            // limited to 20 passes. Allowing patterns to utilize patterns in their logic.
            var passes = 0;
            var pointsToProcess = points.Where(p => Patterns.Patterns.Blocks.Keys.Contains(p.BlockName))
                .OrderBy(a => a.Y).ThenBy(a => a.X).ThenBy(a => a.Z).ToList();
            do
            {
                pointsToProcess.ForEach(p => { p.BlockName = Patterns.Patterns.GetBlock(points, p); });
                passes++;
                pointsToProcess = points.Where(p => Patterns.Patterns.Blocks.Keys.Contains(p.BlockName))
                    .OrderBy(a => a.Y).ThenBy(a => a.X).ThenBy(a => a.Z).ToList();
            } while (passes < 20 && pointsToProcess.Any());

            points.RemoveAll(p =>
                p.BlockName == "inside" ||
                p.BlockName == "empty"); // this is to prevent processing inside some walled shapes.

            return LinesFromPoints(points, options);
        }
    }
}