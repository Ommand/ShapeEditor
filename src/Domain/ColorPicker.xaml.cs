using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Media.ColorConverter;

namespace ShapeEditor.src
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private int buttonsPerRow = 6;
        private double buttonSize = 35;
        private Thickness buttonMargin = new Thickness(4);

        private static ColorPicker _instance;
        public static ColorPicker Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Color picker did not initialized");
                return _instance;
            }
        }
        private ColorPicker()
        {
            InitializeComponent();
            FillWithColors();
        }

        public static void Init(object dataContext)
        {
            if (_instance == null)
                _instance = new ColorPicker();
            _instance.DataContext = dataContext;
        }

        void FillWithColors()
        {
            //declare variables
            var currentColor = 0;
            var row = 0;
            do
            {
                var stackPanel = new StackPanel {Orientation = Orientation.Horizontal};
                lbMain.Items.Add(stackPanel);

                //Add 10 rows to Grid
                for (var j = 0; j < buttonsPerRow; j++)
                {
                    //Define the control that will be added to new row
                    var button = new Button();

                    var binding = new Binding {Path = new PropertyPath("CancelSample4DialogCommand")};
                    //Name of the property in Datacontext
                    button.SetBinding(ButtonBase.CommandProperty, binding);

                    button.Background = new SolidColorBrush(_colorList[currentColor++]);
                    button.Margin = buttonMargin;
                    button.Height = button.Width = buttonSize;

                    //add the stackpanel to the grid
                    stackPanel.Children.Add(button);

                    if (currentColor == _colorList.Count)
                        break;
                }
                row++;

            } while (currentColor != _colorList.Count);
        }

        private List<System.Windows.Media.Color> _colorList =>
                ((string) Application.Current.Resources["ColorsString"]).Split(',')
                    .Select(x => (System.Windows.Media.Color) ConvertFromString(x))
                    .ToList();
    
}
}
