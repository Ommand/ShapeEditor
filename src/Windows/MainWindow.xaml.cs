using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        public MainWindow()
        {
            OpenSample4DialogCommand = new AnotherCommandImplementation(OpenSample4Dialog);
            AcceptSample4DialogCommand = new AnotherCommandImplementation(AcceptSample4Dialog);
            CancelSample4DialogCommand = new AnotherCommandImplementation(CancelSample4Dialog);
            InitializeComponent();
            ColorPicker.Init(this);
            dialogHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            //Sample 4
        }
        private string _name;
        
        #region SAMPLE 4

        //pretty much ignore all the stuff provided, and manage everything via custom commands and a binding for .IsOpen
        public ICommand OpenSample4DialogCommand { get; }
        public ICommand AcceptSample4DialogCommand { get; }
        public ICommand CancelSample4DialogCommand { get; }

        private bool _isSample4DialogOpen;
        private object _sample4Content;

        public bool IsSample4DialogOpen
        {
            get { return _isSample4DialogOpen; }
            set
            {
                if (_isSample4DialogOpen == value) return;
                _isSample4DialogOpen = value;
                OnPropertyChanged();
            }
        }

        public object Sample4Content
        {
            get { return _sample4Content; }
            set
            {
                if (_sample4Content == value) return;
                _sample4Content = value;
                OnPropertyChanged();
            }
        }

        private void OpenSample4Dialog(object obj)
        {
            Sample4Content = ColorPicker.Instance;
            IsSample4DialogOpen = true;
        }

        private void CancelSample4Dialog(object obj)
        {
            IsSample4DialogOpen = false;
        }

        private void AcceptSample4Dialog(object obj)
        {
            //pretend to do something for 3 seconds, then close
            //            Sample4Content = new SampleProgressDialog();
            Task.Delay(TimeSpan.FromSeconds(3))
                .ContinueWith((t, _) => IsSample4DialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
