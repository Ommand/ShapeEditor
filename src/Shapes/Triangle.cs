using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Triangle : IShape, IDrawable
    {
        Color colorBorder;
        Color colorFill;
        public IEnumerable<Point> Points { get; }
        public void Draw(IRenderer render)
        {
            render.FillPolygon(Points, colorBorder, colorFill);
        }
        public bool IsInside(Point point)
        {
            return false;
        }
        public void UpdatePoints(ITransform transform)
        {

        }
        public Triangle(Point point1, Point point2, Point point3, Color _colorBorder, Color _colorFill)
        {
            List<Point> listpoint = new List<Point> { point1, point2, point3 };
            Points = listpoint;
            colorBorder = _colorBorder;
            colorFill = _colorFill;
        }

    }
}