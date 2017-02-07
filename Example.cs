using System;
using System.Collections.Generic;

namespace MyProg
{
    struct Point { }
    struct Color { }

    interface IRenderer
    {
        void DrawPolygon(IEnumerable<Point> points, Color color);
        void FillPolygon(IEnumerable<Point> points, Color color,Color fillColor);
        void DrawText(string text, Point origin, Color color);
    }

    interface IDrawable
    {
        void Draw(IRenderer render);
        bool IsInside(Point point);
    }

    interface IShape
    {
        IEnumerable<Point> Points { get; }
        void UpdatePoints(ITransform transform);
    }

    interface ICaption
    {
        string Text { get; set; }
    }

    interface ITransform
    {
        Point Transform(Point p);
    }

    abstract class ShapeFabric
    {
        protected static ShapeFabric Current { get; set; }
        protected abstract IShape InternalCreateShape(string json);
        public static IShape CreateShape(string json)
        {
            return Current.InternalCreateShape(json);
        }
    }

    class SuperShapeFabric : ShapeFabric
    {
        protected override IShape InternalCreateShape(string json)
        {
            throw new NotImplementedException();
        }

        public static void SetCurrent()
        {
            Current = new SuperShapeFabric();
        }
    }
}
