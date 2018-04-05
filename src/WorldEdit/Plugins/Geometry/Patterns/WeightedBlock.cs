using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeGenerator.Generators.Patterns
{
    public class WeightedBlock
    {
        public double Frequency { get; set; }
        public double CalculatedFrequency { get; set; }
        public string BlockName { get; set; }

        public WeightedBlock(string block, double frequency)
        {
            BlockName = block;
            Frequency = frequency;
        }
    }
}
