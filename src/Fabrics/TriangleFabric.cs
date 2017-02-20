using System.Collections.Generic;
using ShapeEditor.Shapes;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Fabrics
{
    class TriangleFabric : ShapeFabric
    {
        List<Point> points;

        public TriangleFabric()
        {
            points = new List<Point> { new Point(0, 0), new Point(10, 20), new Point(20, 0) };
        }

        public override Shape createShape()
        {
            idCounter++;
            return new Triangle(idCounter, points[0], points[1], points[2], borderColor, fillColor, borderWidth);
        }
    }
}
