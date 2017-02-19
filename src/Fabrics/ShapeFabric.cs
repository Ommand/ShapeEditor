using System.Collections.Generic;
using ShapeEditor.Shapes;
using System.Drawing;

namespace ShapeEditor.Fabrics
{
    abstract class ShapeFabric
    {
        protected static int idCounter = -1;
        protected Color fillColor;
        protected Color borderColor;
        protected int borderWidth;

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
        List<System.Windows.Point> points;

        TriangleFabric()
        {
            borderColor = Color.Black;
            fillColor = Color.White;
            borderWidth = 1;
            points = new List<System.Windows.Point> { new System.Windows.Point(0, 0), new System.Windows.Point(10, 20), new System.Windows.Point(20, 0) };
        }

        public override Shape createShape()
        {
            idCounter++;
            return new Triangle(idCounter, points[0], points[1], points[2], borderColor, fillColor, borderWidth);
        }
    }
}