using System.Windows;
using System.Windows.Controls;

namespace GameSET.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
            AllowsTransparency = true;
        }

        private void Transparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = 1 - Transparency.Value;
        }
    }
}
