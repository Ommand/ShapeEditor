using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeEditor.Renderers
{
    class RendererOpenGl : IRenderer
    {
        int hdc;
        public RendererOpenGl(int _hdc)
        {
            hdc = _hdc;
        }

        public void DrawBoundingBox(Point pointLeftBot, Point pointRightTop)
        {
            OpenGL.glLineStipple(4, 0xAAAA);
            OpenGL.glEnable(OpenGL.GL_LINE_STIPPLE);
            OpenGL.glBegin(OpenGL.GL_LINES);
            OpenGL.glColor3f(0.5f, 0.5f, 0.5f);

            OpenGL.glVertex3d(pointLeftBot.X, pointLeftBot.Y, 0.1);
            OpenGL.glVertex3d(pointRightTop.X, pointLeftBot.Y, 0.1);

            OpenGL.glVertex3d(pointLeftBot.X, pointRightTop.Y, 0.1);
            OpenGL.glVertex3d(pointRightTop.X, pointRightTop.Y, 0.1);

            OpenGL.glVertex3d(pointLeftBot.X, pointLeftBot.Y, 0.1);
            OpenGL.glVertex3d(pointLeftBot.X, pointRightTop.Y, 0.1);

            OpenGL.glVertex3d(pointRightTop.X, pointLeftBot.Y, 0.1);
            OpenGL.glVertex3d(pointRightTop.X, pointRightTop.Y, 0.1);

            OpenGL.glEnd();

            OpenGL.glDisable(OpenGL.GL_LINE_STIPPLE);
        }

        public void DrawText(string text, Point origin, Color color)
        {
            OpenGL.glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            OpenGL.PrintText(hdc,origin.X, origin.Y,0,"Times New Roman",16,text);
        }
        public void DrawLine(IEnumerable<Point> points, Color color, float width)
        {
            OpenGL.glLineWidth(width);
            OpenGL.glBegin(OpenGL.GL_LINE_STRIP);
            OpenGL.glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            foreach (var p in points)
            {
                OpenGL.glVertex3d(p.X, p.Y, 0);
            }
            OpenGL.glEnd();
        }
        public void DrawPolygon(IEnumerable<Point> points, Color color, float width)
        {
            OpenGL.glLineWidth(width);
            OpenGL.glBegin(OpenGL.GL_LINE_LOOP);
            OpenGL.glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
            foreach (var p in points)
            {
                OpenGL.glVertex3d(p.X, p.Y, 0);
            }
            OpenGL.glEnd();
        }
        public void FillPolygon(IEnumerable<Point> points, Color color, Color fillColor, float width)
        {

//            OpenGL.glEnable(OpenGL.GL_POLYGON_OFFSET_FILL);
//            OpenGL.glPolygonOffset(1.0f, 2.0f);

            OpenGL.glBegin(OpenGL.GL_POLYGON);
            OpenGL.glColor3f(fillColor.R/255.0f, fillColor.G/255.0f, fillColor.B/255.0f);
            foreach (var p in points)
            {
                OpenGL.glVertex3d(p.X, p.Y, 0);
            }
            OpenGL.glEnd();

//            OpenGL.glDisable(OpenGL.GL_POLYGON_OFFSET_FILL);

            DrawPolygon(points, color, width);
        }
    }
}