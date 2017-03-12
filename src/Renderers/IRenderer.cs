using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor
{
    public interface IRenderer
    {
        void DrawLine(IEnumerable<Point> points, Color color, float width);
        void DrawPolygon(IEnumerable<Point> points, Color color, float width);
        void FillPolygon(IEnumerable<Point> points, Color color, Color fillColor, float width);
        void DrawText(string text, Point origin, Color color);
        void DrawBoundingBox(Point pointLeftBot, Point pointRightTop);
    }
}