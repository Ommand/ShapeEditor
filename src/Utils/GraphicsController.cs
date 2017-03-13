using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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

        public GraphicsController()
        {
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("DynamicShape"))
                    Render();
                //                else if (args.PropertyName.Equals("SelectedBorderColor"))
                //                {
                //                    var dyn = DynamicShape as IDrawable2DShape;
                //                    if (dyn != null)
                //                        dyn.BorderColor = SelectedBorderColor;
                //                    OnPropertyChanged(nameof(DynamicShape));
                //                }
                //                else if (args.PropertyName.Equals("SelectedFillColor"))
                //                {
                //                    var dyn = DynamicShape as IDrawable2DShape;
                //                    if (dyn != null)
                //                        dyn.FillColor = SelectedFillColor;
                //                    OnPropertyChanged(nameof(DynamicShape));
                //                }
                //                else if (args.PropertyName.Equals("BorderWidth"))
                //                {
                //                    var dyn = DynamicShape as IDrawable2DShape;
                //                    if (dyn != null)
                //                        dyn.BorderWidth = BorderWidth;
                //                    OnPropertyChanged(nameof(DynamicShape));
                //                }
            };
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
                DynamicShape = null;
                OnPropertyChanged();
            }
        }

        public RenderMode CurrentRenderMode
        {
            get { return _currentRenderMode; }
            set
            {
                if (value == _currentRenderMode) return;
                _currentRenderMode = value;
                Render();
                OnPropertyChanged();
            }
        }

        private List<Shape> ShapesList { get; set; } = new List<Shape>();
        public IRenderer Renderer { get; set; }
        public Color _selectedFillColor = Color.FromRgb(255, 255, 255);
        public Color _selectedBorderColor = Color.FromRgb(0, 0, 0);
        public double _scale = 1;
        public float _borderWidth = 1;

        public OpenGLWindow oglWindow { get; set; }
        public WpfWindow wpfWindow { get; set; }

        private Shape DynamicShape
        {
            get { return _dynamicShape; }
            set
            {
                if (value == _dynamicShape) return;
                _dynamicShape = value;
                UpdateDynamicShape();
                OnPropertyChanged();
            }
        }

        public Color SelectedBorderColor
        {
            get { return _selectedBorderColor; }
            set
            {
                if (value.Equals(_selectedBorderColor)) return;
                _selectedBorderColor = value;
                OnPropertyChanged();
            }
        }

        public Color SelectedFillColor
        {
            get { return _selectedFillColor; }
            set
            {
                if (value.Equals(_selectedFillColor)) return;
                _selectedFillColor = value;
                OnPropertyChanged();
            }
        }

        public float BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                if (value.Equals(_borderWidth)) return;
                _borderWidth = value;
                OnPropertyChanged();
            }
        }

        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public void AddShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points)
        {
            var newShape = ShapeFabric.CreateShape(shape, points);
            var newShapeDrawable = newShape as IDrawable2DShape;

            if (newShapeDrawable != null)
            {
                newShapeDrawable.BorderWidth = BorderWidth;
                newShapeDrawable.BorderColor = SelectedBorderColor;
                newShapeDrawable.FillColor = (newShape is Line) ? SelectedBorderColor : SelectedFillColor;
            }

            ShapesList.Add(newShape);
            Render();
        }

        public void Render()
        {
            var drawable2DShapes = ShapesList.Select(x => x as IDrawable2DShape).ToList();
            var drawable2DShape = DynamicShape as IDrawable2DShape;
            if (drawable2DShape != null)
                drawable2DShapes.Add(drawable2DShape);
            //            if (drawable2DShapes.Count <= 0) return;
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    if (oglWindow != null)
                    {
                        oglWindow.Shapes = drawable2DShapes;
                        oglWindow.Invalidate();
                    }
                    break;
                case RenderMode.WPF:
                    if (wpfWindow != null)
                    {
                        wpfWindow.Shapes = drawable2DShapes;
                        wpfWindow.InvalidateVisual();
                    }
                    break;
            }
        }

        List<Point> currentPoints = new List<Point>();
        private Mode _canvasMode;
        private Shape _dynamicShape;
        private RenderMode _currentRenderMode;

        public void CanvasMouseDown(int inX, int inY)
        {
            if (CanvasMode == Mode.None)
                return;

            currentPoints.Add(GetOrthoPoint(inX, inY));

            switch (CanvasMode)
            {
                case Mode.DrawTriangle:
                    if (currentPoints.Count == 3)
                    {
                        AddShape(ShapeTypes.ShapeType.Triangle_, currentPoints);
                        CanvasMode = Mode.None;
                    }
                    break;
                case Mode.DrawRect:
                    if (currentPoints.Count == 4)
                    {
                        AddShape(ShapeTypes.ShapeType.Rectangle_, currentPoints);
                        CanvasMode = Mode.None;
                    }
                    break;
                case Mode.DrawEllipse:
                    if (currentPoints.Count == 3)
                    {
                        AddShape(ShapeTypes.ShapeType.Ellipse_, currentPoints);
                        CanvasMode = Mode.None;
                    }
                    break;
                case Mode.DrawLine:
                    break;
            }
        }

        private Point GetOrthoPoint(int inX, int inY)
        {
            double x;
            double y;
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow.GetOrthoValue(inX, inY, out x, out y);
                    break;
                case RenderMode.WPF:
                    wpfWindow.GetOrthoValue(inX, inY, out x, out y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Point(x, y);
        }

        public void CanvasMouseMove(int inX, int inY)
        {
            var currentPoint = GetOrthoPoint(inX, inY);

            try
            {
                var enumerable = currentPoints.Concat(new List<Point> { currentPoint });
                if (CanvasMode == Mode.DrawLine && currentPoints.Count > 1)
                    DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Line_, enumerable);
                else
                    switch (currentPoints.Count)
                    {
                        case 1:
                            DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Line_, enumerable);
                            break;
                        case 2:
                            if (CanvasMode == Mode.DrawEllipse)
                                DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Ellipse_, enumerable);
                            else
                                DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Triangle_, enumerable);
                            break;
                        case 3:
                            DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Rectangle_, enumerable);
                            break;
                        default:
                            DynamicShape = null;
                            break;
                    }
            }
            catch
            {
                // ignored
            }
        }

        public void KeyPressed(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    if (CanvasMode == Mode.DrawLine && currentPoints.Count > 1)
                        AddShape(ShapeTypes.ShapeType.Line_, currentPoints);
                    CanvasMode = Mode.None;
                    break;
            }
        }

        private void UpdateDynamicShape()
        {
            var dyn = DynamicShape as IDrawable2DShape;
            if (dyn != null)
            {
                dyn.BorderColor = SelectedBorderColor;
                dyn.FillColor = DynamicShape is Line ? SelectedBorderColor : SelectedFillColor;
                dyn.BorderWidth = BorderWidth;
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