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
        double sizeX;
        public List<IDrawable2DShape> Shapes { get; set; }

        public WpfWindow()
        {
            CenterX = CenterY = 0;
            sizeX = 1;
            InitializeComponent();
            this.Background = Brushes.Transparent;
        }

        public void GetOrthoValue(int X, int Y, out double XOrth, out double YOrth)
        {
            double coefWnd = ActualHeight / (double)ActualWidth;

            XOrth = X * (2 * sizeX) / (double)ActualWidth + CenterX - sizeX;
            YOrth = (ActualHeight - Y) * (2 * sizeX * coefWnd) / (double)ActualHeight + CenterY - sizeX * coefWnd;

        }
        public void GetWindowValue(double XOrth, double YOrth, out int X, out int Y)
        {
            var coefWnd = ActualHeight / ActualWidth;

            X = (int)(ActualWidth * (XOrth - (CenterX - sizeX)) / (2 * sizeX));
            Y = (int)ActualHeight - (int)(ActualHeight * (YOrth - (CenterY - sizeX * coefWnd)) / (2 * sizeX * coefWnd));

        }
        public void TranslateI(int deltHor, int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / ActualWidth;
            CenterX += deltHor * coef;
            CenterY += deltVert * coef;
        }
        public void TranslateD(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            CenterX += deltHor;
            CenterY += deltVert;
        }
        public void SetTranslateI(int deltHor, int deltVert) //перемещеат центр сцены (входные параметры это пиксели в окне)
        {
            double coef = 2 * sizeX / ActualWidth;
            CenterX = deltHor * coef;
            CenterY = deltVert * coef;
        }
        public void SetTranslateD(double deltHor, double deltVert) //перемещеат центр сцены (входные параметры это значение мировых координат)
        {
            CenterX = deltHor;
            CenterY = deltVert;
        }
        public double CenterX { get; private set; }

        public double CenterY { get; private set; }

        public void Scale(double sc) ///изменение размера видимой области
        {
            sizeX = sc;
        }
        protected override void OnRender(DrawingContext dc)
        {
            IRenderer renderMy = new Renderers.RendererWpf(dc, this);

            if (Shapes == null) return;
            foreach (var shape in Shapes)
                shape.Draw(renderMy);
        }
    }
}
