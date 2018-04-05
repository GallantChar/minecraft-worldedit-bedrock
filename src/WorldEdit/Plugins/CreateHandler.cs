using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ShapeGenerator;
using ShapeGenerator.Generators;
using WorldEdit.Commands;
using WorldEdit.Input;
using WorldEdit.Output;
using WorldEdit.Schematic;
using Line = ShapeGenerator.Line;

namespace WorldEdit
{
    public class CreateHandler : ChatHandler
    {
        public CreateHandler()
        {
            ChatCommand = "create";
            ChatCommandDescription = "Creates a shape. [box|walls|outline|floor|circle|ring|sphere|merlon|triangle|polygon]";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            CreateGeometry(CommandService, args.ToArray());
        }

        public static void CreateGeometry(IMinecraftCommandService commandService, params string[] args)
        {
            var position = commandService.GetLocation();
            var savedPositions = new List<SavedPosition>();
            var commandArgs = args.Skip(1).ToArray();
            var lines = new List<Line>();
            switch ((commandArgs.ElementAtOrDefault(0)??"").ToLower())
            {
                case "circle":
                    lines = CreateCircle(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "ring":
                    lines = CreateRing(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "walls":
                    lines = CreateWalls(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "outline":
                    lines = CreateOutline(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "box":
                    lines = CreateBox(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "floor":
                    lines = CreateFloor(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "sphere":
                    lines = CreateSphere(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "merlon":
                    lines = CreateMerlon(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "triangle":
                    lines = CreateTriangle(commandService, commandArgs, position, savedPositions, lines);
                    break;
                case "poly":
                case "polygon":
                    lines = CreatePoly(commandService, commandArgs, position, savedPositions, lines);
                    break;
                default:
                    commandService.Status("CREATE\n" +
                                          "create circle\n" +
                                          "create ring\n" +
                                          "create walls\n" +
                                          "create outline\n" +
                                          "create box\n" +
                                          "create floor\n" +
                                          "create sphere\n" +
                                          "create merlon\n" +
                                          "create triangle\n" +
                                          "create [poly|polygon]"
                    );
                    return;
            }

            if (!lines.Any()) return;

            var commandFormater = commandService.GetFormater();
            var sw = new Stopwatch();
            sw.Start();
            var lastLine = lines.First();
            foreach (var line in lines)
            {
                if (lastLine.Start.Distance2D(line.Start) > 100)
                {                        
                    commandService.Command($"tp @s {line.Start.X} ~ {line.Start.Z}");
                }
                var command = commandFormater.Fill(line.Start.X, line.Start.Y, line.Start.Z, line.End.X,line.End.Y,line.End.Z, line.Block, line.Block.Contains(" ")?"":"0");
                commandService.Command(command);
                lastLine = line;
            }
            sw.Stop();
            commandService.Status($"time to queue commands {sw.Elapsed.TotalSeconds}");
            //Console.WriteLine($"time to queue commands {sw.Elapsed.TotalSeconds}");
            sw.Reset();
            sw.Start();
            commandService.Wait();
            sw.Stop();
            commandService.Status($"time to complete import {sw.Elapsed.TotalSeconds}");
            //Console.WriteLine($"time to complete import {sw.Elapsed.TotalSeconds}");
        }

        private static List<Line> CreateMerlon(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions, List<Line> lines)
        {
            ISquareOptions walls = new Options();
            walls.Fill = false;
            walls.Merlon = true;
            switch (commandArgs.Length)
            {
                // width height block [postition]
                case 4:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    walls.X = position.X;
                    walls.Y = position.Y;
                    walls.Z = position.Z;
                    break;
                case 5:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    walls.X = center.X;
                    walls.Y = center.Y;
                    walls.Z = center.Z;
                    break;
                case 7:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    walls.X = commandArgs[4].ToInt();
                    walls.Y = commandArgs[5].ToInt();
                    walls.Z = commandArgs[6].ToInt();
                    break;
                default:
                    var help = "\nCREATE MERLON\n" +
                               "create merlon length width block - center at current position\n" +
                               "create merlon length width block [named position]\n" +
                               "create merlon length width block x y z";
                    commandService.Status(help);
                    return new List<Line>();
            }
            IGenerator generator = new MerlonGenerator();
            lines = generator.Run((Options)walls);
            return lines;

        }

        private static List<Line> CreateFloor(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ISquareOptions walls = new Options();
            walls.Fill = true;
            switch (commandArgs.Length)
            {
                // width height block [postition]
                case 4:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    walls.X = position.X;
                    walls.Y = position.Y;
                    walls.Z = position.Z;
                    break;
                case 5:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    walls.X = center.X;
                    walls.Y = center.Y;
                    walls.Z = center.Z;
                    break;
                case 7:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = 1;
                    walls.Block = commandArgs[3];
                    walls.X = commandArgs[4].ToInt();
                    walls.Y = commandArgs[5].ToInt();
                    walls.Z = commandArgs[6].ToInt();
                    break;
                default:
                    var help = "\nCREATE FLOOR\n" +
                               "create floor length width block - center at current position\n" +
                               "create floor length width block [named position]\n" +
                               "create floor length width block x y z";
                    commandService.Status(help);
                    return new List<Line>();
            }
            IGenerator generator = new BoxGenerator();
            lines = generator.Run((Options)walls);
            return lines;
        }

        private static List<Line> CreateBox(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ISquareOptions walls = new Options();
            walls.Fill = true;
            switch (commandArgs.Length)
            {
                // width height block [postition]
                case 5:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    walls.X = position.X;
                    walls.Y = position.Y;
                    walls.Z = position.Z;
                    break;
                case 6:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[5])).Position;
                    walls.X = center.X;
                    walls.Y = center.Y;
                    walls.Z = center.Z;
                    break;
                case 8:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    walls.X = commandArgs[5].ToInt();
                    walls.Y = commandArgs[6].ToInt();
                    walls.Z = commandArgs[7].ToInt();
                    break;
                default:
                    var help = "\nCREATE BOX\n" +
                               "create box length width height block - center at current position\n" +
                               "create box length width height block [named position]\n" +
                               "create box length width height block x y z";
                    commandService.Status(help);
                    return new List<Line>();
            }
            IGenerator generator = new BoxGenerator();
            lines = generator.Run((Options)walls);
            return lines;
        }

        private static List<Line> CreateOutline(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ISquareOptions walls = new Options();
            walls.Fill = false;
            switch (commandArgs.Length)
            {
                // width height block [postition]
                case 5:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    walls.X = position.X;
                    walls.Y = position.Y;
                    walls.Z = position.Z;
                    break;
                case 6:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[5])).Position;
                    walls.X = center.X;
                    walls.Y = center.Y;
                    walls.Z = center.Z;
                    break;
                case 8:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    walls.X = commandArgs[5].ToInt();
                    walls.Y = commandArgs[6].ToInt();
                    walls.Z = commandArgs[7].ToInt();
                    break;
                default:
                    var help = "\nCREATE OUTLINE\n" +
                               "create outline length width height block - center at current position\n" +
                               "create outline length width height block [named position]\n" +
                               "create outline length width height block x y z";
                    commandService.Status(help);
                    return new List<Line>();
            }
            IGenerator generator = new BoxGenerator();
            lines = generator.Run((Options)walls);
            return lines;
        }

        private static int GetAbsolutePosition(int absolute, string relative)
        {
            // nothing passed as value
            if (string.IsNullOrEmpty(relative) || relative.Equals("~")) return absolute;
            // value is fixed location
            if (!relative.StartsWith("~")) return Convert.ToInt32(relative);
            // value is relative position
            return absolute + Convert.ToInt32(relative.Substring(1));
        }

        private static Position GetAbsolutePosition(Position absolutePosition, IEnumerable<string> relativePosition)
        {
            return relativePosition == null ? absolutePosition :
                new Position(GetAbsolutePosition(absolutePosition.X, relativePosition.ElementAtOrDefault(0)),
                    GetAbsolutePosition(absolutePosition.Y, relativePosition.ElementAtOrDefault(1)),
                    GetAbsolutePosition(absolutePosition.Z, relativePosition.ElementAtOrDefault(2)));
        }

        private static List<Line> CreateWalls(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ISquareOptions walls = new Options();
            walls.Fill = false;

            var location = position;
            switch (commandArgs.Length)
            {
                // width height block [postition]
                case 5:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    break;
                case 6:

                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    location = savedPositions.Single(a => a.Name.Equals(commandArgs[5])).Position;
                    break;
                case 8:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    location = GetAbsolutePosition(position, commandArgs.Skip(5).Take(3));
                    walls.Thickness = 1;
                    break;
                case 9:
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    location = GetAbsolutePosition(position, commandArgs.Skip(5).Take(3));
                    walls.Thickness = commandArgs[8].ToInt();
                    break;
                default:
                    var help = "\nCREATE WALLS\n" +
                               "create walls  length width height block - center at current position\n" +
                               "create walls length width height block [named position]\n" +
                               "create walls length width height block x y z";
                    commandService.Status(help);
                    return new List<Line>();
            }

            walls.X = location.X;
            walls.Y = location.Y;
            walls.Z = location.Z;
            IGenerator generator = new SquareGenerator();
            lines = generator.Run((Options)walls);
            return lines;
        }

        private static List<Line> CreateCircle(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ICircleOptions circle = new Options();
            circle.Fill = true;
            switch (commandArgs.Length)
            {
                // radius height block [position]
                case 4:
                    circle.Radius = commandArgs[1].ToInt();
                    circle.Height = commandArgs[2].ToInt();
                    circle.Block = commandArgs[3];

                    circle.X = position.X;
                    circle.Y = position.Y;
                    circle.Z = position.Z;
                    break;

                // radius height block position
                case 5:
                    circle.Radius = commandArgs[1].ToInt();
                    circle.Height = commandArgs[2].ToInt();
                    circle.Block = commandArgs[3];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    circle.X = center.X;
                    circle.Y = center.Y;
                    circle.Z = center.Z;
                    break;
                // radius height block x y z
                case 7:
                    circle.Radius = commandArgs[1].ToInt();
                    circle.Height = commandArgs[2].ToInt();
                    circle.Block = commandArgs[3];
                    circle.X = commandArgs[4].ToInt();
                    circle.Y = commandArgs[5].ToInt();
                    circle.Z = commandArgs[6].ToInt();
                    break;
                default:
                    var help = "\nCREATE CIRCLE\n" +
                               "create circle radius height block - center at current position\n" +
                               "create circle radius height block [named position]\n" +
                               "create circle radius height block x y z";
                    commandService.Status(help);

                    return new List<Line>();
            }
            IGenerator generator = new CircleGenerator();
            lines = generator.Run((Options)circle);
            return lines;
        }

        private static List<Line> CreateSphere(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ISphereOptions sphere = new Options();

            switch (commandArgs.Length)
            {
                // radius height block [position]
                case 3:
                    sphere.Radius = commandArgs[1].ToInt();
                    //sphere.Height = commandArgs[2].ToInt();
                    sphere.Block = commandArgs[2];

                    sphere.X = position.X;
                    sphere.Y = position.Y;
                    sphere.Z = position.Z;
                    break;

                // radius height block position
                case 4:
                    sphere.Radius = commandArgs[1].ToInt();
                    //sphere.Height = commandArgs[2].ToInt();
                    sphere.Block = commandArgs[2];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[3])).Position;
                    sphere.X = center.X;
                    sphere.Y = center.Y;
                    sphere.Z = center.Z;
                    break;
                // radius height block x y z
                case 6:
                    sphere.Radius = commandArgs[1].ToInt();
                    //sphere.Height = commandArgs[2].ToInt();
                    sphere.Block = commandArgs[2];
                    sphere.X = commandArgs[3].ToInt();
                    sphere.Y = commandArgs[4].ToInt();
                    sphere.Z = commandArgs[5].ToInt();
                    break;
                default:
                    commandService.Status("\nCREATE SPHERE\n" +
                                      "create sphere radius block - current postion\n" +
                                      "create sphere radius block [named position]\n" +
                                      "create sphere raidus block x y z");
                    return new List<Line>();
            }
            IGenerator generator = new SphereGenerator();
            lines = generator.Run((Options)sphere);
            return lines;
        }

        private static List<Line> CreateRing(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            ICircleOptions ring = new Options();
            ring.Fill = false;
            switch (commandArgs.Length)
            {
                case 3:
                    //radius height block
                    ring.Block = commandArgs[2];
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = 1;
                    ring.X = position.X;
                    ring.Y = position.Y;
                    ring.Z = position.Z;
                    break;
                // radius height block [position]
                case 4:
                    ring.Block = commandArgs[3];
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = commandArgs[2].ToInt();
                    ring.X = position.X;
                    ring.Y = position.Y;
                    ring.Z = position.Z;
                    break;

                // radius height block position
                case 5:
                    ring.Block = commandArgs[3];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = commandArgs[2].ToInt();
                    ring.X = center.X;

                    ring.Y = center.Y;
                    ring.Z = center.Z;
                    break;
                // radius height block x y z
                case 7:
                    ring.Block = commandArgs[3];
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = commandArgs[2].ToInt();
                    ring.X = commandArgs[4].ToInt();
                    ring.Y = commandArgs[5].ToInt();
                    ring.Z = commandArgs[6].ToInt();
                    break;
                default:
                    commandService.Status("\nCREATE RING\n" +
                                          "create ring radius block\n" +
                                          "create ring radius height block - current position\n" +
                                          "create ring radius height block [named position]\n" +
                                          "create ring radius height block x y z");
                    return new List<Line>();
            }
            IGenerator generator = new RingGenerator();
            lines = generator.Run((Options)ring);
            return lines;
        }

        private static List<Line> CreateTriangle(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            IPolygonOptions triangle = new Options();
            triangle.Fill = false;
            if (commandArgs[1].Equals("fill", System.StringComparison.InvariantCultureIgnoreCase))
            {
                commandArgs = commandArgs.Skip(1).ToArray();
                triangle.Fill = true;
            }
            switch (commandArgs.Length)
            {
                case 3:
                    //radius block
                    triangle.Block = commandArgs[2];
                    triangle.Radius = commandArgs[1].ToInt() / 2;
                    triangle.Height = 1;
                    triangle.X = position.X;
                    triangle.Y = position.Y;
                    triangle.Z = position.Z;
                    break;
                // radius height block [position]
                case 4:
                    triangle.Block = commandArgs[3];
                    triangle.Radius = commandArgs[1].ToInt()/2;
                    triangle.Height = commandArgs[2].ToInt();
                    triangle.X = position.X;
                    triangle.Y = position.Y;
                    triangle.Z = position.Z;
                    break;

                // radius height block position
                case 5:
                    triangle.Block = commandArgs[3];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    triangle.Radius = commandArgs[1].ToInt()/2;
                    triangle.Height = commandArgs[2].ToInt();
                    triangle.X = center.X; 

                    triangle.Y = center.Y;
                    triangle.Z = center.Z;
                    break;
                // radius height block x y z
                case 7:
                    triangle.Block = commandArgs[3];
                    triangle.Radius = commandArgs[1].ToInt()/2;
                    triangle.Height = commandArgs[2].ToInt();
                    triangle.X = commandArgs[4].ToInt();
                    triangle.Y = commandArgs[5].ToInt();
                    triangle.Z = commandArgs[6].ToInt();
                    break;
                default:
                    commandService.Status("\nCREATE TRIANGLE\n" +
                                          "create triangle [fill] radius block\n" +
                                          "create triangle [fill] radius height block - current position\n" +
                                          "create triangle [fill] radius height block [named position]\n" +
                                          "create triangle [fill] radius height block x y z");
                    return new List<Line>();
            }

            triangle.Sides = 3;
            triangle.Steps = 3;
            triangle.StartingAngle = 0;

            IGenerator generator = new PolygonGenerator();
            lines = generator.Run((Options)triangle);
            return lines;
        }

        private static List<Line> CreatePoly(IMinecraftCommandService commandService, string[] commandArgs, Position position, List<SavedPosition> savedPositions,
            List<Line> lines)
        {
            IPolygonOptions poly = new Options();
            poly.Fill = false;

            if (commandArgs[1].Equals("fill", System.StringComparison.InvariantCultureIgnoreCase))
            {
                commandArgs = commandArgs.Skip(1).ToArray();
                poly.Fill = true;
            }

            poly.StartingAngle = commandArgs[1].ToInt();
            poly.Sides = commandArgs[2].ToInt();
            poly.Steps = commandArgs[3].ToInt();
            commandArgs = commandArgs.Skip(4).ToArray();
            
            switch (commandArgs.Length)
            {
                case 2:
                    //radius block
                    poly.Block = commandArgs[1];
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = 1;
                    poly.X = position.X;
                    poly.Y = position.Y;
                    poly.Z = position.Z;
                    break;
                // radius height block [position]
                case 3:
                    poly.Block = commandArgs[2];
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = commandArgs[1].ToInt();
                    poly.X = position.X;
                    poly.Y = position.Y;
                    poly.Z = position.Z;
                    break;

                // radius height block position
                case 4:
                    poly.Block = commandArgs[2];
                    var center = savedPositions.Single(a => a.Name.Equals(commandArgs[4])).Position;
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = commandArgs[1].ToInt();
                    poly.X = center.X;

                    poly.Y = center.Y;
                    poly.Z = center.Z;
                    break;
                // radius height block x y z
                case 6:
                    poly.Block = commandArgs[2];
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = commandArgs[1].ToInt();
                    poly.X = commandArgs[3].ToInt();
                    poly.Y = commandArgs[4].ToInt();
                    poly.Z = commandArgs[5].ToInt();
                    break;
                default:
                    commandService.Status("\nCREATE POLY\n" +
                                          "create poly [fill] startingAngle sides steps radius block\n" +
                                          "create poly [fill] startingAngle sides steps radius height block - current position\n" +
                                          "create poly [fill] startingAngle sides steps radius height block [named position]\n" +
                                          "create poly [fill] startingAngle sides steps radius height block x y z");
                    return new List<Line>();
            }

            
            

            IGenerator generator = new PolygonGenerator();
            lines = generator.Run((Options)poly);
            return lines;
        }
    }
}