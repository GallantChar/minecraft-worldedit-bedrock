using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    interface IPattern
    {
        string GetBlock();
        string GetBlock(List<Point> allPoints, Point currentPoint);
    }
}
