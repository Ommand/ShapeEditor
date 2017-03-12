using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShapeEditor.src.RenderWindows
{
    /// <summary>
    /// Логика взаимодействия для WpfWindow.xaml
    /// </summary>
    public partial class WpfWindow : UserControl
    {
        double centerX, centerY, sizeX;
        public WpfWindow()
        {
            centerX = centerY = 0;
            sizeX = 1;
            InitializeComponent();
            this.MouseDown += WpfWindow_MouseDown; ;
        }

        private void WpfWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void getOrthoValue(int X, int Y, out double XOrth, out double YOrth)
        {
            double coefWnd = ActualHeight / (double)ActualWidth;

            XOrth = X * (2 * sizeX) / (double)ActualWidth + centerX - sizeX;
            YOrth = (ActualHeight - Y) * (2 * sizeX * coefWnd) / (double)ActualHeight + centerY - sizeX * coefWnd;
      
        }
        public void getWindoValue(double XOrth, double YOrth, out int X, out int Y)
        {
            double coefWnd = ActualHeight / (double)ActualWidth;

            X = (int)(ActualWidth * (XOrth - (centerX - sizeX)) / (2 * sizeX));
            Y = (int)ActualHeight - (int)(ActualHeight * (YOrth - (centerY - sizeX * coefWnd)) / (2 * sizeX * coefWnd));

        }
        public void translatei(int deltHor, int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / ActualWidth;
            centerX += deltHor * coef;
            centerY += deltVert * coef;
        }
        public void translated(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            centerX += deltHor;
            centerY += deltVert;
        }
        public void scale(double sc) ///изменение размера видимой области
        {
            sizeX *= sc;
        }
        protected override void OnRender(DrawingContext dc)
        {
            IRenderer renderMy = new Renderers.RendererWpf(dc,this);


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
        }
    }
}
