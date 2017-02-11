using System.Collections.Generic;
using System.Windows;

namespace ShapeEditor.Shapes
{
    interface IShape 
    {
        IEnumerable<Point> Points { get; }

        void UpdatePoints(ITransform transform);
    }
}