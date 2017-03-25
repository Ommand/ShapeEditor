using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ShapeEditor.Fabrics;

namespace ShapeEditor.Shapes
{
    class SelectRectangle : Shape, IDrawable2DShape
    {
        public SelectRectangle(Point point1,
                               Point point2,
                               float borderWidth) : this(point1, point2)
        {
            BorderColor = Color.FromRgb(0, 0, 0);
            FillColor = Color.FromRgb(255, 255, 255);
            BorderWidth = borderWidth;
        }

        public SelectRectangle(Point point1, Point point2)
        {
            //левый верхний угол,првый нижний угол
            Points = new List<Point> { point1, point2 };
        }

        public void Draw(IRenderer render)
        {
            Point[] pointsList = Points.ToArray();
            render.DrawBoundingBox(pointsList[0], pointsList[1]);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }

        public override bool IsInside(Point point)
        {
            Point[] pointsList = Points.ToArray();

            if (pointsList[0].X <= point.X && point.X <= pointsList[1].X &&
               pointsList[0].Y <= point.Y && point.Y <= pointsList[1].Y)
                return true;

            return false;
        }

        public PointPlaces.PointPlace IsOnBoundary(Point point)
        {
            Point[] pointsList = Points.ToArray();
            double eps = Point.Subtract(pointsList[1], pointsList[0]).Length / 20;

            if (Math.Abs(pointsList[0].X - point.X) <= eps)
            {
                if (Math.Abs(pointsList[0].Y - point.Y) <= eps)
                {
                    return PointPlaces.PointPlace.LeftUpCorner;
                }
                else
                {
                    if (Math.Abs(pointsList[1].Y - point.Y) <= eps)
                    {
                        return PointPlaces.PointPlace.LeftLowCorner;
                    }
                    else
                    {
                        if (point.Y > pointsList[1].Y && point.Y < pointsList[0].Y)
                        {
                            return PointPlaces.PointPlace.LeftEdge;
                        }                            
                    }
                }
            }

            if (Math.Abs(pointsList[1].X - point.X) <= eps)
            {
                if (Math.Abs(pointsList[0].Y - point.Y) <= eps)
                {
                    return PointPlaces.PointPlace.RightUpCorner;
                }
                else
                {
                    if (Math.Abs(pointsList[1].Y - point.Y) <= eps)
                    {
                        return PointPlaces.PointPlace.RightLowCorner;
                    }
                    else
                    {
                        if (point.Y > pointsList[1].Y && point.Y < pointsList[0].Y)
                        {
                            return PointPlaces.PointPlace.RightEdge;
                        }
                    }
                }
            }

            if (Math.Abs(pointsList[0].Y - point.Y) <= eps)
            {
                if (point.X > pointsList[0].X && point.X < pointsList[1].X)
                {
                    return PointPlaces.PointPlace.UpEdge;
                }
            }

            if (Math.Abs(pointsList[1].Y - point.Y) <= eps)
            {
                if (point.X > pointsList[0].X && point.X < pointsList[1].X)
                {
                    return PointPlaces.PointPlace.LowEdge;
                }
            }

            return PointPlaces.PointPlace.None;
        }
    }
}
