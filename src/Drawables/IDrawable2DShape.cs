using System.Windows.Media;

namespace ShapeEditor
{
    public interface IDrawable2DShape : IDrawable
    {
        Color FillColor { get; set; }
        Color BorderColor { get; set; }
        double BorderWidth { get; set; }
    }
}