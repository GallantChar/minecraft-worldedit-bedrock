using System;
using System.Collections.Generic;
using System.Linq;
using WorldEdit.Input;
using WorldEdit.Schematic;

namespace ShapeGenerator
{
    public static class PositionExtension
    {
        public static Point ToPoint(this Position position)
        {
            return new Point {X = position.X, Y = position.Y, Z = position.Z};
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

        public static Position GetAbsolutePosition(this Position absolutePosition, IEnumerable<string> relativePosition,
            List<SavedPosition> savedPositions = null)
        {
            // check saved positions if arg1 is a position name.
            if (savedPositions != null)
            {
                var arg1 = relativePosition.ElementAtOrDefault(0);
                if (arg1 != null && !int.TryParse(arg1, out _) && !arg1.StartsWith("~"))
                {
                    return savedPositions.Single(a => a.Name.Equals(arg1)).Position;
                }
            }

            return relativePosition == null
                ? absolutePosition
                : new Position(GetAbsolutePosition(absolutePosition.X, relativePosition.ElementAtOrDefault(0)),
                    GetAbsolutePosition(absolutePosition.Y, relativePosition.ElementAtOrDefault(1)),
                    GetAbsolutePosition(absolutePosition.Z, relativePosition.ElementAtOrDefault(2)));
        }
    }
}
