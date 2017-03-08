using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Line : IShape, IDrawable2DShape
    {
        public Line(IEnumerable<Point> points,
                    Color borderColor,
                    Color fillColor,
                    double borderWidth) : this(points)
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
            render.DrawLine(Points, FillColor);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public double BorderWidth { get; set; }
        public IEnumerable<Point> Points { get; }
        public bool IsInside(Point point)
        {
            return false;
        }

        public IEnumerable<Point> GetShapePoints(double scale)
        {
            return Points;
        }

        public void ApplyTransformation(ITransform transform)
        {
            foreach (Point point in Points)
                transform.Transform(point);
        }
    }
}
