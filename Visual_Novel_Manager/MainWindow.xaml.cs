using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Visual_Novel_Manager.View;

namespace Visual_Novel_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = StaticClass.MainWindowViewModelStatic;
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Why is button.ContextMenu.DataContext null? ... when right click is O.K. but leftdown is N.G.
                button.ContextMenu.DataContext = button.DataContext;
                button.ContextMenu.IsOpen = true;
            }
        }



        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var settings = new View.Settings();
            settings.Show();
        }

        private void AddVnMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var addvn = new View.AddVisualNovel();
            addvn.Show();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //use this for testing methods and such
            
        }
    }
}
