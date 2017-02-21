using System;
using System.Windows;

namespace ShapeEditor
{
    public interface ITransform
    {
        Point Transform(Point p);
    }
}
