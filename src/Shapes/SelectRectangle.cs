using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
            Points = new List<Point> { point1, point2 };
        }

        public void Draw(IRenderer render)
        {
            List<Point> shapePoints = GetShapePoints().ToList();
            render.DrawPolygon(shapePoints, BorderColor, BorderWidth);
        }

        public Color FillColor { get; set; }
        public Color BorderColor { get; set; }
        public float BorderWidth { get; set; }

        public override bool IsInside(Point point)
        {
            Point[] pointsList = Points.ToArray();

            if(pointsList[0].X <= point.X && point.X <= pointsList[1].X &&
               pointsList[0].Y <= point.Y && point.Y <= pointsList[1].Y)
                return true;

            return false;
        }

        public bool IsOnBoundary(ShapeEditor.Fabrics.PointPlaces.PointPlace pointPlace, Point point)
        {
            double eps = 1e-10;
            Point[] shapePoints = Points.ToArray();

            if (Math.Abs(shapePoints[0].X - point.X) <= eps)
            {
                if (Math.Abs(shapePoints[0].Y - point.Y) <= eps)
                {
                    pointPlace = Fabrics.PointPlaces.PointPlace.LeftUpCorner;
                    return true;
                }
                else
                {
                    if (Math.Abs(shapePoints[1].Y - point.Y) <= eps)
                    { 
                        pointPlace = Fabrics.PointPlaces.PointPlace.LeftLowCorner;
                        return true;
                    }

                    pointPlace = Fabrics.PointPlaces.PointPlace.LeftEdge;
                    return true;
                }
            }

            if (Math.Abs(shapePoints[1].X - point.X) <= eps)
            {
                if (Math.Abs(shapePoints[0].Y - point.Y) <= eps)
                {
                    pointPlace = Fabrics.PointPlaces.PointPlace.RightUpCorner;
                    return true;
                }
                else
                {
                    if (Math.Abs(shapePoints[1].Y - point.Y) <= eps)
                    {
                        pointPlace = Fabrics.PointPlaces.PointPlace.RightLowCorner;
                        return true;
                    }

                    pointPlace = Fabrics.PointPlaces.PointPlace.RightEdge;
                    return true;
                }
            }

            if (Math.Abs(shapePoints[0].Y - point.Y) <= eps)
            {
                pointPlace = Fabrics.PointPlaces.PointPlace.UpEdge;
                return true;
            }

            if (Math.Abs(shapePoints[1].Y - point.Y) <= eps)
            {
                pointPlace = Fabrics.PointPlaces.PointPlace.LowEdge;
                return true;
            }

            return false;
        }

        public IEnumerable<Point> GetShapePoints()
        {
            Point[] pointsList = Points.ToArray();
            List<Point> shapePoints = new List<Point>();
            shapePoints.Add(pointsList[0]);
            shapePoints.Add(new Point(pointsList[1].X, pointsList[0].Y));
            shapePoints.Add(pointsList[1]);
            shapePoints.Add(new Point(pointsList[0].X, pointsList[1].Y));

            return shapePoints;
        }
    }
}
