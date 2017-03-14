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
        #region Enums

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

        #endregion

        #region Variables
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
                SyncTranslate();
                _currentRenderMode = value;
                UpdateScale();
                Render();
                OnPropertyChanged();
            }
        }

        private List<Shape> ShapesList { get; set; } = new List<Shape>();
        public IRenderer Renderer { get; set; }
        private Color _selectedFillColor = Color.FromRgb(255, 255, 255);
        private Color _selectedBorderColor = Color.FromRgb(0, 0, 0);
        private double _scale = 1;
        private float _borderWidth = 1;

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

        private double minScale = 0.1;
        private double maxScale = 5;
        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value > maxScale) value = maxScale;
                else if (value < minScale) value = minScale;
                if (Scale == value) return;
                _scale = value;
                OnPropertyChanged();
                UpdateScale();
            }
        }
        List<Point> currentPoints = new List<Point>();
        private Mode _canvasMode;
        private Shape _dynamicShape;
        private RenderMode _currentRenderMode;

        private KeyValuePair<int, int> lastTransformPoint;
        #endregion

        #region Constructor

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

        #endregion

        #region Shapes control
        public void AddShape(ShapeTypes.ShapeType shape, IEnumerable<Point> points)
        {
            try
            {
                var shapeMode = ShapeModes.ShapeMode.Fixed;
                var newShape = ShapeFabric.CreateShape(shape, points, shapeMode);
                var newShapeDrawable = newShape as IDrawable2DShape;

                if (newShapeDrawable != null)
                {
                    newShapeDrawable.BorderWidth = BorderWidth;
                    newShapeDrawable.BorderColor = SelectedBorderColor;
                    newShapeDrawable.FillColor = (newShape is Line) ? SelectedBorderColor : SelectedFillColor;
                }

                ShapesList.Add(newShape);
            }
            catch (Exception ex)
            {
                OnExceptionRaised(ex);
            }
            Render();
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
        #endregion

        #region Render control

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

        private void UpdateScale()
        {
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow?.Scale(1 / Scale);
                    break;
                case RenderMode.WPF:
                    wpfWindow?.Scale(1 / Scale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Render();
        }

        private void UpdateTranslate(int deltaX, int deltaY)
        {
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow?.TranslateI(deltaX, deltaY);
                    break;
                case RenderMode.WPF:
                    wpfWindow?.TranslateI(deltaX, deltaY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Render();
        }

        private void SyncTranslate()
        {
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    wpfWindow?.SetTranslateD(oglWindow.CenterX, oglWindow.CenterY);
                    break;
                case RenderMode.WPF:
                    oglWindow?.SetTranslateD(wpfWindow.CenterX, wpfWindow.CenterY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        #endregion

        #region Canvas events

        public void CanvasMouseDown(int inX, int inY, MouseButton changedButton)
        {
            switch (changedButton)
            {
                case MouseButton.Left:
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
                                AddShape(ShapeTypes.ShapeType.Quadrangle_, currentPoints);
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
                    break;
                case MouseButton.Right:
                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                    break;
            }
        }

        public void CanvasMouseMove(int inX, int inY, bool lmbPressed, bool rmbPressed)
        {
            if (!lmbPressed && !rmbPressed)
            {
                var currentPoint = GetOrthoPoint(inX, inY);
                var shapeMode = ShapeModes.ShapeMode.NotFixed;

                try
                {
                    var enumerable = currentPoints.Concat(new List<Point> { currentPoint });
                    if (CanvasMode == Mode.DrawLine && currentPoints.Count > 1)
                        DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Line_, enumerable, shapeMode);
                    else
                        switch (currentPoints.Count)
                        {
                            case 1:
                                DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Line_, enumerable, shapeMode);
                                break;
                            case 2:
                                if (CanvasMode == Mode.DrawEllipse)
                                    DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Ellipse_, enumerable,
                                        shapeMode);
                                else
                                    DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Triangle_, enumerable,
                                        shapeMode);
                                break;
                            case 3:
                                DynamicShape = ShapeFabric.CreateShape(ShapeTypes.ShapeType.Quadrangle_, enumerable,
                                    shapeMode);
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
            else if (rmbPressed && !lmbPressed)
            {
                UpdateTranslate(-inX + lastTransformPoint.Key, inY - lastTransformPoint.Value);
                lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
            }
        }

        public void CanvasMouseWheel(int inX, int inY, int delta)
        {
            Scale += delta * Scale / 800.0;
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

        #endregion

        #region Event handlers

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public delegate void StringEventHandler(object sender, StringEventArgs args);
        public event StringEventHandler ExceptionRaised;
        public class StringEventArgs : EventArgs
        {
            public StringEventArgs(string str)
            {
                Str = str;
            }

            public string Str { get; }
        }

        protected void OnExceptionRaised(Exception ex)
        {
            ExceptionRaised?.Invoke(this, new StringEventArgs(ex.Message));
        }

        #endregion
    }
}