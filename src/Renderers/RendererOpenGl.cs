using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using SharpGL;

namespace ShapeEditor.Renderers
{
    class RendererOpenGl : IRenderer
    {
        SharpGL.WPF.OpenGLControl _openGlRender;
        
        public RendererOpenGl(SharpGL.WPF.OpenGLControl control)
        {
            _openGlRender = control;
            InitOpenGl(control);

        }
        private void InitOpenGl(SharpGL.WPF.OpenGLControl control)
        {
            control.Name = "OpenGLRender";
            control.OpenGLDraw += OpenGLControl_OpenGLDraw;
            control.OpenGLInitialized += OpenGLControl_OpenGLInitialized;
            control.FrameRate = 60;
            control.Resized += OpenGLControl_Resized;
            control.BorderBrush = Brushes.Gray;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_BLEND);
            // Сглаживание точек
            gl.Enable(OpenGL.GL_POINT_SMOOTH);
            gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
            // Сглаживание линий
            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.Hint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);
            // Сглаживание полигонов    
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH);
            gl.Hint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            gl.Clear(SharpGL.OpenGL.GL_COLOR_BUFFER_BIT | SharpGL.OpenGL.GL_DEPTH_BUFFER_BIT);

//            WpfRender.Children.Clear();
//            foreach (var p in listShapes)
//            {
//                renderer.DrawBoundingBox(new Point(-0.5, -0.5), new Point(0.5, 0.5));
//                p.Draw(renderer);
//                renderer.DrawText("SSSSSSSSSS", new Point(0, 0), Color.FromRgb(0, 255, 0));
//            }
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double coef = _openGlRender.ActualHeight / _openGlRender.ActualWidth;
            double minx = -1, miny = coef * -1, maxx = 1, maxy = coef;
            gl.Ortho(minx, maxx, miny, maxy, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }
        public void DrawBoundingBox(Point pointLeftBot, Point pointRightTop)
        {
            var gl = _openGlRender.OpenGL;
            gl.LineStipple(1, 0x0101);
            gl.Enable(SharpGL.OpenGL.GL_LINE_STIPPLE);
            gl.Begin(SharpGL.OpenGL.GL_LINES);
            double size = 3.0 * 2.0 / _openGlRender.ActualWidth;
            gl.Color(0.5f, 0.5f, 0.5f);

            gl.Vertex(pointLeftBot.X, pointLeftBot.Y, 0.1);
            gl.Vertex(pointRightTop.X, pointLeftBot.Y, 0.1);

            gl.Vertex(pointLeftBot.X, pointRightTop.Y, 0.1);
            gl.Vertex(pointRightTop.X, pointRightTop.Y, 0.1);

            gl.Vertex(pointLeftBot.X, pointLeftBot.Y, 0.1);
            gl.Vertex(pointLeftBot.X, pointRightTop.Y, 0.1);

            gl.Vertex(pointRightTop.X, pointLeftBot.Y, 0.1);
            gl.Vertex(pointRightTop.X, pointRightTop.Y, 0.1);

            gl.End();

            gl.Disable(SharpGL.OpenGL.GL_LINE_STIPPLE);
        }
        public void DrawPolygon(IEnumerable<Point> points, Color color)
        {
            var gl = _openGlRender.OpenGL;
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(color.R, color.G, color.B);
            foreach (var p in points)
            {
                gl.Vertex(p.X, p.Y, 0);
            }
            gl.End();
        }
        public void FillPolygon(IEnumerable<Point> points, Color color, Color fillColor)
        {
            var gl = _openGlRender.OpenGL;

            gl.Enable(SharpGL.OpenGL.GL_POLYGON_OFFSET_FILL);
            gl.PolygonOffset(1.0f, 2.0f);

            gl.Begin(SharpGL.OpenGL.GL_POLYGON);
            gl.Color(fillColor.R, fillColor.G, fillColor.B);
            foreach (var p in points)
            {
                gl.Vertex(p.X, p.Y, 0);
            }
            gl.End();

            gl.Disable(SharpGL.OpenGL.GL_POLYGON_OFFSET_FILL);

            gl.Begin(SharpGL.OpenGL.GL_LINE_LOOP);
            gl.Color(color.R, color.G, color.B);
            foreach (var p in points)
            {
                gl.Vertex(p.X, p.Y, 0);
            }
            gl.End();
        }
        double ToWindowX(double x) { return (x + 1) * _openGlRender.ActualWidth / 2; }// (x - мировое значение)
        double ToWindowY(double y)
        {
            double coef = (((double)(_openGlRender.ActualHeight)) / _openGlRender.ActualWidth);
            return _openGlRender.ActualHeight - (y + coef) * _openGlRender.ActualHeight / (2 * coef);
        }
        public void DrawText(string text, Point origin, Color color)
        {
            var gl = _openGlRender.OpenGL;
            gl.DrawText((int)ToWindowX(origin.X), (int)ToWindowY(origin.Y), color.R, color.G, color.B, "Arial", 11, text);
        }
    }
}