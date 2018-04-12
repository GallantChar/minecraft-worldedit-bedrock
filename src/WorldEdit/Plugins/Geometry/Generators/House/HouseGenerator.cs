using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ShapeGenerator.Generators.HouseGen;
using WorldEdit.Output;

namespace ShapeGenerator.Generators
{
    public class HouseGenerator : Generator, IGenerator
    {
        private readonly IMinecraftCommandService _commandService;

        public HouseGenerator()
        {

        }

        public HouseGenerator(IMinecraftCommandService commandService)
        {
            _commandService = commandService;
        }

        List<Line> IGenerator.Run(Options options)
        {
            return TransformToLines(Run(options), options);
        }

        static Random rnd = new Random();

        public List<Point> Run(IHouseOptions options)
        {
            const string GLASS = "glass_pane 0";
            const string WALL = "wallblock";
            const string INSIDE = "inside";
            const string EMPTY = "emtpy";
            const string AIR = "air";


            var house = new House(rnd.Next(2, 7))
            {
                AirBlock = AIR,
                InsideBlock = INSIDE,
                CeilingBlock = $"Top{WALL}",
                WallBlock = WALL,
                GlassBlock = GLASS,
                EmptyBlock = EMPTY,
                InnerDoor = "spruce_door",
                Floors = options.Height
            };

            var points = house.RenderPoints();

            var wallblock = String.IsNullOrEmpty(options.Block) ? Room.RandomBuildingBlock() : options.Block;
            points.Where(p => p.BlockName == house.CeilingBlock || p.BlockName == house.WallBlock).ToList().ForEach(p => p.BlockName = wallblock);

            points.ForEach(p =>
            {
                p.X += options.X;
                p.Y += options.Y;
                p.Z += options.Z;
            });
            return points.Select(p => (Point) p).ToList();
        }
    }
}
