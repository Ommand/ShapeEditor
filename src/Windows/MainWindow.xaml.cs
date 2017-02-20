﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using ShapeEditor.Domain;
using ShapeEditor.Renderers;
using ShapeEditor.src;
using ShapeEditor.Shapes;
using ShapeEditor.Fabrics;
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
        
        List<Shape> listShapes = new List<Shape> { };
        IRenderer renderer;
        private OpenGLControl control;
        #endregion

        #region Main
        public MainWindow()
        {
            InitializeColorDialog();
            InitializeComponent();
            
            control = new OpenGLControl();
            grdMain.Children.Add(control);
            renderer = new RendererOpenGl(control);

            //example of using shape fabric
            List<ShapeFabric> fabricsList = new List<ShapeFabric> { new TriangleFabric(), new TriangleFabric() }; 
            foreach(ShapeFabric fabric in fabricsList)
            {
                fabric.useBorderColor(SelectedBorderColor);
                fabric.useBorderWidth(1);
                fabric.useFillColor(SelectedFillColor);
                Shape shape = fabric.createShape();
                listShapes.Add(shape);
            }
           // listShapes = new List<Shape> { new Triangle(new Point(-0.5, -0.5), new Point(0.7, -0.5), new Point(0.5, 0.5), SelectedBorderColor, SelectedFillColor) };
            // renderer = new RendererWpf(WpfRender);
            WpfRender.Visibility = Visibility.Hidden;
            control.Visibility = Visibility.Visible;

            //Show dialog host
            dialogHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            grdMain.Children.Remove(dialogHost);
            grdMain.Children.Add(dialogHost);
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
        private Color _selectedFillColor = Color.FromRgb(255, 255, 255);
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
