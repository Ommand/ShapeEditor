using System.Collections.Generic;
using ShapeEditor.Shapes;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Fabrics
{
    abstract class ShapeFabric
    {
        protected static int idCounter = -1;
        protected Color fillColor = Color.FromRgb(255, 255, 255);
        protected Color borderColor = Color.FromRgb(0, 0, 0);
        protected int borderWidth = 1;

        public void useFillColor(Color color)
        {
            fillColor = color;
        }

        public void useBorderColor(Color color)
        {
            borderColor = color;
        }

        public void useBorderWidth(int width)
        {
            borderWidth = width;
        }

        public abstract Shape createShape();
    }

    class TriangleFabric: ShapeFabric
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