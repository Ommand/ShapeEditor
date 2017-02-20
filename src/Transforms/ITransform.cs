using System;
using System.Windows;

namespace ShapeEditor
{
    interface ITransform
    {
        Point Transform(Point p);
    }

    class Translate : ITransform
    {
        Point delta;
        public Point Transform(Point p)
        {
            return new Point(p.X + delta.X, p.Y + delta.Y);
        }
    }

    class Rotate : ITransform
    {
        Point center;
        double phi;
        public Point Transform(Point p)
        {
            double x = center.X + (p.X - center.X) * Math.Cos(phi) - (p.Y - center.Y) * Math.Sin(phi);
            double y = center.Y + (p.Y - center.Y) * Math.Cos(phi) + (p.X - center.X) * Math.Sin(phi);
            return new Point(x, y);
        }
    }

    class Dilate : ITransform
    {
        Point center;
        double kx, ky;
        public Point Transform(Point p)
        {
            return new Point(kx * p.X + (1 - kx) * center.X, ky * p.Y + (1 - ky) * center.Y);
        }
    }
}
