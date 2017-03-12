using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeEditor.Renderers
{
    class RendererWpf : IRenderer
    {
        DrawingContext _contex;
        ShapeEditor.src.RenderWindows.WpfWindow window;
        public RendererWpf(DrawingContext ctx, ShapeEditor.src.RenderWindows.WpfWindow win)
        {
            _contex = ctx;
            window = win;
        }

        public void DrawLine(IEnumerable<Point> points, Color color, float width)
        {
            var geometry = new StreamGeometry();
            List<Point> listPoints = new List<Point>();
            foreach (var p in points)
            {
                int x, y;
                window.GetWindowValue(p.X, p.Y, out x, out y);
                listPoints.Add(new Point(x, y));
            }
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(listPoints[0], true /* is filled */, false /* is closed */);//если замыкать то здесь  true
                for (int i = 1; i < listPoints.Count; i++)
                    ctx.LineTo(listPoints[i], true /* is stroked */, false /* is smooth join */);
            }

            Pen myPen = new Pen(new SolidColorBrush(color), width); //бордер
            _contex.DrawGeometry(null, myPen, geometry);
        }
        public void DrawBoundingBox(Point pointLeftBot, Point pointRightTop)
        {

            Color color = Color.FromRgb(128, 128, 128);

            var geometry = new StreamGeometry();
            List<Point> listPoints = new List<Point>();
            int x, y;
            window.GetWindowValue(pointLeftBot.X, pointLeftBot.Y, out x, out y);
            Point localleftbot = new Point(x, y);

            window.GetWindowValue(pointRightTop.X, pointRightTop.Y, out x, out y);
            Point localrighttop = new Point(x, y);

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(localleftbot, true /* is filled */, true /* is closed */);//если замыкать то здесь  true
                ctx.LineTo(new Point(localrighttop.X, localleftbot.Y), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point(localrighttop.X, localrighttop.Y), true /* is stroked */, false /* is smooth join */);
                ctx.LineTo(new Point(localleftbot.X, localrighttop.Y), true /* is stroked */, false /* is smooth join */);

            }

            Pen myPen = new Pen(new SolidColorBrush(color), 1); //бордер
            myPen.DashStyle = new DashStyle(new double[2] { 5, 5 }, 0);

            _contex.DrawGeometry(null, myPen, geometry);


        }
        public void DrawPolygon(IEnumerable<Point> points, Color color, float width)
        {

            var geometry = new StreamGeometry();
            List<Point> listPoints = new List<Point>();
            foreach (var p in points)
            {
                int x, y;
                window.GetWindowValue(p.X, p.Y, out x, out y);
                listPoints.Add(new Point(x, y));
            }
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(listPoints[0], true /* is filled */, true /* is closed */);//если замыкать то здесь  true
                for (int i = 1; i < listPoints.Count; i++)
                    ctx.LineTo(listPoints[i], true /* is stroked */, false /* is smooth join */);
            }

            Pen myPen = new Pen(new SolidColorBrush(color), width); //бордер

            _contex.DrawGeometry(null, myPen, geometry);
        }
        public void FillPolygon(IEnumerable<Point> points, Color color, Color fillColor, float width)
        {
            var geometry = new StreamGeometry();
            List<Point> listPoints = new List<Point>();
            foreach (var p in points)
            {
                int x, y;
                window.GetWindowValue(p.X, p.Y, out x, out y);
                listPoints.Add(new Point(x, y));
            }
            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(listPoints[0], true /* is filled */, true /* is closed */);//если замыкать то здесь  true
                for (int i = 1; i < listPoints.Count; i++)
                    ctx.LineTo(listPoints[i], true /* is stroked */, false /* is smooth join */);
            }

            _contex.DrawGeometry(new SolidColorBrush(fillColor), null, geometry);
            DrawPolygon(points, color, width);
        }
        public void DrawText(string text, Point origin, Color color)
        {

            int x, y;
            window.GetWindowValue(origin.X, origin.Y, out x, out y);
            _contex.DrawText(new FormattedText(text, CultureInfo.GetCultureInfo("en-us"),
      FlowDirection.LeftToRight,
      new Typeface("Times New Roman"),
      16,
      new SolidColorBrush(color)), new Point(x, y));


        }
    }
}