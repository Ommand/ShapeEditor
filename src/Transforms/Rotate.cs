using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor
{
    class Rotate : ITransform
    {
        Point center;
        double phi;

        public Rotate(Point _center, double _phi)
        {
            center = _center;
            phi = _phi;
        }
        public Point Transform(Point p)
        {
            double x = center.X + (p.X - center.X) * Math.Cos(phi) - (p.Y - center.Y) * Math.Sin(phi);
            double y = center.Y + (p.Y - center.Y) * Math.Cos(phi) + (p.X - center.X) * Math.Sin(phi);
            return new Point(x, y);
        }
    }
}
