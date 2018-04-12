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
        private List<SavedPosition> SavedPositions => _savedPositionHandler.SavedPositions.Positions;
        private readonly SavedPositionHandler _savedPositionHandler;
        public CreateHandler(SavedPositionHandler posHandler)
        {
            ChatCommand = "create";
            ChatCommandDescription =
                "Creates a shape.\ncreate [box|walls|outline|floor|circle|ring|sphere|merlon|triangle|polygon(poly)]";
            _savedPositionHandler = posHandler;
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            CreateGeometry(CommandService, SavedPositions, args.ToArray());
        }

        public static void CreateGeometry(IMinecraftCommandService commandService, List<SavedPosition> savedPositions, params string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var position = commandService.GetLocation();
            
            var commandArgs = args.Skip(1).ToArray();
            var lines = new List<Line>();
            var createCommand = (commandArgs.ElementAtOrDefault(0) ?? "").ToLower();
            switch (createCommand)
            {
                case "circle":
                    lines = CreateCircle(commandService, commandArgs, position, savedPositions);
                    break;
                case "ring":
                    lines = CreateRing(commandService, commandArgs, position, savedPositions);
                    break;
                case "walls":
                    lines = CreateWalls(commandService, commandArgs, position, savedPositions);
                    break;
                case "outline":
                    lines = CreateBox(commandService, commandArgs, position, savedPositions, false);
                    break;
                case "box":
                    lines = CreateBox(commandService, commandArgs, position, savedPositions, true);
                    break;
                case "floor":
                    lines = CreateFloor(commandService, commandArgs, position, savedPositions);
                    break;
                case "sphere":
                    lines = CreateSphere(commandService, commandArgs, position, savedPositions);
                    break;
                case "merlon":
                    lines = CreateMerlon(commandService, commandArgs, position, savedPositions);
                    break;
                case "maze":
                    lines = CreateMaze(commandService, commandArgs, position, savedPositions);
                    break;
                case "house":
                    lines = CreateHouse(commandService, commandArgs, position, savedPositions);
                    break;
                case "triangle":
                    lines = CreateTriangle(commandService, commandArgs, position, savedPositions);
                    break;
                case "poly":
                case "polygon":
                    lines = CreatePoly(commandService, commandArgs, position, savedPositions);
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
                                          "create [poly|polygon]\n" +
                                          "create maze"
                    );
                    return;
            }

            if (!lines.Any()) return;
            LogTime(commandService, sw, $"CREATE {createCommand.ToUpper()}: time to get lines to render: {sw.Elapsed.TotalSeconds}");

            var commandFormater = commandService.GetFormater();
            
            var lastLine = lines.First();
            foreach (var line in lines)
            {
                if (lastLine.Start.Distance2D(line.Start) > 100)
                {
                    commandService.Command($"tp @s {line.Start.X} ~ {line.Start.Z}");
                }

                var command = commandFormater.Fill(line.Start.X, line.Start.Y, line.Start.Z, line.End.X, line.End.Y,
                    line.End.Z, line.Block, line.Block.Contains(" ") ? "" : "0");
                // TODO: Identify the limitation here and account for it.  
                // Should this be executed on a thread so that other commands can be processed at the same time?  
                // Is that possible?
                commandService.Command(command);
                lastLine = line;
            }

            LogTime(commandService, sw, $"CREATE {createCommand.ToUpper()}: time to queue commands: {sw.Elapsed.TotalSeconds}");

            commandService.Wait();
            LogTime(commandService, sw, $"CREATE {createCommand.ToUpper()}: time to complete import: {sw.Elapsed.TotalSeconds}");
        }

        private static void LogTime(IMinecraftCommandService commandService, Stopwatch sw, string message)
        {
            sw.Stop();
            commandService.Status(message);
            sw.Reset();
            sw.Start();
        }

        private static List<Line> CreateMerlon(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ISquareOptions merlon = new Options { Fill = false, Merlon = true };
            commandArgs = ProcessFillArgument(commandArgs, (Options)merlon);
            var location = position;
            switch (commandArgs.Length)
            {
                case 4: // width(X) length(Z) block @ current position
                    merlon.Width = commandArgs[1].ToInt();
                    merlon.Length = commandArgs[2].ToInt();
                    merlon.Height = 1;
                    merlon.Block = commandArgs[3];
                    break;
                case 5: // width(X) length(Z) block savedposition
                case 7: // width(X) length(Z) block x y z
                    merlon.Width = commandArgs[1].ToInt();
                    merlon.Length = commandArgs[2].ToInt();
                    merlon.Height = 1;
                    merlon.Block = commandArgs[3];
                    location = location.GetAbsolutePosition(commandArgs.Skip(4).Take(3), savedPositions);
                    break;
                default:
                    var help = "\nCREATE MERLON\n" +
                               "create merlon [fill] width(X) length(Z) block - center at current position\n" +
                               "create merlon [fill] width(X) length(Z) block [named position]\n" +
                               "create merlon [fill] width(X) length(Z) block [x y z]";
                    commandService.Status(help);
                    return new List<Line>();
            }

            merlon.Start = location.ToPoint();
            IGenerator generator = new MerlonGenerator();
            return generator.Run((Options)merlon);
        }

        private static List<Line> CreateMaze(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            IMazeOptions maze = new Options { Fill = false, Thickness = 1, InnerThickness = 3 };
            var location = position;
            switch (commandArgs.Length)
            {
                // width(X) length(Z) height(Y) block [postition]
                case 7:
                    maze.Width = commandArgs[1].ToInt();
                    maze.Length = commandArgs[2].ToInt();
                    maze.Height = commandArgs[3].ToInt();
                    maze.Thickness = commandArgs[4].ToInt();
                    maze.InnerThickness = commandArgs[5].ToInt();
                    maze.Block = commandArgs[6];
                    break;
                case 8: // width(X) length(Z) height(Y) block savedposition
                case 10: // width(X) length(Z) height(Y) block x y z
                    maze.Width = commandArgs[1].ToInt();
                    maze.Length = commandArgs[2].ToInt();
                    maze.Height = commandArgs[3].ToInt();
                    maze.Thickness = commandArgs[4].ToInt();
                    maze.InnerThickness = commandArgs[5].ToInt();
                    maze.Block = commandArgs[6];
                    location = location.GetAbsolutePosition(commandArgs.Skip(7).Take(3), savedPositions);
                    break;
                default:
                    var help = "\nCREATE MAZE\n" +
                               "create maze width(X) length(Z) height(Y) wall-thickness inner-thickness block - center at current position\n" +
                               "create maze width(X) length(Z) height(Y) wall-thickness inner-thickness block [name]\n" +
                               "create maze width(X) length(Z) height(Y) wall-thickness inner-thickness block [x y z]\n";
                    commandService.Status(help);
                    return new List<Line>();
            }

            maze.Start = location.ToPoint();
            IGenerator generator = new MazeGenerator(commandService);
            return generator.Run((Options)maze);
        }

        private static List<Line> CreateHouse(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            IHouseOptions house = new Options { Fill = false, Thickness = 1, InnerThickness = 3 };
            var location = position;
            if (commandArgs.Length >= 2)
            {
                commandArgs = commandArgs.Skip(1).ToArray();
                bool evaluate = true;
                while (evaluate)
                {
                    switch (commandArgs[0])
                    {
                        case "floors":
                            var floors = commandArgs.ElementAtOrDefault(1);
                            house.Height = floors.ToInt();
                            commandArgs = commandArgs.Skip(2).ToArray();
                            break;
                        default: 
                            house.Block = commandArgs.Length == 0 ? "stone" : String.Join(" ", commandArgs );
                            evaluate = false;
                            break;
                    }
                }
            }

            house.Start = location.ToPoint();
            IGenerator generator = new HouseGenerator(commandService);
            return generator.Run((Options)house);
        }

        private static List<Line> CreateFloor(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ISquareOptions floor = new Options {Fill = true};
            commandArgs = ProcessFillArgument(commandArgs, (Options)floor);
            var location = position;
            switch (commandArgs.Length)
            {
                case 4: // width(X) length(Z) block @ current position
                    floor.Width = commandArgs[1].ToInt();
                    floor.Length = commandArgs[2].ToInt();
                    floor.Height = 1;
                    floor.Block = commandArgs[3];
                    break;
                case 5: // width(X) length(Z) block savedposition
                case 7: // width(X) length(Z) block x y z
                    floor.Width = commandArgs[1].ToInt();
                    floor.Length = commandArgs[2].ToInt();
                    floor.Height = 1;
                    floor.Block = commandArgs[3];
                    location = location.GetAbsolutePosition(commandArgs.Skip(4).Take(3), savedPositions);
                    break;
                default:
                    var help = "\nCREATE FLOOR\n" +
                               "create floor [nofill] width(X) length(Z) block - center at current position\n" +
                               "create floor [nofill] width(X) length(Z) block [named position]\n" +
                               "create floor [nofill] width(X) length(Z) block [x y z]";
                    commandService.Status(help);
                    return new List<Line>();
            }

            floor.Start = location.ToPoint();
            IGenerator generator = new BoxGenerator();
            return generator.Run((Options) floor);
        }

        private static List<Line> CreateBox(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions, bool fill)
        {
            ISquareOptions box = new Options {Fill = fill};
            var command = commandArgs[0].ToLowerInvariant();
            commandArgs = ProcessFillArgument(commandArgs, (Options)box);

            var location = position;
            switch (commandArgs.Length)
            {
                // width(X) length(Z) height(Y) block [postition]
                case 5:
                    box.Width = commandArgs[1].ToInt();
                    box.Length = commandArgs[2].ToInt();
                    box.Height = commandArgs[3].ToInt();
                    box.Block = commandArgs[4];
                    break;
                case 6: // width(X) length(Z) height(Y) block savedposition
                case 8: // width(X) length(Z) height(Y) block x y z
                    box.Width = commandArgs[1].ToInt();
                    box.Length = commandArgs[2].ToInt();
                    box.Height = commandArgs[3].ToInt();
                    box.Block = commandArgs[4];
                    location = location.GetAbsolutePosition(commandArgs.Skip(5).Take(3), savedPositions);
                    break;
                default:
                    var help = $"\nCREATE {command.ToUpper()} - {(fill ? "filled" : "not filled")} by default\n" +
                               $"create {command} [fill|nofill] width(X) length(Z) height(Y) block - center at current position\n" +
                               $"create {command} [fill|nofill] width(X) length(Z) height(Y) block [named position]\n" +
                               $"create {command} [fill|nofill] width(X) length(Z) height(Y) block [x y z]";

                    commandService.Status(help);
                    return new List<Line>();
            }

            box.Start = location.ToPoint();
            IGenerator generator = new BoxGenerator();
            return generator.Run((Options) box);
        }

        private static List<Line> CreateWalls(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ISquareOptions walls = new Options{Fill = false, Thickness = 1};
            var location = position;
            switch (commandArgs.Length)
            {
                case 5: // width(X) length(Z) height(Y) block @ current position
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    break;
                case 6: // width(X) length(Z) height(Y) block savedposition
                case 7: // width(X) length(Z) height(Y) block savedposition thickness
                case 8: // width(X) length(Z) height(Y) block x y z
                case 9: // width(X) length(Z) height(Y) block x y z thickness
                    walls.Width = commandArgs[1].ToInt();
                    walls.Length = commandArgs[2].ToInt();
                    walls.Height = commandArgs[3].ToInt();
                    walls.Block = commandArgs[4];
                    location = location.GetAbsolutePosition(commandArgs.Skip(5).Take(3), savedPositions);
                    walls.Thickness = (commandArgs.ElementAtOrDefault(commandArgs.Length == 7 ? 6 : 8) ?? "1").ToInt();
                    break;
                default:
                    var help = "\nCREATE WALLS\n" +
                               "create walls width(X) length(Z) height(Y) block - defaults to center at current position, thickness 1\n" +
                               "create walls width(X) length(Z) height(Y) block [named position] [thickness]\n" +
                               "create walls width(X) length(Z) height(Y) block [x y z] [thickness]";
                    commandService.Status(help);
                    return new List<Line>();
            }

            walls.Start = location.ToPoint();
            IGenerator generator = new SquareGenerator();
            return generator.Run((Options) walls);
        }

        private static List<Line> CreateCircle(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ICircleOptions circle = new Options {Fill = true};
            var location = position;
            switch (commandArgs.Length)
            {
                // radius height(Y) block [position]
                case 4:
                    circle.Radius = commandArgs[1].ToInt();
                    circle.Height = commandArgs[2].ToInt();
                    circle.Block = commandArgs[3];
                    break;


                case 5: // radius height(Y) block savedposition
                case 7: // radius height(Y) block x y z
                    circle.Radius = commandArgs[1].ToInt();
                    circle.Height = commandArgs[2].ToInt();
                    circle.Block = commandArgs[3];
                    location = location.GetAbsolutePosition(commandArgs.Skip(4).Take(3), savedPositions);
                    break;
                default:
                    var help = "\nCREATE CIRCLE\n" +
                               "create circle radius height(Y) block - center at current position\n" +
                               "create circle radius height(Y) block [named position]\n" +
                               "create circle radius height(Y) block x y z";
                    commandService.Status(help);

                    return new List<Line>();
            }

            circle.Start = location.ToPoint();
            IGenerator generator = new CircleGenerator();
            return generator.Run((Options) circle);
        }

        private static List<Line> CreateSphere(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ISphereOptions sphere = new Options();
            var location = position;
            switch (commandArgs.Length)
            {
                case 3: // radius height(Y) block @ current position
                    sphere.Radius = commandArgs[1].ToInt();
                    sphere.Block = commandArgs[2];
                    break;
                case 4: // radius height(Y) block savedposition
                case 6: // radius height(Y) block x y z
                    sphere.Radius = commandArgs[1].ToInt();
                    sphere.Block = commandArgs[2];
                    location = location.GetAbsolutePosition(commandArgs.Skip(3).Take(3), savedPositions);
                    break;
                default:
                    commandService.Status("\nCREATE SPHERE\n" +
                                          "create sphere radius block - current postion\n" +
                                          "create sphere radius block [named position]\n" +
                                          "create sphere raidus block x y z");
                    return new List<Line>();
            }

            sphere.Start = location.ToPoint();
            IGenerator generator = new SphereGenerator();
            return generator.Run((Options) sphere);
        }

        private static List<Line> CreateRing(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            ICircleOptions ring = new Options {Fill = false};
            var location = position;
            switch (commandArgs.Length)
            {
                case 3: // radius block @ current position & height=1
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Block = commandArgs[2];
                    ring.Height = 1;
                    break;
                case 4: // radius height(Y) block @ current position
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = commandArgs[2].ToInt();
                    ring.Block = commandArgs[3];
                    break;
                case 5: // radius height(Y) block savedposition
                case 7: // radius height(Y) block x y z
                    ring.Radius = commandArgs[1].ToInt();
                    ring.Height = commandArgs[2].ToInt();
                    ring.Block = commandArgs[3];
                    location = location.GetAbsolutePosition(commandArgs.Skip(4).Take(3), savedPositions);
                    break;
                default:
                    commandService.Status("\nCREATE RING\n" +
                                          "create ring radius block\n" +
                                          "create ring radius height(Y) block - current position\n" +
                                          "create ring radius height(Y) block [named position]\n" +
                                          "create ring radius height(Y) block x y z");
                    return new List<Line>();
            }

            ring.Start = location.ToPoint();
            IGenerator generator = new RingGenerator();
            return generator.Run((Options) ring);
        }

        private static List<Line> CreateTriangle(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            IPolygonOptions triangle = new Options { Fill = false };
            var location = position;
            commandArgs = ProcessFillArgument(commandArgs, (Options)triangle);

            switch (commandArgs.Length)
            {
                case 3: // radius block @ current position & height=1
                    triangle.Radius = commandArgs[1].ToInt() / 2;
                    triangle.Block = commandArgs[2];
                    triangle.Height = 1;
                    break;
                case 4: // radius height(Y) block @ current position
                    triangle.Radius = commandArgs[1].ToInt() / 2;
                    triangle.Height = commandArgs[2].ToInt();
                    triangle.Block = commandArgs[3];
                    break;
                case 5: // radius height(Y) block savedposition
                case 7: // radius height(Y) block x y z
                    triangle.Radius = commandArgs[1].ToInt() / 2;
                    triangle.Height = commandArgs[2].ToInt();
                    triangle.Block = commandArgs[3];
                    location = location.GetAbsolutePosition(commandArgs.Skip(4).Take(3), savedPositions);
                    break;
                default:
                    commandService.Status("\nCREATE TRIANGLE\n" +
                                          "create triangle [fill] radius block\n" +
                                          "create triangle [fill] radius height(Y) block - current position\n" +
                                          "create triangle [fill] radius height(Y) block [named position]\n" +
                                          "create triangle [fill] radius height(Y) block x y z");
                    return new List<Line>();
            }

            triangle.Start = location.ToPoint();
            triangle.Sides = 3;
            triangle.Steps = 3;
            triangle.StartingAngle = 0;

            IGenerator generator = new PolygonGenerator();
            return generator.Run((Options) triangle);
        }

        private static List<Line> CreatePoly(IMinecraftCommandService commandService, string[] commandArgs,
            Position position, List<SavedPosition> savedPositions)
        {
            IPolygonOptions poly = new Options { Fill = false };
            var location = position;

            commandArgs = ProcessFillArgument(commandArgs, (Options)poly);

            if (commandArgs.Count() > 3)
            {
                poly.StartingAngle = commandArgs[1].ToInt();
                poly.Sides = commandArgs[2].ToInt();
                poly.Steps = commandArgs[3].ToInt();
            }

            commandArgs = commandArgs.Skip(4).ToArray();
            switch (commandArgs.Length)
            {
                case 2: // radius block @ current position & height=1
                    poly.Block = commandArgs[1];
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = 1;
                    break;
                case 3: // radius height(Y) block @ current position
                    poly.Block = commandArgs[2];
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = commandArgs[1].ToInt();
                    break;
                case 4: // radius height(Y) block savedposition
                case 6: // radius height(Y) block x y z
                    poly.Radius = commandArgs[0].ToInt();
                    poly.Height = commandArgs[1].ToInt();
                    poly.Block = commandArgs[2];
                    location = location.GetAbsolutePosition(commandArgs.Skip(3).Take(3), savedPositions);
                    break;
                default:
                    commandService.Status("\nCREATE POLY\n" +
                                          "create poly [fill] startingAngle sides steps radius block\n" +
                                          "create poly [fill] startingAngle sides steps radius height(Y) block - current position\n" +
                                          "create poly [fill] startingAngle sides steps radius height(Y) block [named position]\n" +
                                          "create poly [fill] startingAngle sides steps radius height(Y) block x y z");
                    return new List<Line>();
            }

            poly.Start = location.ToPoint();
            IGenerator generator = new PolygonGenerator();
            return generator.Run((Options) poly);
        }

        private static string[] ProcessFillArgument(string[] commandArgs, Options options)
        {
            var arg = commandArgs.ElementAtOrDefault(1).ToLowerInvariant();
            switch (arg)
            {
                case "fill":
                case "true":
                    options.Fill = true;
                    return commandArgs.Skip(1).ToArray();
                case "nofill":
                case "false":
                    options.Fill = false;
                    return commandArgs.Skip(1).ToArray();
                default:
                    return commandArgs;
            }
        }
    }
}