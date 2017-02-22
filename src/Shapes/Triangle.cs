using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace ShapeEditor.Shapes
{
    class Triangle : IShape, IDrawable2DShape
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
            Point[] pointsList = Points.ToArray();

            double a = (pointsList[0].X - point.X) * (pointsList[1].Y - pointsList[0].Y) -
                       (pointsList[0].Y - point.Y) * (pointsList[1].X - pointsList[0].X);
            double b = (pointsList[1].X - point.X) * (pointsList[2].Y - pointsList[1].Y) -
                       (pointsList[1].Y - point.Y) * (pointsList[2].X - pointsList[1].X);
            double c = (pointsList[2].X - point.X) * (pointsList[0].Y - pointsList[2].Y) -
                       (pointsList[2].Y - point.Y) * (pointsList[0].X - pointsList[2].X);

            if (a >= 0 && b >= 0 && c >= 0 || a <= 0 && b <= 0 && c <= 0)
                return true;

            return false;
        }

        public IEnumerable<Point> GetShapePoints(double scale)
        {
            List<Point>  pointsList = Points.ToList();
            pointsList.Add(pointsList[0]);
            return pointsList;
        }

        public void ApplyTransformation(ITransform transform)
        {
            throw new System.NotImplementedException();
        }
    }
}