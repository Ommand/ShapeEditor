using System.Windows;

namespace ShapeEditor
{
    interface IDrawable
    {
        void Draw(IRenderer render);
        bool IsInside(Point point);
    }
}