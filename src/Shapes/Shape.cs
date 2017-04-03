using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System;
using System.Runtime.Serialization;

namespace ShapeEditor.Shapes
{
    [Serializable]
    [KnownType(typeof(Ellipse))]
    [KnownType(typeof(Triangle))]
    [KnownType(typeof(Line))]
    [KnownType(typeof(Quadrangle))]
    public abstract class Shape
    {
        protected IEnumerable<Point> Points { get; set; }

        public IEnumerable<Point> GetPoint
        {
            get { return this.Points; }
        }

        public abstract bool IsInside(Point point);

        public void ApplyTransformation(ITransform transform)
        {
            List<Point> newPoints = new List<Point>();

            foreach (Point point in Points)
            { 
                Point newPoint = transform.Transform(point);               
                newPoints.Add(newPoint);
            }

            Points = newPoints;
        }

        public virtual Point GetCenter()
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

        public virtual IEnumerable<Point> FormSelection()
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            foreach (Point point_ in Points)
            {
                x.Add(point_.X);
                y.Add(point_.Y);
            }

            List<Point> selectionPoints = new List<Point>();
            selectionPoints.Add(new Point(x.Min(), y.Max()));
            selectionPoints.Add(new Point(x.Max(), y.Min()));

            return selectionPoints;
        }
    }
}