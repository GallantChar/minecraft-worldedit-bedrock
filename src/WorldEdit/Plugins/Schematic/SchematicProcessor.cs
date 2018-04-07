using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ShapeGenerator;
using WorldEdit.Input;
using WorldEdit.Output;
// ReSharper disable PossibleMultipleEnumeration

namespace WorldEdit.Schematic
{
    public class SchematicProcessor
    {
        private readonly IMinecraftCommandService _commandService;

        public SchematicProcessor(IMinecraftCommandService commandService)
        {
            _commandService = commandService;
        }

        public void SchematicCommandProcessor(Position position, List<SavedPosition> savedPositions,
            IEnumerable<string> args)
        {
            switch (args.FirstOrDefault() /* command */)
            {
                case "list":
                    CommandListFiles();
                    break;
                case "analyze":
                    CommandAnalyzeModel(args.Skip(1));
                    break;
                case "outline":
                    CommandOutline(args.Skip(1), position, savedPositions);
                    break;
                case "cleararea":
                    CommandClearArea(args.Skip(1), position, savedPositions);
                    break;
                case "import":
                    CommandImport(args.Skip(1), position, savedPositions);
                    break;
                case "test":
                    CommandTest(args.Skip(1), position);
                    break;
                default:
                    CommandHelp();
                    break;
            }
        }

        private void CommandHelp()
        {
            _commandService.Status("schematic command\n" +
                                  "schematic list\n" +
                                  "schematic analyze [name]\n" +
                                  "schematic import name x y z (rotation) (Shift X) (Shift Y) (Shift Z)");
        }

        private void CommandTest(IEnumerable<string> args, Position position)
        {
            var target = position.GetAbsolutePosition(args.Take(3));
            var shift = GetShiftPosition(args.Skip(3).Take(3));

            // analyze then import all schematics in the folder.
            var filesToProcess = GetSchematics().Select(a => new {Points = LoadFile(a), Filename = a})
                .ToList()
                .Select(a => new {a.Filename, a.Points, Analysis = ModelAnalyzer.Analyze(a.Points)})
                .OrderBy(a => a.Analysis.TotalPlaceableBlocks)
                .ToList();
            foreach (var f in filesToProcess)
            {
                _commandService.Command(
                    $"tp @s {target.X + f.Analysis.Width / 2} {target.Y + f.Analysis.Height} {target.Z - 5}");
                _commandService.Status($"importing {Path.GetFileName(f.Filename)}");
                SendCommandsToCodeConnection(target, f.Points, Rotate.None, shift);

                target.X += f.Analysis.Width + 15;
            }
        }

        private static string[] GetSchematics()
        {
            return Directory.GetFiles(GetSchematicFolder(), "*.schematic");
        }

        private static string GetSchematicFolder()
        {
            var schematicFolder = Path.GetFullPath(ConfigurationManager.AppSettings["schematics"]);
            if (!Directory.Exists(schematicFolder)) Directory.CreateDirectory(schematicFolder);
            return schematicFolder;
        }

        private void CommandAnalyzeModel(IEnumerable<string> args)
        {
            var points = LoadFile(args.FirstOrDefault());
            var results = ModelAnalyzer.Analyze(points);
            var firstGroundLayer =
                results.Layers.First(a => a.Blocks.Any(b => b.Block.Equals("air") && b.PercentOfLayer >= 0.5))
                    .Y;
            var output =
                $"{Path.GetFileName(args.FirstOrDefault())} Model Size: X:{results.Width} Y:{results.Height} Z:{results.Length} Ground Level:{firstGroundLayer} Total Blocks:{results.Width * results.Height * results.Length}";

            _commandService.Status(output);
        }

        private void CommandListFiles()
        {
            _commandService
                .Status("Schematics: \n" +
                        string.Join("\n", GetSchematics().Select(Path.GetFileNameWithoutExtension).OrderBy(a => a)));
        }

        private void CommandOutline(IEnumerable<string> args, Position position, List<SavedPosition> savedPositions)
        {
            var filename = args.FirstOrDefault();
            var target = position.GetAbsolutePosition(args.Skip(1).Take(3), savedPositions);
            //var rotation = GetRotation(args.ElementAtOrDefault(4));
            //var shift = GetPosition(args.Skip(4).Take(3));

            Console.WriteLine($"outlineing {filename} to {target}");
            var points = LoadFile(filename);
            var results = ModelAnalyzer.Analyze(points);
            var x = (target.X + results.Width / 2).ToString();
            var z = (target.Z + results.Length / 2).ToString();
            CreateHandler.CreateGeometry(_commandService, savedPositions, "create", "box", results.Width.ToString(),
                results.Length.ToString(), results.Height.ToString(), "wool", x, target.Y.ToString(), z);
        }

        private void CommandClearArea(IEnumerable<string> args, Position position, List<SavedPosition> savedPositions)
        {
            var filename = args.FirstOrDefault();
            var target = position.GetAbsolutePosition(args.Skip(1).Take(3), savedPositions);
            //var rotation = GetRotation(args.ElementAtOrDefault(4));
            //var shift = GetPosition(args.Skip(4).Take(3));

            Console.WriteLine($"outlineing {filename} to {target}");
            var points = LoadFile(filename);
            var results = ModelAnalyzer.Analyze(points);
            var x = (target.X + results.Width / 2).ToString();
            var z = (target.Z + results.Length / 2).ToString();
            CreateHandler.CreateGeometry(_commandService, savedPositions, "create", "box", results.Width.ToString(),
                results.Length.ToString(), results.Height.ToString(), "air", x, target.Y.ToString(), z);
        }

