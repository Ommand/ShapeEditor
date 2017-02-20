using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    abstract class PointShape : Shape
    {
        protected IEnumerable<Point> points;
        public IEnumerable<Point> getPoints()
        {
            return points;
        }
    }
}