using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeGenerator;

namespace ShapeGenerator.Generators
{
    public class Generator
    {
        public static List<Line> LinesFromPoints(List<Point> points, Options options)
        {
            var lines = new List<Line>();
            foreach (var point in points.ToList())
            {
                var item1 = new Line { Start = point.Clone(), End = point.Clone(), Block = point.BlockName };
                lines.Add(item1);
            }
            lines = lines.OrderBy(a => a.Start.X).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.Y).ToList();
            lines = SquashLines(lines);
            lines = lines.OrderBy(a => a.Start.Y).ThenBy(a => a.Start.X).ThenBy(a => a.Start.Z).ToList();
            lines = SquashLines(lines);

            lines = lines.OrderBy(a => a.Start.Z).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.X).ToList();
            lines = SquashLines(lines);

            lines = lines.OrderBy(a => a.Start.Z).ThenBy(a => a.Start.X).ThenBy(a => a.Start.Y).ToList();
            lines = SquashLines(lines);

            lines = lines.OrderBy(a => a.Start.Y).ThenBy(a => a.Start.Z).ThenBy(a => a.Start.X).ToList();
            lines = SquashLines(lines);

            lines = lines.OrderBy(a => a.Start.X).ThenBy(a => a.Start.Y).ThenBy(a => a.Start.Z).ToList();
            lines = SquashLines(lines);

            lines = SplitLinesIntoMaxSizes(lines);
            return lines;
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

        public static List<Line> SquashLines(List<Line> lines)
        {
            for (var i = 0; i < lines.Count - 1; i++)
            {
                for (var j = i + 1; j < lines.Count; j++)
                {
                    if (!lines[i].CanCombine(lines[j])) continue;

                    lines[i] = lines[i].Combine(lines[j]);
                    lines.Remove(lines[j]);
                    i--;
                    break;
                }
            }
            return lines;
        }

        public List<Line> TransformToLines(List<Point> points, Options options)
        {
            // set patterns
            points.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.BlockName)) return;
                p.BlockName = options.Block;
            });

            foreach (var transformer in Patterns.Patterns.Transformers)
                transformer.Value.transform(points);

            // limited to 20 passes. Allowing patterns to utilize patterns in their logic.
            int passes = 0;
            do
            {
                var processList = points.Where(p => Patterns.Patterns.Blocks.Keys.Contains(p.BlockName))
                                        .OrderBy(a => a.Y).ThenBy(a => a.X).ThenBy(a => a.Z).ToList();
                processList.ForEach(p => { p.BlockName = Patterns.Patterns.GetBlock(points, p); });
                passes++;
            } while (passes<20 && points.Any(p => Patterns.Patterns.Blocks.Keys.Contains(p.BlockName)));

            points.RemoveAll(p => p.BlockName == "inside" || p.BlockName == "empty"); // this is to prevent processing inside some walled shapes.

            return LinesFromPoints(points, options);
        }
    }
}
