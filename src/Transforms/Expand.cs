using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor
{
    class Expand: ITransform
    {
        Point center;
        double kx, ky;
        public Expand(Point _center, double _kx, double _ky)
        {
            center = _center;
            kx = _kx;
            ky = _ky;
        }
        public Point Transform(Point p)
        {
            return new Point(kx * p.X + (1 - kx) * center.X, ky * p.Y + (1 - ky) * center.Y);
        }
    }
}
