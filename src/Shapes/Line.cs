using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
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
            return false;
        }

    }
}
