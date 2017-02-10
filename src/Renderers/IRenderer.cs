using System.Collections.Generic;

namespace ShapeEditor
{
    interface IRenderer
    {
        void DrawPolygon(IEnumerable<Point> points, Color color);
        void FillPolygon(IEnumerable<Point> points, Color color,Color fillColor);
        void DrawText(string text, Point origin, Color color);
    }
}