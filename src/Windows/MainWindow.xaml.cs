using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ShapeEditor.Domain;
using ShapeEditor.src;

namespace ShapeEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables
        public Color SelectedFillColor
        {
            get { return _selectedFillColor; }
            private set
            {
                _selectedFillColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedFillColorBrush));
            }
        }
        public Color SelectedBorderColor
        {
            get { return _selectedBorderColor; }
            private set
            {
                _selectedBorderColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedBorderColorBrush));
            }
        }
        public Brush SelectedFillColorBrush => new SolidColorBrush(SelectedFillColor);
        public Brush SelectedBorderColorBrush => new SolidColorBrush(SelectedBorderColor);

        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value == _scale)
                    return;
                _scale = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScalePercent));
            }
        }
        public string ScalePercent => $"{(int)(Scale*100)}%";
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Init
        public MainWindow()
        {
            InitializeColorDialog();
            InitializeComponent();
            //Show dialog host
            dialogHost.HorizontalAlignment = HorizontalAlignment.Stretch;
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
        private Color _selectedFillColor = Color.FromRgb(0, 0, 0);
        private Color _selectedBorderColor = Color.FromRgb(0, 0, 0);
        private double _scale = 1;
        private int _borderWidth;

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
