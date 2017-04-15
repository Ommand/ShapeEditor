using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ShapeEditor.Renderers;
using ShapeEditor.src;
using ShapeEditor.src.IO;
using ShapeEditor.Utils;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ShapeEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables

        GraphicsController _graphics = new GraphicsController();
        Dictionary<Button, GraphicsController.Mode> shapeButtons;

        public Color SelectedFillColor
        {
            get { return _graphics.SelectedFillColor; }
            private set
            {
                _graphics.SelectedFillColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedFillColorBrush));
            }
        }

        public Color SelectedBorderColor
        {
            get { return _graphics.SelectedBorderColor; }
            private set
            {
                _graphics.SelectedBorderColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedBorderColorBrush));
            }
        }

        public Brush SelectedFillColorBrush => new SolidColorBrush(SelectedFillColor);
        public Brush SelectedBorderColorBrush => new SolidColorBrush(SelectedBorderColor);

        public double Scale
        {
            get { return _graphics.Scale; }
            set
            {
                if (value == _graphics.Scale)
                    return;
                _graphics.Scale = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScalePercent));
            }
        }

        public string ScalePercent => $"{(int)(Scale * 100)}%";

        public float BorderWidth
        {
            get { return _graphics.BorderWidth; }
            set
            {
                _graphics.BorderWidth = value;
                OnPropertyChanged();
            }
        }

        public bool IsWpfMode
        {
            get { return _graphics.CurrentRenderMode == GraphicsController.RenderMode.WPF; }
            set
            {
                if (value == (_graphics.CurrentRenderMode == GraphicsController.RenderMode.WPF)) return;

                _graphics.CurrentRenderMode = value
                    ? GraphicsController.RenderMode.WPF
                    : GraphicsController.RenderMode.OpenGL;
                SwitchRenderer();
                OnPropertyChanged();
            }
        }

        private Brush secondaryAccentBrush;
        private Brush primaryHueMidBrush;

        #endregion

        #region Main

        public MainWindow()
        {
            InitializeColorDialog();
            InitializeComponent();
            IsWpfMode = true;

            //Show dialog host
            dialogHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            grdMain.Children.Remove(dialogHost);
            grdMain.Children.Add(dialogHost);

            //Assign brushes
            secondaryAccentBrush = (Brush)FindResource("SecondaryAccentBrush");
            primaryHueMidBrush = (Brush)FindResource("PrimaryHueMidBrush");

            //Initialize button dictionary
            shapeButtons = new Dictionary<Button, GraphicsController.Mode>
            {
                {btnTriangle, GraphicsController.Mode.DrawTriangle},
                {btnEllipse, GraphicsController.Mode.DrawEllipse},
                {btnLine, GraphicsController.Mode.DrawLine},
                {btnQuadrilateral, GraphicsController.Mode.DrawRect}
            };

            //Assign button style to mode change
            _graphics.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_graphics.CanvasMode)))
                {
                    foreach (var it in shapeButtons.Keys)
                        it.Background = primaryHueMidBrush;

                    if (_graphics.CanvasMode > (GraphicsController.Mode)1 &&
                        _graphics.CanvasMode <= (GraphicsController.Mode)5)
                        shapeButtons.First(x => x.Value == _graphics.CanvasMode).Key.Background = secondaryAccentBrush;
                }
                else if (args.PropertyName.Equals(nameof(_graphics.Scale)))
                {
                    OnPropertyChanged(nameof(Scale));
                    OnPropertyChanged(nameof(ScalePercent));
                }
                else if (args.PropertyName.Equals(nameof(_graphics.SelectedFillColor))
                         || args.PropertyName.Equals(nameof(_graphics.SelectedBorderColor))
                         || args.PropertyName.Equals(nameof(_graphics.BorderWidth)))
                {
                    OnPropertyChanged(args.PropertyName);
                    OnPropertyChanged($"{args.PropertyName}Brush");
                }
            };

            //Настройка GraphicsController
            _graphics.oglWindow = OpenGLRender;
            _graphics.wpfWindow = WpfRender;
            _graphics.CurrentRenderMode = GraphicsController.RenderMode.WPF;
            _graphics.ExceptionRaised += (sender, args) => HandleException(args.Str);
        }

        private void HandleException(string msg)
        {
            Snackbar.MessageQueue.Enqueue(msg);
        }

        #endregion

        #region UI

        private void ButtonAddShape_Click(object sender, RoutedEventArgs e)
        {
            var mode = shapeButtons[(Button)sender];
            _graphics.CanvasMode = _graphics.CanvasMode == mode ? GraphicsController.Mode.None : mode;
        }

        private void OpenGLRender_OnMouseDown(object sender, MouseEventArgs e)
        {
            _graphics.CanvasMouseDown(e.X, e.Y, e.Button == MouseButtons.Left ? MouseButton.Left : MouseButton.Right);
        }

        private void OpenGLRender_OnMouseUp(object sender, MouseEventArgs e)
        {
            _graphics.CanvasMouseUp(e.X, e.Y, e.Button == MouseButtons.Left ? MouseButton.Left : MouseButton.Right);
        }

        private void WpfRender_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(this);
            _graphics.CanvasMouseDown((int)pt.X, (int)pt.Y, e.ChangedButton);
        }

        private void WpfRender_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(this);
            _graphics.CanvasMouseUp((int)pt.X, (int)pt.Y, e.ChangedButton);
        }

        private void WpfRender_OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pt = e.GetPosition(this);
            _graphics.CanvasMouseMove((int)pt.X, (int)pt.Y, e.LeftButton == MouseButtonState.Pressed,
                e.RightButton == MouseButtonState.Pressed, e.MiddleButton == MouseButtonState.Pressed);
        }

        private void OpenGLRender_OnMouseMove(object sender, MouseEventArgs e)
        {
            _graphics.CanvasMouseMove(e.X, e.Y, (e.Button & MouseButtons.Left) != 0,
                (e.Button & MouseButtons.Right) != 0, (e.Button & MouseButtons.Middle) != 0);
        }

        private void SwitchRenderer()
        {
            //переключение между окнами в рендере
            HostOpenGL.Visibility = _graphics.CurrentRenderMode == GraphicsController.RenderMode.OpenGL
                ? Visibility.Visible
                : Visibility.Collapsed;
            WpfRender.Visibility = _graphics.CurrentRenderMode == GraphicsController.RenderMode.WPF
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void WpfRender_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pt = e.GetPosition(this);
            _graphics.CanvasMouseWheel((int)pt.X, (int)pt.Y, e.Delta);
        }

        private void OpenGLRender_OnMouseWheel(object sender, MouseEventArgs e)
        {
            _graphics.CanvasMouseWheel(e.X, e.Y, e.Delta);
        }

        private void ButtonMoveShapeToTopClick(object sender, RoutedEventArgs e)
        {
            _graphics.MoveSelectedShapeToTop();
        }

        private void ButtonDeleteShapeClick(object sender, RoutedEventArgs e)
        {
            _graphics.DeleteSelectedShape();
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            _graphics.CanvasMode = GraphicsController.Mode.None;
            var sfd = new Microsoft.Win32.SaveFileDialog { Filter = "JSON file (*.json)|*.json|SVG file (*.svg)|*.svg" };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    new IOProxy(_graphics).saveShapes(_graphics.ShapesList, sfd.FileName);
                    HandleException("Save completed");
                }
                catch (Exception ex)
                {
                    HandleException($"Save failed: {ex.Message}");
                }
            }
        }

        private void ButtonLoadClick(object sender, RoutedEventArgs e)
        {
            _graphics.CanvasMode = GraphicsController.Mode.None;
            var ofd = new Microsoft.Win32.OpenFileDialog { Filter = "Supported file formats (*.json;*.svg)|*.json;*.svg" };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    var graphicsShapesList = new IOProxy(_graphics).loadShapes(ofd.FileName);
                    if (graphicsShapesList == null)
                        HandleException("Load failed");
                    else
                    {
                        _graphics.ShapesList = graphicsShapesList;
                        HandleException("Load completed");
                        _graphics.Render();
                    }
                }
                catch (Exception ex)
                {
                    HandleException($"Load failed: {ex.Message}");
                }
            }
        }

        #endregion

        #region ColorDialog

        private void InitializeColorDialog()
        {
            //Set color picker command
            OpenColorPickerCommand = new CommandImplementation(OpenColorPickerDialog);
            //Initialize colorPicker dialog
            ColorPicker.Init(this);
        }

        public ICommand OpenColorPickerCommand
        {
            get { return _openColorPickerCommand; }
            private set
            {
                _openColorPickerCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand CancelColorPickerCommand
        {
            get { return _cancelColorPickerCommand; }
            private set
            {
                _cancelColorPickerCommand = value;
                OnPropertyChanged();
            }
        }

        private bool _isColorPickerOpen;
        private object _colorPickerContent;
        private ICommand _openColorPickerCommand;
        private ICommand _cancelColorPickerCommand;
        private bool _isWpfMode = true;

        public bool IsColorPickerOpen
        {
            get { return _isColorPickerOpen; }
            set
            {
                if (_isColorPickerOpen == value) return;
                _isColorPickerOpen = value;
                OnPropertyChanged();
            }
        }

        public object ColorPickerContent
        {
            get { return _colorPickerContent; }
            set
            {
                if (_colorPickerContent == value) return;
                _colorPickerContent = value;
                OnPropertyChanged();
            }
        }

        private void OpenColorPickerDialog(object obj)
        {
            ColorPickerContent = ColorPicker.Instance;
            IsColorPickerOpen = true;
        }

        private void OnSelectColorPickerFill(object obj)
        {
            IsColorPickerOpen = false;
            SelectedFillColor = ColorPicker.SelectedColor;
        }

        private void OnSelectColorPickerBorder(object obj)
        {
            IsColorPickerOpen = false;
            SelectedBorderColor = ColorPicker.SelectedColor;
        }

        private void btnFillColorClick(object sender, RoutedEventArgs e)
        {
            CancelColorPickerCommand = new CommandImplementation(OnSelectColorPickerFill);
        }

        private void btnBorderColorClick(object sender, RoutedEventArgs e)
        {
            CancelColorPickerCommand = new CommandImplementation(OnSelectColorPickerBorder);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            _graphics.KeyPressed(e.Key);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
