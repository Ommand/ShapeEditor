using System.Collections.Generic;

namespace ShapeEditor
{
    interface IShape
    {
        IEnumerable<Point> Points { get; }
        void UpdatePoints(ITransform transform);
    }
}