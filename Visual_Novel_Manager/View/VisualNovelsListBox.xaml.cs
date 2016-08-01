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

namespace Visual_Novel_Manager.View
{
    /// <summary>
    /// Interaction logic for VisualNovelsListBox.xaml
    /// </summary>
    public partial class VisualNovelsListBox : UserControl
    {
        public static VisualNovelsListBox ListInstance { get; private set; }

        public VisualNovelsListBox()
        {
            InitializeComponent();
            ListInstance = this;
            DataContext = StaticClass.VnListboxViewModelStatic;
            StaticClass.VnListboxViewModelStatic.BindListboxInitialCommand.Execute(null);
            StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);
        }

        //NEED to find a better way to get the item header, preferably using XAML binding, since this doesn't really follow MVVM
        internal void AddToCategory(object sender, RoutedEventArgs e)
        {
            StaticClass.VnListboxViewModelStatic.AddCategoryCommand.Execute(sender);
        }
        internal void RemoveFromCategory(object sender, RoutedEventArgs e)
        {
            StaticClass.VnListboxViewModelStatic.RemoveFromCategoryCommand.Execute(sender);
        }

    }
}
