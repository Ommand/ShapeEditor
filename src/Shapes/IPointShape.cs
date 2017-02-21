using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Shapes
{
    interface IPointShape : IShape
    {
        IEnumerable<Point> Points { get; }
    }
}