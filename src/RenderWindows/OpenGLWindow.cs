using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShapeEditor;
using ShapeEditor.Shapes;

namespace ShapeEditor.src.RenderWindows
{
    public partial class OpenGLWindow : UserControl
    {
        int oglContext_;
        double sizeX;
        IntPtr hdc;
        public List<IDrawable2DShape> Shapes { get; set; }
        protected override void OnPaintBackground(PaintEventArgs pevent) { }
        public OpenGLWindow()
        {
            CenterX = CenterY = 0;
            sizeX = 1;
            InitializeComponent();
            oglContext_ = OpenGL.InitOpenGL((int)Handle);

            hdc = Win32.GetDC(Handle);

            Win32.wglMakeCurrent(hdc, (IntPtr)oglContext_);

            OpenGL.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            OpenGL.glBlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            OpenGL.glEnable(OpenGL.GL_BLEND);
            // Сглаживание точек
            OpenGL.glEnable(OpenGL.GL_POINT_SMOOTH);
            OpenGL.glHint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);
            // Сглаживание линий
            OpenGL.glEnable(OpenGL.GL_LINE_SMOOTH);
            OpenGL.glHint(OpenGL.GL_LINE_SMOOTH_HINT, OpenGL.GL_NICEST);
            // Сглаживание полигонов    
            OpenGL.glEnable(OpenGL.GL_POLYGON_SMOOTH);
            OpenGL.glHint(OpenGL.GL_POLYGON_SMOOTH_HINT, OpenGL.GL_NICEST);
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        }

        public void GetOrthoValue(int X, int Y, out double XOrth, out double YOrth)
        {
            double coefWnd = Height / (double)Width;

            XOrth = X * (2 * sizeX) / (double)Width + CenterX - sizeX;
            YOrth = (Height - Y) * (2 * sizeX * coefWnd) / (double)Height + CenterY - sizeX * coefWnd;
            ChangeOrth();
        }
        public void GetWindowValue(double XOrth, double YOrth, out int X, out int Y)
        {
            double coefWnd = Height / (double)Width;

            X = (int)(Width * (XOrth - (CenterX - sizeX)) / (2 * sizeX));
            Y = Height - (int)(Height * (YOrth - (CenterY - sizeX * coefWnd)) / (2 * sizeX * coefWnd));
            ChangeOrth();
        }
        public void TranslateI(int deltHor, int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / Width;
            CenterX += deltHor * coef;
            CenterY += deltVert * coef;
            ChangeOrth();
        }
        public void TranslateD(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            CenterX += deltHor;
            CenterY += deltVert;
            ChangeOrth();
        }
        public void SetTranslateI(int deltHor, int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / Width;
            CenterX = deltHor * coef;
            CenterY = deltVert * coef;
            ChangeOrth();
        }
        public void SetTranslateD(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            CenterX = deltHor;
            CenterY = deltVert;
            ChangeOrth();
        }
        public double CenterX { get; private set; }

        public double CenterY { get; private set; }

        public void Scale(double sc) ///изменение размера видимой области
        {
            sizeX = sc;
            ChangeOrth();
        }
        private void OpenGLWindow_Paint(object sender, PaintEventArgs e)
        {
            if (oglContext_ == 0) return;
            hdc = Win32.GetDC(Handle);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglContext_);
            OpenGL.glClear(OpenGL.GL_COLOR_BUFFER_BIT);

            IRenderer renderMy = new Renderers.RendererOpenGl((int)hdc);

            if (Shapes != null)
                foreach (var shape in Shapes)
                    shape.Draw(renderMy);


            Win32.SwapBuffers(hdc);
            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Win32.ReleaseDC(Handle, hdc);
        }

        private void ChangeOrth()
        {
            hdc = Win32.GetDC(Handle);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglContext_);
            OpenGL.glClearColor(1, 1, 1, 1);
            OpenGL.glMatrixMode(OpenGL.GL_PROJECTION);
            OpenGL.glLoadIdentity();
            OpenGL.glViewport(0, 0, Width, Height);
            double coefWnd = Height / (double)Width;
            OpenGL.glOrtho(CenterX - sizeX, CenterX + sizeX, CenterY - sizeX * coefWnd, CenterY + sizeX * coefWnd, -1, 1);
            OpenGL.glMatrixMode(OpenGL.GL_MODELVIEW);
            OpenGL.glLoadIdentity();

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Win32.ReleaseDC(Handle, hdc);
        }
        private void OpenGLWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            ChangeOrth();
        }
    }
}
