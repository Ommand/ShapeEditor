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
            ShapeSelected,
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
                if (SelectedShape != null)
                {
                    SelectedShape = null;
                    Mouse.OverrideCursor = null;
                    BoundingBox = null;
                    Render();
                }
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

        public List<Shape> ShapesList
        {
            get { return _shapesList; }
            set
            {
                if (value == null) return;

                SelectShape();
                _shapesList.Clear();
                _shapesList = value;
            }
        }

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

                UpdateSelectedShapeColors();

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

                UpdateSelectedShapeColors();

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

                UpdateSelectedShapeColors();

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
        private Shape SelectedShape { get; set; }
        private SelectRectangle BoundingBox { get; set; }
        private RenderMode _currentRenderMode;

        private KeyValuePair<int, int> lastTransformPoint;
        private ShapeExpandController expandController;
        private List<Shape> _shapesList = new List<Shape>();

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

        public void MoveSelectedShapeToTop()
        {
            if (SelectedShape == null) return;

            ShapesList.Remove(SelectedShape);
            ShapesList.Add(SelectedShape);
            Render();
        }

        public void DeleteSelectedShape()
        {
            if (SelectedShape != null)
                ShapesList.Remove(SelectedShape);
            SelectShape();
        }

        private void SelectShape(Shape s = null)
        {
            if (s == SelectedShape)
                return;

            if (s == null)
            {
                CanvasMode = Mode.None;
                BoundingBox = null;
                Mouse.OverrideCursor = null;
            }
            else
            {
                CanvasMode = Mode.ShapeSelected;

                var dShape = s as IDrawable2DShape;

                SelectedFillColor = dShape.FillColor;
                SelectedBorderColor = dShape.BorderColor;
                BorderWidth = dShape.BorderWidth;

                UpdateBoundingBox(s);
            }

            SelectedShape = s;
            Render();
        }

        private void UpdateBoundingBox(Shape s)
        {
            if (s == null)
                return;

            var formSelection = s.FormSelection().ToList();
            BoundingBox = new SelectRectangle(formSelection[0], formSelection[1]);
        }

        private void UpdateSelectedShapeColors()
        {
            var changed = false;
            var drawable2DShape = SelectedShape as IDrawable2DShape;
            if (drawable2DShape == null) return;
            if (drawable2DShape.FillColor != SelectedFillColor)
            {
                drawable2DShape.FillColor = SelectedFillColor;
                changed = true;
            }
            if (drawable2DShape.BorderColor != SelectedBorderColor)
            {
                drawable2DShape.BorderColor = SelectedBorderColor;
                changed = true;
            }
            if (drawable2DShape.BorderWidth != BorderWidth)
            {
                drawable2DShape.BorderWidth = BorderWidth;
                changed = true;
            }

            if (changed) Render();
        }

        #endregion

        #region Render control

        public void Render()
        {
            var drawable2DShapes = ShapesList.Select(x => x as IDrawable2DShape).ToList();
            var drawable2DShape = DynamicShape as IDrawable2DShape;
            if (drawable2DShape != null)
                drawable2DShapes.Add(drawable2DShape);

            //Selection box processing
            if (BoundingBox != null)
                drawable2DShapes.Add(BoundingBox);

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

        public Point GetOrthoPoint(int inX, int inY)
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

        public Point GetPointOrtho(double inX, double inY)
        {
            int X, Y;
            switch (CurrentRenderMode)
            {
                case RenderMode.OpenGL:
                    oglWindow.GetWindowValue(inX, inY, out X, out Y);
                    break;
                case RenderMode.WPF:
                    wpfWindow.GetWindowValue(inX, inY, out X, out Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Point(X, Y);
        }
        #endregion

        #region Canvas events

        public void CanvasMouseDown(int inX, int inY, MouseButton button)
        {
            var orthoPoint = GetOrthoPoint(inX, inY);
            switch (button)
            {
                case MouseButton.Left:
                    if (CanvasMode >= Mode.DrawTriangle && CanvasMode <= Mode.DrawLine)
                    {
                        currentPoints.Add(orthoPoint);
                    }

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
                        case Mode.None:
                            SelectShape();
                            foreach (var shape in Enumerable.Reverse(ShapesList))
                                if (shape.IsInside(orthoPoint))
                                {
                                    SelectShape(shape);
                                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                                    break;
                                }
                            break;
                        case Mode.ShapeSelected:
                            if (BoundingBox.IsOnBoundary(orthoPoint) != PointPlaces.PointPlace.None)
                            {
                                var formSelection = SelectedShape.FormSelection().ToList();
                                expandController = new ShapeExpandController(new KeyValuePair<int, int>(inX, inY), BoundingBox.IsOnBoundary(orthoPoint), new Point(formSelection[0].X, formSelection[1].Y), new Point(formSelection[1].X, formSelection[0].Y), GetOrthoPoint);
                            }
                            else
                                goto case Mode.None;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case MouseButton.Right:
                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                    break;
                case MouseButton.Middle:
                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                    break;
            }
        }

        public void CanvasMouseMove(int inX, int inY, bool lmbPressed, bool rmbPressed, bool mmbPressed)
        {
            if (!lmbPressed && !rmbPressed && !mmbPressed)
            {
                var currentPoint = GetOrthoPoint(inX, inY);
                var shapeMode = ShapeModes.ShapeMode.NotFixed;

                if (currentPoints.Count > 0)
                {
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
                else if (CanvasMode == Mode.ShapeSelected)
                {
                    switch (BoundingBox.IsOnBoundary(GetOrthoPoint(inX, inY)))
                    {
                        case PointPlaces.PointPlace.LeftUpCorner:
                            Mouse.OverrideCursor = Cursors.SizeNWSE;
                            break;
                        case PointPlaces.PointPlace.UpEdge:
                            Mouse.OverrideCursor = Cursors.SizeNS;
                            break;
                        case PointPlaces.PointPlace.RightUpCorner:
                            Mouse.OverrideCursor = Cursors.SizeNESW;
                            break;
                        case PointPlaces.PointPlace.RightEdge:
                            Mouse.OverrideCursor = Cursors.SizeWE;
                            break;
                        case PointPlaces.PointPlace.RightLowCorner:
                            Mouse.OverrideCursor = Cursors.SizeNWSE;
                            break;
                        case PointPlaces.PointPlace.LowEdge:
                            Mouse.OverrideCursor = Cursors.SizeNS;
                            break;
                        case PointPlaces.PointPlace.LeftLowCorner:
                            Mouse.OverrideCursor = Cursors.SizeNESW;
                            break;
                        case PointPlaces.PointPlace.LeftEdge:
                            Mouse.OverrideCursor = Cursors.SizeWE;
                            break;
                        case PointPlaces.PointPlace.None:
                            Mouse.OverrideCursor = null;
                            break;
                    }
                }
            }
            else if (rmbPressed && !lmbPressed && !mmbPressed)
            {
                UpdateTranslate(-inX + lastTransformPoint.Key, inY - lastTransformPoint.Value);
                lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
            }
            else if (lmbPressed && !rmbPressed && !mmbPressed)
            {
                if (expandController != null)
                {
                    var calculatedExpand = expandController.CalculateExpand(new KeyValuePair<int, int>(inX, inY));
                    if (calculatedExpand != null)
                    {
                        SelectedShape.ApplyTransformation(calculatedExpand);
                        UpdateBoundingBox(SelectedShape);
                    }
                }
                else if (CanvasMode == Mode.ShapeSelected)
                {
                    var last = GetOrthoPoint(lastTransformPoint.Key, lastTransformPoint.Value);
                    var current = GetOrthoPoint(inX, inY);
                    SelectedShape.ApplyTransformation(new Translate(new Point(current.X - last.X, current.Y - last.Y)));
                    UpdateBoundingBox(SelectedShape);

                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                }
                Render();
            }
            else if (!lmbPressed && !rmbPressed && mmbPressed)
            {
                if (CanvasMode == Mode.ShapeSelected)
                {
                    var last = GetOrthoPoint(lastTransformPoint.Key, lastTransformPoint.Value);
                    var current = GetOrthoPoint(inX, inY);

                    SelectedShape.ApplyTransformation(new Rotate(SelectedShape.GetCenter(), -(current.X - last.X) * 2.0f));
                    UpdateBoundingBox(SelectedShape);
                    Render();

                    lastTransformPoint = new KeyValuePair<int, int>(inX, inY);
                }
            }
        }

        public void CanvasMouseUp(int inX, int inY, MouseButton button)
        {
            expandController = null;
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

    public class ShapeExpandController
    {
        private readonly PointPlaces.PointPlace mode;
        private readonly Point leftBottom;
        private readonly Point rightTop;
        private KeyValuePair<int, int> lastTransformPoint;
        private Func<int, int, Point> Transform;

        public Expand CalculateExpand(KeyValuePair<int, int> newPoint)
        {
            Expand result = null;
            int deltaX = lastTransformPoint.Key - newPoint.Key;
            int deltaY = lastTransformPoint.Value - newPoint.Value;
            var currentPoint = Transform(newPoint.Key, newPoint.Value);
            var prevPoint = Transform(lastTransformPoint.Key, lastTransformPoint.Value);
            Point center = new Point(0, 0);
            double kX = double.NaN, kY = double.NaN;

            switch (mode)
            {
                case PointPlaces.PointPlace.LeftUpCorner:
                    center = new Point(rightTop.X, leftBottom.Y);
                    break;
                case PointPlaces.PointPlace.RightUpCorner:
                    center = leftBottom;
                    break;
                case PointPlaces.PointPlace.LowEdge:
                    kX = 1;
                    center = rightTop;
                    break;
                case PointPlaces.PointPlace.UpEdge:
                    kX = 1;
                    center = leftBottom;
                    break;
                case PointPlaces.PointPlace.LeftLowCorner:
                    center = rightTop;
                    break;
                case PointPlaces.PointPlace.RightLowCorner:
                    center = new Point(leftBottom.X, rightTop.Y);
                    break;
                case PointPlaces.PointPlace.None:
                    break;
                case PointPlaces.PointPlace.LeftEdge:
                    kY = 1;
                    center = rightTop;
                    break;
                case PointPlaces.PointPlace.RightEdge:
                    kY = 1;
                    center = leftBottom;
                    break;
            }
            kX = double.IsNaN(kX) ? (currentPoint.X - center.X) / (prevPoint.X - center.X) : kX;
            kY = double.IsNaN(kY) ? (currentPoint.Y - center.Y) / (prevPoint.Y - center.Y) : kY;

            result = new Expand(center, kX, kY);
            lastTransformPoint = newPoint;
            return result;
        }

        public ShapeExpandController(KeyValuePair<int, int> p, PointPlaces.PointPlace mode, Point leftBottom, Point rightTop, Func<int, int, Point> transform)
        {
            lastTransformPoint = p;
            this.mode = mode;
            this.leftBottom = leftBottom;
            this.rightTop = rightTop;
            Transform = transform;
        }

    }
}