        private void CommandImport(IEnumerable<string> args, Position position, List<SavedPosition> savedPositions)
        {
            var filename = args.FirstOrDefault();
            var target = position.GetAbsolutePosition(args.Skip(1).Take(3), savedPositions);
            var rotation = GetRotation(args.ElementAtOrDefault(4));
            var shift = GetShiftPosition(args.Skip(5).Take(3));

            Console.WriteLine($"importing {filename} to {target}");
            var points1 = LoadFile(filename);
            SendCommandsToCodeConnection(target, points1, rotation, shift);
        }

        private List<Point> LoadFile(string filename)
        {
            var schematicFolder = GetSchematicFolder();

            filename = new Regex($"[{Path.GetInvalidFileNameChars()}]").Replace(filename ?? "", "");

            if (!filename.EndsWith(".schematic")) filename += ".schematic";

            // Check for file in folder
            var files = Directory.GetFiles(schematicFolder, filename);
            if (files.Length == 0)
            {
                _commandService.Status("Unable to locate the schematic.");
                return null;
            }

            var schematic = Schematic.LoadFromFile(Path.GetFullPath(files[0]));
            return schematic.GetPoints();
        }

        private void SendCommandsToCodeConnection(Position target, List<Point> points, Rotate rotation,
            Position clip = null)
        {
            // var service = new MinecraftCodeConnectionCommandService();
            var sw = new Stopwatch();

            _commandService.Status("preparing schematic");

            if (clip != null)
                points =
                    points.Where(a => a.X >= clip.X && a.Y >= clip.Y && a.Z >= clip.Z)
                        .Select(a => a.Shift(clip.Muliply(-1)))
                        .ToList();
            if (rotation != Rotate.None)
            {
                sw.Start();
                Console.WriteLine("rotating points...");
                var rotatedPoints = points.AsParallel().Select(a => a.Rotate(rotation)).ToList();
                Console.WriteLine($"time to rotate {sw.Elapsed}");
                sw.Reset();
                var measures = ModelAnalyzer.Analyze(rotatedPoints);
                sw.Start();
                Console.WriteLine("shifting points...");
                points = rotatedPoints.AsParallel().Select(a => a.Shift(measures.Minimum.Muliply(-1))).ToList();
                Console.WriteLine($"time to shift {sw.Elapsed}");
                sw.Reset();
            }

            sw.Start();
            Console.WriteLine($"combining points...");
            var exportLines = ConvertFileToCommands(points.Where(a => a.BlockId != 0).ToList());
            Console.WriteLine($"time to combine {sw.Elapsed}");
            sw.Reset();
            sw.Start();
            var shift = exportLines.AsParallel().Select(a => a.Shift(target)).ToList();
            Console.WriteLine($"time to shift {sw.Elapsed}");
            sw.Reset();
            sw.Start();
            var importLines =
                shift.AsParallel()
                    .OrderBy(a => a.Start.SortOrder)
                    .ThenBy(a => a.Start.Y)
                    .ThenBy(a => a.Start.X)
                    .ThenBy(a => a.Start.Z)
                    .ToList();
            Console.WriteLine($"time to sort {sw.Elapsed}");

            sw.Reset();
            _commandService.Status("starting schematic import");

            sw.Start();
            foreach (var line in importLines)
            {
                var command = _commandService.GetFormater()
                    .Fill(line.Start.X, line.Start.Y, line.Start.Z, line.End.X, line.End.Y, line.End.Z, line.BlockName,
                        line.Data.ToString());
                _commandService.Command(command);
                //$"fill?from={line.Start.X} {line.Start.Y} {line.Start.Z}&to={line.End.X} {line.End.Y} {line.End.Z}&tileName={line.BlockName}&tileData={line.Data}");
            }

            sw.Stop();
            _commandService.Status($"time to queue commands {sw.Elapsed.TotalSeconds}");
            Console.WriteLine($"time to queue commands {sw.Elapsed.TotalSeconds}");
            sw.Reset();
            sw.Start();
            _commandService.Wait();
            sw.Stop();
            _commandService.Status($"time to complete import {sw.Elapsed.TotalSeconds}");
            Console.WriteLine($"time to complete import {sw.Elapsed.TotalSeconds}");
        }

        private static List<Line> ConvertFileToCommands(List<Point> points)
        {
            return LineFactory.CreateFromPoints(points);
        }

        private Position GetShiftPosition(IEnumerable<string> coordinates)
        {
            return new Position
            (Convert.ToInt32(coordinates.ElementAtOrDefault(0)),
                Convert.ToInt32(coordinates.ElementAtOrDefault(1)),
                Convert.ToInt32(coordinates.ElementAtOrDefault(2)));
        }

        private Rotate GetRotation(string rotation)
        {
            switch (rotation)
            {
                case "right":
                case "1":
                case "90":
                case "ninety": return Rotate.Ninety;
                case "around":
                case "reverse":
                case "2":
                case "180": return Rotate.OneEighty;
                case "270":
                case "left":
                case "3": return Rotate.TwoSeventy;
                default:
                    return Rotate.None;
            }
        }
    }
}