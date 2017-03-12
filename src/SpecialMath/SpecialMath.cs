using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor.Shapes
{
    public static class SpecialMath
    {
        public static bool AreOnOneLine(Point point1, Point point2, Point point3)
        {
            double a = point1.Y - point2.Y;
            double b = point2.X - point1.X;
            double c = point1.X * point2.Y - point2.X * point1.Y;

            if (Math.Abs(a * point3.X + b * point3.Y + c) <= 1e-10)
                return true;

            return false;
        }

        public static double VectorProductAB(Point a, Point b)
        {
            double product = a.X * b.Y - a.Y * b.X;
            return product;
        }

        public static Point GetVector(Point begin, Point end)
        {
            Point vec = new Point(end.X - begin.X, end.Y - begin.Y);
            return vec;
        }

        public static double Norm(Point vec)
        {
            double norm = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
            return norm;
        }

        public static double Norm(Point begin, Point end)
        {
            Point vec = GetVector(begin, end);
            double norm = Norm(vec);
            return norm;
        }
    }
}
