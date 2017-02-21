using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;

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
            Point[] pointList = Points.ToArray();

            double a = (pointList[0].X - point.X) * (pointList[1].Y - pointList[0].Y) -
                       (pointList[0].Y - point.Y) * (pointList[1].X - pointList[0].X);
            double b = (pointList[1].X - point.X) * (pointList[2].Y - pointList[1].Y) -
                       (pointList[1].Y - point.Y) * (pointList[2].X - pointList[1].X);
            double c = (pointList[2].X - point.X) * (pointList[0].Y - pointList[2].Y) -
                       (pointList[2].Y - point.Y) * (pointList[0].X - pointList[2].X);

            if (a >= 0 && b >= 0 && c >= 0 || a <= 0 && b <= 0 && c <= 0)
                return true;

            return false;
        }

        public void ApplyTransformation(ITransform transform)
        {
            throw new System.NotImplementedException();
        }
    }
}