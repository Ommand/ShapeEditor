using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    class Triangle : PointShape
    {
        public override bool isInside(System.Windows.Point point)
        {
            List<Point> pointList = new List<Point> { };
            foreach(Point p in points)
            {
                pointList.Add(p);
            }

            double a = (pointList[0].X - point.X) * (pointList[1].Y - pointList[0].Y) -
                       (pointList[0].Y - point.Y) * (pointList[1].X - pointList[0].X);
            double b = (pointList[1].X - point.X) * (pointList[2].Y - pointList[1].Y) -
                       (pointList[1].Y - point.Y) * (pointList[2].X - pointList[1].X);
            double c = (pointList[2].X - point.X) * (pointList[0].Y - pointList[2].Y) -
                       (pointList[2].Y - point.Y) * (pointList[0].X - pointList[2].X);

            if (a >= 0 && b >= 0 && c >= 0 || a <= 0 && b <= 0 && c <= 0)
                return true;

            return false;
        }

        public override void applyTransformation(ITransform transform)
        {

        }

        public Triangle(int id_,
                        Point point1,
                        Point point2,
                        Point point3, 
                        Color borderColor_, 
                        Color fillColor_, 
                        int borderWidth_)
        {
            List<Point> pointList = new List<Point> { point1, point2, point3 };
            points = pointList;
            id = id_;
            borderColor = borderColor_;
            fillColor = fillColor_;
            borderWidth = borderWidth_;
        }

    }
}