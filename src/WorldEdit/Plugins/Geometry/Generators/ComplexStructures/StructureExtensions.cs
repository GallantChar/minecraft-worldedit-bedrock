using System;

namespace ShapeGenerator.Generators.ComplexStructures
{
    public static class StructureExtensions
    {
        public static T NextEnum<T>(this Random r) where T : struct, IComparable, IFormattable,
            IConvertible
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(r.Next(values.Length));
        }
    }
}