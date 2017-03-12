using System.Collections.Generic;
using System.Windows;

namespace ShapeEditor.Shapes
{
    public interface IShape
    {
        IEnumerable<Point> Points { get; }

        bool IsInside(Point point);
        void ApplyTransformation(ITransform transform);
        IEnumerable<Point> GetShapePoints(int segmentsCount);
        Point GetCenter();
    }
}