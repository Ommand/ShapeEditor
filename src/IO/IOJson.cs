using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeEditor.Shapes;

namespace ShapeEditor.src.IO
{
    class IOJson : IOShapeEditor
    {
        List<Shape> IOShapeEditor.loadShapes(string filePath)
        {
            throw new NotImplementedException();
        }

        int IOShapeEditor.saveShapes(List<Shape> shapes, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
