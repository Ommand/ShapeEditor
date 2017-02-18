using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeEditor.Renderers
{
    class RendererWpf : IRenderer
    {
        Canvas _canvasRender;
        public RendererWpf(Canvas canvasRender)
        {
            _canvasRender = canvasRender;
        }
        double getx(double x) { return (x) * 2 / _canvasRender.ActualWidth - 1; } //координата x по нажатию (x - пиксельное значение)
        double gety(double y) { double coef = (_canvasRender.ActualHeight / _canvasRender.ActualWidth); return (_canvasRender.ActualHeight - y) * 2 * coef / _canvasRender.ActualHeight - coef; }
        double ToWindowX(double x) { return (x + 1) * _canvasRender.ActualWidth / 2; }// (x - мировое значение)
        double ToWindowY(double y)
        {
            double coef = (((double)(_canvasRender.ActualHeight)) / _canvasRender.ActualWidth);
            return _canvasRender.ActualHeight - (y + coef) * _canvasRender.ActualHeight / (2 * coef);
        }
        public void DrawLine(IEnumerable<Point> points, Color color)
        {
            List<Point> polylineList = new List<Point> { };
            foreach (var p in points)
            {
                polylineList.Add(new Point(ToWindowX(p.X), ToWindowY(p.Y)));
            }
            Polyline linepoly = new Polyline() { Points = new PointCollection(polylineList) };
            SolidColorBrush Brush1 = new SolidColorBrush();
            Brush1.Color = color;
            linepoly.Stroke = Brush1;
            _canvasRender.Children.Add(linepoly);
        }
        public void DrawBoundingBox(Point pointLeftBot, Point pointRightTop)
        {
            Color color = Color.FromRgb(128, 128, 128);

            Line newLine1 = new Line();
            Line newLine2 = new Line();
            newLine1.Stroke = new SolidColorBrush(color);
            newLine2.Stroke = new SolidColorBrush(color);
            newLine1.StrokeDashArray = new DoubleCollection() { 2, 2 };
            newLine2.StrokeDashArray = new DoubleCollection() { 2, 2 };

            newLine1.X1 = ToWindowX(pointRightTop.X);
            newLine1.X2 = ToWindowX(pointLeftBot.X);
            newLine1.Y1 = ToWindowY(pointLeftBot.Y);
            newLine1.Y2 = ToWindowY(pointLeftBot.Y);

            newLine2.X1 = ToWindowX(pointRightTop.X);
            newLine2.X2 = ToWindowX(pointLeftBot.X);
            newLine2.Y1 = ToWindowY(pointRightTop.Y);
            newLine2.Y2 = ToWindowY(pointRightTop.Y);

            _canvasRender.Children.Add(newLine1);
            _canvasRender.Children.Add(newLine2);


            Line newLine3 = new Line();
            Line newLine4 = new Line();
            newLine3.Stroke = new SolidColorBrush(color);
            newLine4.Stroke = new SolidColorBrush(color);
            newLine3.StrokeDashArray = new DoubleCollection() { 2, 2 };
            newLine4.StrokeDashArray = new DoubleCollection() { 2, 2 };
            newLine3.X1 = ToWindowX(pointRightTop.X);
            newLine3.X2 = ToWindowX(pointRightTop.X);
            newLine3.Y1 = ToWindowY(pointLeftBot.Y);
            newLine3.Y2 = ToWindowY(pointRightTop.Y);

            newLine4.X1 = ToWindowX(pointLeftBot.X);
            newLine4.X2 = ToWindowX(pointLeftBot.X);
            newLine4.Y1 = ToWindowY(pointLeftBot.Y);
            newLine4.Y2 = ToWindowY(pointRightTop.Y);

            _canvasRender.Children.Add(newLine3);
            _canvasRender.Children.Add(newLine4);
        }
        public void DrawPolygon(IEnumerable<Point> points, Color color)
        {
            List<Point> polylineList = new List<Point> { };
            foreach (var p in points)
            {
                polylineList.Add(new Point(ToWindowX(p.X), ToWindowY(p.Y)));
            }
            polylineList.Add(polylineList[0]);
            Polyline linepoly = new Polyline() { Points = new PointCollection(polylineList) };
            SolidColorBrush Brush1 = new SolidColorBrush();
            Brush1.Color = color;
            linepoly.Stroke = Brush1;
            _canvasRender.Children.Add(linepoly);
        }
        public void FillPolygon(IEnumerable<Point> points, Color color, Color fillColor)
        {
            List<Point> polygonList = new List<Point> { };
            foreach (var p in points)
            {
                polygonList.Add(new Point(ToWindowX(p.X), ToWindowY(p.Y)));
            }
            Polygon polygon = new Polygon() { Points = new PointCollection(polygonList) };
            SolidColorBrush Brush2 = new SolidColorBrush();
            Brush2.Color = fillColor;
            polygon.Stroke = Brush2;
            polygon.Fill = Brush2;
            _canvasRender.Children.Add(polygon);

            DrawPolygon(points, color);
        }
        public void DrawText(string text, Point origin, Color color)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = 12;
            textBlock.FontFamily = new FontFamily("Arial");
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, ToWindowX(origin.X));
            Canvas.SetTop(textBlock, ToWindowY(origin.Y));

            _canvasRender.Children.Add(textBlock);
        }
    }
}