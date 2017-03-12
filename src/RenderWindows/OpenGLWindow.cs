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
namespace ShapeEditor.src.RenderWindows
{
    public partial class OpenGLWindow : UserControl
    {
        int oglcontext_ = 0;
        double centerX,centerY,sizeX;
        IntPtr hdc;
        int OglContex{ get { return oglcontext_; } }
        protected override void OnPaintBackground(PaintEventArgs pevent) { }
        public OpenGLWindow()
        {
            centerX = centerY = 0;
            sizeX = 1;
            InitializeComponent();
            oglcontext_ = OpenGL.InitOpenGL((int)Handle);

            hdc = Win32.GetDC(Handle);

            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext_);

            OpenGL.glEnable(OpenGL.GL_DEPTH_TEST);
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

     

        public void getOrthoValue(int X,int Y,out double XOrth,out double YOrth)
        {
            double coefWnd = Height / (double)Width;

            XOrth = X*(2* sizeX) / (double)Width + centerX - sizeX;
            YOrth = (Height-Y) * (2 * sizeX* coefWnd) / (double)Height + centerY - sizeX*coefWnd;
            chengeOrth();
        }
        public void getWindoValue(double XOrth,double YOrth,out int X,out int Y)
        {
            double coefWnd = Height / (double)Width;

            X =  (int) (Width * (XOrth-(centerX - sizeX))/(2*sizeX));
            Y = Height-(int)(Height * (YOrth - (centerY - sizeX*coefWnd)) / (2 * sizeX * coefWnd));
            chengeOrth();
        }
        public void translatei(int deltHor,int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / Width;
            centerX += deltHor * coef;
            centerY += deltVert * coef;
            chengeOrth();
        }
        public void translated(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            centerX += deltHor ;
            centerY += deltVert ;
            chengeOrth();
        }
        public void scale(double sc) ///изменение размера видимой области
        {
            sizeX*= sc;
            chengeOrth();
        }
        private void OpenGLWindow_Paint(object sender, PaintEventArgs e)
        {

            if (oglcontext_ != 0)
            {
                hdc = Win32.GetDC(Handle);
                Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext_);
                OpenGL.glClear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                IRenderer renderMy = new Renderers.RendererOpenGl((int)hdc);

                ////////////////////////////////////////////////////////тут должно быть что то типо
                /// forech (var figure in figures)
                ///    figure.draw(renderMy);
                ///
                //



                ////////////////////////////////////////////////////////////////////////////////// этот блок можешь удалить (отладка была здесь)
                IEnumerable<System.Windows.Point> pointsTr = new List<System.Windows.Point> {new System.Windows.Point(-0.5,-0.5),
                new System.Windows.Point(0.4,-0.4),
                new System.Windows.Point(0.5,0.5)};
                IEnumerable<System.Windows.Point> pointsTr2 = new List<System.Windows.Point> {new System.Windows.Point(-0.5,-0.5),
                new System.Windows.Point(-0.8,-0.8),
                new System.Windows.Point(-1,1)};

                renderMy.FillPolygon(pointsTr, System.Windows.Media.Color.FromRgb(255, 0, 0), System.Windows.Media.Color.FromRgb(0, 255, 0));
                renderMy.DrawText("test", new System.Windows.Point(-0.5, 0), System.Windows.Media.Color.FromRgb(0, 0, 255));
                renderMy.DrawBoundingBox(new System.Windows.Point(-0.6, -0.6), new System.Windows.Point(0.6, 0.6));
                renderMy.DrawLine(pointsTr2, System.Windows.Media.Color.FromRgb(255, 255, 0));

                //////////////////////////////////////////////////////////////////////////////////////////


                Win32.SwapBuffers(hdc);
                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                Win32.ReleaseDC(Handle, hdc);
            }
        }
        private void chengeOrth()
        {
            hdc = Win32.GetDC(Handle);
            Win32.wglMakeCurrent(hdc, (IntPtr)oglcontext_);
            OpenGL.glClearColor(1, 1, 1, 1);
            OpenGL.glMatrixMode(OpenGL.GL_PROJECTION);
            OpenGL.glLoadIdentity();
            OpenGL.glViewport(0, 0, Width, Height);
            double coefWnd = Height / (double)Width;
            OpenGL.glOrtho(centerX - sizeX, centerX + sizeX, centerY - sizeX * coefWnd, centerY + sizeX * coefWnd, -1, 1);
            OpenGL.glMatrixMode(OpenGL.GL_MODELVIEW);
            OpenGL.glLoadIdentity();

            Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            Win32.ReleaseDC(Handle, hdc);
        } 
        private void OpenGLWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            chengeOrth();
        }
    }
}
