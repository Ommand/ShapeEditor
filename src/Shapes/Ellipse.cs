using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace ShapeEditor.Shapes
{
    [Serializable]
    class Ellipse : Shape, IDrawable2DShape
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

        public override bool IsInside(Point point)
        {
            Point[] pointsList = Points.ToArray();

            double k = (pointsList[1].Y - pointsList[0].Y) / (pointsList[1].X - pointsList[0].X);
            double alpha = Math.Atan(k);
            double cosA = Math.Cos(alpha);
            double sinA = Math.Sin(alpha);

            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
            SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            double c = SpecialMath.Norm(pointsList[0], pointsList[1]) * 0.5;
            double b = Math.Sqrt(a * a - c * c);
            Point center = GetCenter();

            double x = point.X - center.X;
            double y = point.Y - center.Y;
            Point pointInEllipseSystem = new Point(x * cosA + y * sinA, -x * sinA + y * cosA);

            if ((pointInEllipseSystem.X * pointInEllipseSystem.X) / (a * a) +
                (pointInEllipseSystem.Y * pointInEllipseSystem.Y) / (b * b) <= 1)
                return true;

            return false;
        }

        public IEnumerable<Point> GetShapePoints(int segmentsCount)
        {
            Point[] pointsList = Points.ToArray();
            List<Point> shapePoints = new List<Point>();

            double k = (pointsList[1].Y - pointsList[0].Y) / (pointsList[1].X - pointsList[0].X);
            double alpha = Math.Atan(k);

            Point center = GetCenter();
            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
                        SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            double c = SpecialMath.Norm(pointsList[0], pointsList[1]) * 0.5;
            double b = Math.Sqrt(a * a - c * c);

            double cosA = Math.Cos(alpha);
            double sinA = Math.Sin(alpha);
            double h = Math.PI * 2 / segmentsCount;

            //0<=t<=2*PI
            shapePoints.Add(new Point(cosA * a + center.X, sinA * a + center.Y));
            for (int i = 1; i < segmentsCount; i++)
            {
                double t = i * h;
                double cosT = Math.Cos(t);
                double sinT = Math.Sin(t);
                Point point = new Point(cosA * a * cosT - sinA * b * sinT + center.X,
                                        sinA * a * cosT + cosA * b * sinT + center.Y);
                shapePoints.Add(point);
            }

            return shapePoints;
        }

        public override Point GetCenter()
        {
            Point[] pointsList = Points.ToArray();

            double xC = 0.5 * (pointsList[0].X + pointsList[1].X);
            double yC = 0.5 * (pointsList[0].Y + pointsList[1].Y);

            Point center = new Point(xC, yC);
            return center;
        }

        public double GetSemiMinorAxis()
        {
            Point[] pointsList = Points.ToArray();
            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
                        SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            double c = SpecialMath.Norm(pointsList[0], pointsList[1]) * 0.5;
            double b = Math.Sqrt(a * a - c * c);
            return b;
        }

        public double GetSemiMajorAxis()
        {
            Point[] pointsList = Points.ToArray();
            double a = (SpecialMath.Norm(pointsList[0], pointsList[2]) +
                        SpecialMath.Norm(pointsList[1], pointsList[2])) * 0.5;
            return a;
        }

        public double AngleBeetweenMajorAxisAndPositiveX()
        {
            Point[] pointsList = Points.ToArray();
            double k = (pointsList[1].Y - pointsList[0].Y) / (pointsList[1].X - pointsList[0].X);
            double alpha = Math.Atan(k);
            return alpha;
        }

        public void BuildEllipse(double semiMinorAxis, 
                                 double semiMajorAxis,
                                 double angleBeetweenMajorAxisAndPositiveX,
                                 Point center)
        {
            double c = Math.Sqrt(semiMajorAxis * semiMajorAxis - semiMinorAxis * semiMinorAxis);
            double cosA = Math.Cos(angleBeetweenMajorAxisAndPositiveX);
            double sinA = Math.Sin(angleBeetweenMajorAxisAndPositiveX);

            Point focus1 = new Point(cosA * (-c) + center.X, sinA * (-c) + center.Y);
            Point focus2 = new Point(cosA * c + center.X, sinA * c + center.Y);
            Point pointM = new Point(-sinA * semiMinorAxis + center.X, cosA * semiMinorAxis + center.Y);

            Points = new List<Point> { focus1, focus2, pointM };
        }

        public override IEnumerable<Point> FormSelection()
        {
            Point[] shapePoints = GetShapePoints(360).ToArray();
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            foreach (Point point_ in shapePoints)
            { 
                x.Add(point_.X);
                y.Add(point_.Y);
            }

            List<Point> selectionPoints = new List<Point>();
            selectionPoints.Add(new Point(x.Min(), y.Max()));
            selectionPoints.Add(new Point(x.Max(), y.Min()));

            return selectionPoints;
        }
    }
}
