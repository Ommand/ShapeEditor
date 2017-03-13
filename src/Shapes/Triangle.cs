using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System;

namespace ShapeEditor.Shapes
{
    class Triangle : Shape, IDrawable2DShape
    {
        public Triangle(Point point1,
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
        public Triangle(Point point1, Point point2, Point point3)
        {
            if(SpecialMath.AreOnOneLine(point1, point2, point3))
                throw new Exception("Cannot create triangle: points are on one line");

            Points = new List<Point> { point1, point2, point3 };
        }


        public void Draw(IRenderer render)
        {
            render.FillPolygon(Points, BorderColor, FillColor, BorderWidth);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }

        public override bool IsInside(Point point)
        {
            Point[] pointsList = Points.ToArray();
            Point ma = SpecialMath.GetVector(point, pointsList[0]);
            Point mb = SpecialMath.GetVector(point, pointsList[1]);
            Point mc = SpecialMath.GetVector(point, pointsList[2]);
            Point ab = SpecialMath.GetVector(pointsList[0], pointsList[1]);
            Point bc = SpecialMath.GetVector(pointsList[1], pointsList[2]);
            Point ca = SpecialMath.GetVector(pointsList[2], pointsList[0]);
            double productMAAB = SpecialMath.VectorProductAB(ma, ab);
            double productMBBC = SpecialMath.VectorProductAB(mb, bc);
            double productMCCA = SpecialMath.VectorProductAB(mc, ca);

            if (productMAAB >= 0 && productMBBC >= 0 && productMCCA >= 0 ||
                productMAAB <= 0 && productMBBC <= 0 && productMCCA <= 0)
                return true;

            return false;
        }
    }
}