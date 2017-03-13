using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Ellipse : IShape, IDrawable2DShape
    {
        public Ellipse(Point point1,
                        Point point2,
                        Point point3,
                        Color borderColor,
                        Color fillColor,
                        float borderWidth) : this(point1, point2, point3)
        {
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
        }
        public Ellipse(Point point1, Point point2, Point point3)
        {
            if (SpecialMath.AreOnOneLine(point1, point2, point3))
                throw new Exception("Cannot create ellipse: points are on one line");

            Points = new List<Point> { point1, point2, point3 };
        }


        public void Draw(IRenderer render)
        {
            List<Point> shapePoints = GetShapePoints(360).ToList();
            render.FillPolygon(shapePoints, BorderColor, FillColor, BorderWidth);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }
        public IEnumerable<Point> Points { get; }
        public bool IsInside(Point point)
        {
            Point[] pointsList = Points.ToArray();

            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
            SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            double c = SpecialMath.Norm(pointsList[0], pointsList[1]) * 0.5;
            double b = Math.Sqrt(a * a - c * c);
            Point center = GetCenter();

            if (((point.X - center.X) * (point.X - center.X)) / (a * a) +
                ((point.Y - center.Y) * (point.Y - center.Y)) / (b * b) <= 1)
                return true;

            return false;
        }

        public IEnumerable<Point> GetShapePoints(int segmentsCount)
        {
            Point[] pointsList = Points.ToArray();
            List<Point> shapePoints = new List<Point>();

            double k = (pointsList[1].Y - pointsList[0].Y) / (pointsList[1].X - pointsList[0].X);
            double alpha = Math.Atan(k);
            if(k < 0)
            {
                alpha += Math.PI * 0.5;
            }
            Point center = GetCenter();
            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
                        SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            double c = SpecialMath.Norm(pointsList[0], pointsList[1]) * 0.5;
            double b = Math.Sqrt(a * a - c * c);

            double cosA = Math.Cos(alpha);
            double sinA = Math.Sin(alpha);
            double h = Math.PI * 2 / segmentsCount;

            //0<=t<=2*PI
            shapePoints.Add(new Point(cosA * a + center.X, -sinA * a + center.Y));
            for (int i = 1; i < segmentsCount; i++)
            {
                double t = i * h;
                double cosT = Math.Cos(t);
                double sinT = Math.Sin(t);
                Point point = new Point(cosA * a * cosT + sinA * b * sinT + center.X,
                                        -sinA * a * cosT + cosA * b * sinT + center.Y);
                shapePoints.Add(point);
            }

            return shapePoints;
        }

        public void ApplyTransformation(ITransform transform)
        {
            foreach (Point point in Points)
                transform.Transform(point);
        }

        public Point GetCenter()
        {
            Point[] pointsList = Points.ToArray();

            double xC = 0.5 * (pointsList[0].X + pointsList[1].X);
            double yC = 0.5 * (pointsList[0].Y + pointsList[1].Y);

            Point center = new Point(xC, yC);
            return center;
        }
    }
}
