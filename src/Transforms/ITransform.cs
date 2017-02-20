using System;
using System.Windows;

namespace ShapeEditor
{
    interface ITransform
    {
        Point Transform(Point p);
    }
}
