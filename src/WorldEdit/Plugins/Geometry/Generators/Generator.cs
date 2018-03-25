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
                var item1 = new Line { Start = point.Clone(), End = point.Clone(), Block = GetBlockName(options) };
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

        private static Random random = new Random();

        private static string GetBlockName(Options options)
        {

            if (options.Block.Equals("castle", StringComparison.InvariantCultureIgnoreCase))
            {
                var blocks = new List<Tuple<string, int>>();
                blocks.Add(new Tuple<string, int>("stonebrick 0", 78));
                blocks.Add(new Tuple<string, int>("stone", 5));
                blocks.Add(new Tuple<string, int>("stonebrick 2", 15));
                blocks.Add(new Tuple<string, int>("stonebrick 1", 2));

                var total = blocks.Sum(a => a.Item2);
                var normalized = blocks.Select(a => new { Item = a.Item1, Percentage = a.Item2 / total, Frequency = a.Item2 }).OrderByDescending(a => a.Frequency).ToList();

                var number = random.Next(0, total);
                foreach (var item in normalized)
                {
                    number -= item.Frequency;
                    if (number <= 0)
                    {
                        return item.Item;
                    }
                }
            }
            return options.Block;
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
                    if (lines[i].CanCombine(lines[j]))
                    {
                        lines[i] = lines[i].Combine(lines[j]);
                        lines.Remove(lines[j]);
                        i--;
                        break;
                    }
                }
            }
            return lines;
        }

        public List<Line> TransformToLines(List<Point> points, Options options)
        {
            return LinesFromPoints(points, options);
        }
    }
}
