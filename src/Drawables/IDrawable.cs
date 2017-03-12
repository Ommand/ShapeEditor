using System.Windows;

namespace ShapeEditor
{
    public interface IDrawable
    {
        void Draw(IRenderer render);
    }
}