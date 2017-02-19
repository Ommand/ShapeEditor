using System.Collections.Generic;
using System.Windows;
using System.Drawing;

namespace ShapeEditor.Shapes
{
    abstract class Shape 
    {
        protected Color fillColor { get; set; }
        protected Color borderColor { get; set; }
        protected int borderWidth { get; set; }
        protected int id;

        public int getId()
        {
            return id;
        }

        public abstract bool isInside(System.Windows.Point point);
        public abstract void applyTransformation(ITransform transform);
    }

    abstract class PointShape: Shape
    {
        protected IEnumerable<System.Windows.Point> points;
        public IEnumerable<System.Windows.Point> getPoints()
        {
            return points;
        }
    }
}