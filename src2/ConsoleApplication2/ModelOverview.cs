﻿using System.Collections.Generic;
using ShapeGenerator;

namespace SchematicExporter
{
    public class ModelOverview
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public List<Layer> Layers { get; set; }
        public Position Minimum { get; set; }
        public Position Maximum { get; set; }
    }
}