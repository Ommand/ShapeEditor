using System.Windows;

namespace ShapeEditor.Shapes
{
    public interface IShape
    {
        bool IsInside(Point point);
        void ApplyTransformation(ITransform transform);
    }
}