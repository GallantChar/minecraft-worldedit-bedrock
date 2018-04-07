/***
 * Using a method similar to Henry Kroll III's method: https://thenerdshow.com/maze.html
 * Also looked at: https://github.com/theJollySin/mazelib in python
 *
 * Used both to derive the below maze algorithm. Tried to implement the hunt method
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WorldEdit.Output;

namespace ShapeGenerator.Generators
{
    public class MazeGenerator : Generator, IGenerator
    {
        private readonly IMinecraftCommandService _commandService;
        public MazeGenerator()
        {

        }

        public MazeGenerator(IMinecraftCommandService commandService)
        {
            _commandService = commandService;
        }

        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        private static readonly Random Random = new Random();

        public List<Point> Run(IMazeOptions options)
        {
            _commandService?.Status("CREATE MAZE: Creating Maze...");

            var sw = new Stopwatch();
            sw.Start();
            // create solid grid (all true)
            var mazeWidth = options.Width * 2 + 1;
            var mazeHeight = options.Length * 2 + 1;
            var maze = new bool[mazeWidth, mazeHeight];

            for (var x = 0; x < mazeWidth; x++)
            {
                for (var z = 0; z < mazeHeight; z++)
                {
                    maze[x, z] = true;
                }
            }

            // hole at top
            maze[0, options.Length + 1] = false;

            // hole at bottom
            maze[options.Width * 2, options.Length + 1] = false;

            // get random z starting position
            var startZ = Random.Next(options.Length) * 2 + 1;
            maze[1, startZ] = false;

            // walk the maze
            WalkMaze(maze, 1, startZ, options.Width, options.Length);
            LogTime(sw, $"CREATE MAZE: time to build maze model: {sw.Elapsed.TotalSeconds}");

            // convert maze to points array and return it.
            var points = RenderPoints(maze, options, mazeWidth, mazeHeight);
            LogTime(sw, $"CREATE MAZE: time to convert maze to render points: {sw.Elapsed.TotalSeconds}");
            sw.Stop();
            
            return points;
        }

        private void LogTime(Stopwatch stopwatch, string message)
        {
            stopwatch.Stop();
            _commandService?.Status(message);
            stopwatch.Reset();
            stopwatch.Start();
        }

        private static List<Point> RenderPoints(bool[,] maze, IMazeOptions options, int mazeWidth, int mazeLength)
        {
            var outer = options.Thickness;
            var inner = options.InnerThickness;

            var points = new List<Point>();

            for (var z = 1; z < mazeLength - 1; z += 2)
            {
                for (var x = 1; x < mazeWidth - 1; x += 2)
                {
                    var cellX = (x - 1) / 2;
                    var cellZ = (z - 1) / 2;
                    var topleft = new Point
                    {
                        X = options.X + cellX * (outer + inner),
                        Y = options.Y,
                        Z = options.Z + cellZ * (outer + inner),
                        BlockName = options.Block
                    };

                    if (maze[x, z - 1]) // top
                    {
                        for (var x2 = 0; x2 < outer + outer + inner; x2++)
                        {
                            for (var z2 = 0; z2 < outer; z2++)
                            {
                                var cell = topleft.Clone();
                                cell.X += x2;
                                cell.Z += z2;
                                points.Add(cell);
                            }
                        }
                    }
                    if (maze[x, z + 1]) // bottom
                    {
                        for (var x2 = 0; x2 < outer + outer + inner; x2++)
                        {
                            for (var z2 = outer + inner; z2 < outer + outer + inner; z2++)
                            {
                                var cell = topleft.Clone();
                                cell.X += x2;
                                cell.Z += z2;
                                points.Add(cell);
                            }
                        }
                    }
                    if (maze[x - 1, z]) // left
                    {
                        for (var z2 = 0; z2 < outer + outer + inner; z2++)
                        {
                            for (var x2 = 0; x2 < outer; x2++)
                            {
                                var cell = topleft.Clone();
                                cell.X += x2;
                                cell.Z += z2;
                                points.Add(cell);
                            }
                        }
                    }
                    if (maze[x + 1, z]) // right
                    {
                        for (var z2 = 0; z2 < outer + outer + inner; z2++)
                        {
                            for (var x2 = outer + inner; x2 < outer + outer + inner; x2++)
                            {
                                var cell = topleft.Clone();
                                cell.X += x2;
                                cell.Z += z2;
                                points.Add(cell);
                            }
                        }
                    }
                }
            }

            // raise the maze
            if (options.Height == 1) return points.Distinct().ToList();

            var ypoints = new List<Point>();
            for (var y = 0; y < options.Height; y++)
                foreach (var p in points.Distinct())
                {
                    var yp = p.Clone();
                    yp.Y = options.Y + y;
                    ypoints.Add(yp);
                }

            return ypoints;
        }

        private static void WalkMaze(bool[,] maze, int x, int z, int mazeWidth, int mazeLength)
        {
            while (true)
            {
                int i = 0;
                var loc = new int[4, 2];
                if (x > 1 && maze[x - 2, z])
                {
                    loc[i, 0] = x - 2;
                    loc[i, 1] = z;
                    i++;
                }

                if (x < mazeWidth * 2 - 1 && maze[x + 2, z])
                {
                    loc[i, 0] = x + 2;
                    loc[i, 1] = z;
                    i++;
                }

                if (z > 1 && maze[x, z - 2])
                {
                    loc[i, 0] = x;
                    loc[i, 1] = z - 2;
                    i++;
                }

                if (z < mazeLength * 2 - 1 && maze[x, z + 2])
                {
                    loc[i, 0] = x;
                    loc[i, 1] = z + 2;
                    i++;
                }

                if (i == 0) return; /* check for dead end */
                i = Random.Next(i); /* choose a path */
                maze[(loc[i, 0] + x) / 2, (loc[i, 1] + z) / 2] = false; /* knock out a wall */
                maze[loc[i, 0], loc[i, 1]] = false; /* clear to it */
                x = loc[i, 0];
                z = loc[i, 1];
                WalkMaze(maze, loc[i, 0], loc[i, 1], mazeWidth, mazeLength); /* repeat */
            }
        }
    }
}