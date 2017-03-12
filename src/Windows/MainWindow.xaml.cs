using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ShapeEditor.Renderers;
using ShapeEditor.src;
using ShapeEditor.Utils;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace ShapeEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables

        GraphicsController _graphics = new GraphicsController();

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

        public double BorderWidth
        {
            get { return _graphics.BorderWidth; }
            set
            {
                _graphics.BorderWidth = value;
                OnPropertyChanged();
            }
        }

//        private OpenGLControl control;

        #endregion

       #region Main

        public MainWindow()
        {
            InitializeColorDialog();
            InitializeComponent();

           //////переключение между окнами в рендере
           WpfRender.Visibility = Visibility.Hidden;
 //           control.Visibility = Visibility.Visible;

            //Show dialog host
            dialogHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            grdMain.Children.Remove(dialogHost);
            grdMain.Children.Add(dialogHost);
        }

        #endregion

        #region UI
        private void ButtonAddShape_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnTriangle)
                _graphics.CanvasMode = GraphicsController.Mode.DrawTriangle;
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
