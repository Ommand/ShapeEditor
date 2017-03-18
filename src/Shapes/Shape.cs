using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System;

namespace ShapeEditor.Shapes
{
    public abstract class Shape
    {
        protected IEnumerable<Point> Points { get; set; }

        public abstract bool IsInside(Point point);

        public void ApplyTransformation(ITransform transform)
        {
            List<Point> newPoints = new List<Point>();

            foreach (Point point in Points)
            { 
                transform.Transform(point);
                newPoints.Add(point);
            }

            Points = newPoints;
        }

        public Point GetCenter()
        {
            int count = Points.Count();
            double xC = 0, yC = 0;

            foreach (Point point in Points)
            {
                xC += point.X;
                yC += point.Y;
            }

            Point center = new Point(xC / count, yC / count);
            return center;
        }
    }
}