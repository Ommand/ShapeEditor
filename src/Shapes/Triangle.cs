using System.Collections.Generic;
using System.Windows;
using System.Drawing;

namespace ShapeEditor.Shapes
{
    class Triangle : PointShape
    {
        public override bool isInside(System.Windows.Point point)
        {
            return false;
        }

        public override void applyTransformation(ITransform transform)
        {

        }

        public Triangle(int id_, 
                        System.Windows.Point point1, 
                        System.Windows.Point point2, 
                        System.Windows.Point point3, 
                        Color borderColor_, 
                        Color fillColor_, 
                        int borderWidth_)
        {
            List<System.Windows.Point> listPoint = new List<System.Windows.Point> { point1, point2, point3 };
            points = listPoint;
            id = id_;
            borderColor = borderColor_;
            fillColor = fillColor_;
            borderWidth = borderWidth_;
        }

    }
}