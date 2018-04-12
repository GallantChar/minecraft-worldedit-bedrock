using System;

namespace ShapeGenerator.Generators.HouseGen
{
    public static class Extensions
    {
        public static T NextEnum<T>(this Random r) where T : struct, IComparable, IFormattable,
            IConvertible
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(r.Next(values.Length));
        }
    }
}