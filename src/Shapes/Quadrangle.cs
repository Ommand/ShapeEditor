using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Quadrangle : IShape, IDrawable2DShape
    {
        public Quadrangle(Point point1,
                          Point point2,
                          Point point3,
                          Point point4,
                          Color borderColor,
                          Color fillColor,
                          double borderWidth) : this(point1, point2, point3, point4)
        {
            BorderColor = borderColor;
            FillColor = fillColor;
            BorderWidth = borderWidth;
        }

        public Quadrangle(Point point1, Point point2, Point point3, Point point4)
        {
            if (SpecialMath.AreOnOneLine(point1, point2, point3) || SpecialMath.AreOnOneLine(point1, point2, point4) ||
               SpecialMath.AreOnOneLine(point1, point3, point4) || SpecialMath.AreOnOneLine(point2, point3, point4))
                throw new Exception("Cannot create rectangle: 3 points are on one line");

            if (IsConvex(point1, point2, point3, point4))
            {
                Points = new List<Point> { point1, point2, point3, point4 };
            }               
            else
            {
                if (IsConvex(point1, point2, point4, point3))
                {
                    Points = new List<Point> { point1, point2, point4, point3 };
                }
                else
                {
                    if (IsConvex(point1, point3, point2, point4))
                    {
                        Points = new List<Point> { point1, point3, point2, point4 };
                    }
                    else
                    {
                        if (IsConvex(point1, point3, point4, point2))
                        {
                            Points = new List<Point> { point1, point3, point4, point2 };
                        }
                        else
                        {
                            if (IsConvex(point1, point4, point2, point3))
                            {
                                Points = new List<Point> { point1, point4, point2, point3 };
                            }
                            else
                            {
                                Points = new List<Point> { point1, point4, point3, point2 };
                            }
                        }
                    }
                }
            }
        }



        bool IsConvex(Point a, Point b, Point c, Point d)
        {
            Point ba = SpecialMath.GetVector(b, a);
            Point bd = SpecialMath.GetVector(b, d);
            Point bc = SpecialMath.GetVector(b, c);
            Point ab = SpecialMath.GetVector(a, b);
            Point ac = SpecialMath.GetVector(a, c);
            Point ad = SpecialMath.GetVector(a, d);
            double productBABD = SpecialMath.VectorProductAB(ba, bd);
            double productBDBC = SpecialMath.VectorProductAB(bd, bc);
            double productABAC = SpecialMath.VectorProductAB(ab, ac);
            double productACAD = SpecialMath.VectorProductAB(ac, ad);

            if (productBABD >= 0 && productBDBC >= 0 && productABAC >= 0 && productACAD >= 0 ||
               productBABD <= 0 && productBDBC <= 0 && productABAC <= 0 && productACAD <= 0)
                return true;

            return false;
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

            Point ma = SpecialMath.GetVector(point, pointsList[0]);
            Point mb = SpecialMath.GetVector(point, pointsList[1]);
            Point mc = SpecialMath.GetVector(point, pointsList[2]);
            Point md = SpecialMath.GetVector(point, pointsList[3]);

            double productMAMB = SpecialMath.VectorProductAB(ma, mb);
            double productMBMC = SpecialMath.VectorProductAB(mb, mc);
            double productMCMD = SpecialMath.VectorProductAB(mc, md);
            double productMDMA = SpecialMath.VectorProductAB(md, ma);

            if (productMAMB >= 0 && productMBMC >= 0 && productMCMD >= 0 && productMDMA >= 0 ||
                productMAMB <= 0 && productMBMC <= 0 && productMCMD <= 0 && productMDMA <= 0)
                return true;

            return false;
        }

        public IEnumerable<Point> GetShapePoints(double scale)
        {
            List<Point> pointsList = Points.ToList();
            pointsList.Add(pointsList[0]);
            return pointsList;
        }

        public void ApplyTransformation(ITransform transform)
        {
            foreach (Point point in Points)
                transform.Transform(point);
        }

        public Point GetCenter()
        {
            int count = Points.Count();
            double xC = 0, yC = 0;

            foreach (Point point in Points)
            {
                xC += point.X;
                yC += point.Y;
            }

            Point center = new Point(xC / count, yC / count);
            return center;
        }
    }
}
