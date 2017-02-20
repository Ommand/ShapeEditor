using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

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

        public abstract bool isInside(Point point);
        public abstract void applyTransformation(ITransform transform);
    }
}