using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ShapeEditor.Annotations;
using ShapeEditor.Fabrics;
using ShapeEditor.src.RenderWindows;
using ShapeEditor.Shapes;

namespace ShapeEditor.Utils
{
    public class GraphicsController : INotifyPropertyChanged
    {
        public enum Mode
        {
            None,
            DrawTriangle,
            DrawRect,
            DrawEllipse,
            DrawLine
        }

        public enum RenderMode
        {
            OpenGL, WPF
        }

        public Mode CanvasMode
        {
            get { return _canvasMode; }
            set
            {
                _canvasMode = value;
                currentPoints.Clear();
                OnPropertyChanged();
            }
        }

        public RenderMode CurrentRenderMode { get; set; }

        public List<IShape> ShapesList { get; set; } = new List<IShape>();
        public IRenderer Renderer { get; set; }
        public Color SelectedFillColor = Color.FromRgb(255, 255, 255);
        public Color SelectedBorderColor = Color.FromRgb(0, 0, 0);
        public double Scale = 1;
        public double BorderWidth = 1;

        public OpenGLWindow oglWindow { get; set; }
        public WpfWindow wpfWindow { get; set; }

        public void AddShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points)
        {
            var newShape = ShapeFabric.CreateShape(shape, points);
            var newShapeDrawable = newShape as IDrawable2DShape;

            if (newShapeDrawable != null)
            {
                newShapeDrawable.BorderWidth = BorderWidth;
                newShapeDrawable.BorderColor = SelectedBorderColor;
                newShapeDrawable.FillColor = SelectedFillColor;
            }

            ShapesList.Add(newShape);
            Render();
        }

        public void Render()
        {
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow.Shapes = ShapesList.Select(x => x as IDrawable2DShape).ToList();
                    oglWindow.Invalidate();
                    break;
                case RenderMode.WPF:
                    wpfWindow.Shapes = ShapesList.Select(x => x as IDrawable2DShape).ToList();
                    wpfWindow.InvalidateVisual();
                    break;
            }
        }

        List<Point> currentPoints = new List<Point>();
        private Mode _canvasMode;

        public void CanvasMouseDown(int inX,int inY)
        {
            Random r = new Random();

            double y = 0;
            double x = 0;
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow.GetOrthoValue(inX, inY, out x, out y);
                    break;
                case RenderMode.WPF:
                    wpfWindow.GetOrthoValue(inX, inY, out x, out y);
                    break;
            }
            currentPoints.Add(new Point(x, y));

            switch (CanvasMode)
            {
                case Mode.DrawTriangle:
                    if (currentPoints.Count == 3)
                    {
                        AddShape(ShapeTypes.ShapeType.Triangle_, currentPoints);
                        CanvasMode = Mode.None;
                    }
                    else
                    {
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}