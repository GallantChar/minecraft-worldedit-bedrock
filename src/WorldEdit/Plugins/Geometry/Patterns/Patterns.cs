using System.Collections.Generic;

namespace ShapeGenerator.Generators.Patterns
{
    static class Patterns
    {
        private static Dictionary<string, IPattern> blocks;
        private static Dictionary<string, ITransformer> transformers;

        public static Dictionary<string, IPattern> Blocks
        {
            get
            {
                if (blocks == null) InitializePatterns();
                return blocks;
            }
        }

        public static Dictionary<string, ITransformer> Transformers
        {
            get
            {
                if (transformers == null) InitializeTransformers();
                return transformers;
            }
        }

        private static void InitializeTransformers()
        {
            transformers = new Dictionary<string, ITransformer>
            {
                {WhiteVillage.Name, new WhiteVillage()}
            };
        }

        private static void InitializePatterns()
        {
            blocks = new Dictionary<string, IPattern>
            {
                {Castle.Name, new Castle()},
                {CastleWithVines.Name, new CastleWithVines() },
                {WhiteVillage.Name, new WhiteVillage()}
            };
        }

        public static bool IsPattern(string name) => Blocks.ContainsKey(name);

        public static string GetBlock(List<Point> allPoints, Point point) => IsPattern(point.BlockName) ? Blocks[point.BlockName].GetBlock(allPoints, point) : point.BlockName;
    }
}
