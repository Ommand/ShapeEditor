using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Triangle : IPointShape, IDrawable2DShape
    {
        public Triangle(Point point1,
                        Point point2,
                        Point point3,
                        Color borderColor,
                        Color fillColor,
                        double borderWidth) : this(point1, point2, point3)
        {
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
        }
        public Triangle(Point point1, Point point2, Point point3)
        {
            Points = new List<Point> { point1, point2, point3 };
        }


        public void Draw(IRenderer render)
        {
            render.FillPolygon(Points, BorderColor, FillColor);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public double BorderWidth { get; set; }
        public IEnumerable<Point> Points { get; }
        public bool IsInside(Point point)
        {
            throw new System.NotImplementedException();
        }

        public void ApplyTransformation(ITransform transform)
        {
            throw new System.NotImplementedException();
        }
    }
}