using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    public interface ITransformer
    { 
        void Transform(List<Point> points);
    }
}
