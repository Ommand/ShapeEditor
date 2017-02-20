using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeEditor
{
    class Translate : ITransform
    {
        Point delta;

        public Translate(Point _delta)
        {
            delta = _delta;
        }
        public Point Transform(Point p)
        {
            return new Point(p.X + delta.X, p.Y + delta.Y);
        }
    }
}
