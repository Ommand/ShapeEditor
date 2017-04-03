﻿using ShapeEditor.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeEditor.src.IO
{
    public interface IOShapeEditor
    {
        int saveShapes(List<Shape> shapes, String filePath);
        List<Shape> loadShapes(String filePath);
    }
}