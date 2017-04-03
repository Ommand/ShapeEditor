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
    class Line : Shape, IDrawable2DShape
    {
        public Line(IEnumerable<Point> points,
                    Color borderColor,
                    Color fillColor,
                    float borderWidth) : this(points)
        {
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
        }
        public Line(IEnumerable<Point> points)
        {
            Points = points;
        }


        public void Draw(IRenderer render)
        {
            render.DrawLine(Points, FillColor, BorderWidth);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }

        public override bool IsInside(Point point)
        {
            double epsDistance = 0.05;
            Point[] shapePoints = Points.ToArray();
            List<double> x = new List<double>();
            foreach(Point point_ in shapePoints)
                x.Add(point_.X);

            if(x.Min() <= point.X && point.X <= x.Max())
            { 
                for(int i = 0; i < shapePoints.Length - 1; i++)
                {
                    double a = shapePoints[i].Y - shapePoints[i + 1].Y;
                    double b = shapePoints[i + 1].X - shapePoints[i].X;
                    double c = shapePoints[i].X * shapePoints[i + 1].Y -
                               shapePoints[i + 1].X * shapePoints[i].Y;
                    double y = -a / b * point.X - c / b;
                    if(Math.Abs(y - point.Y) <= epsDistance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
