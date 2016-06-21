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
using Visual_Novel_Manager.JSON;

namespace Visual_Novel_Manager.View
{
    /// <summary>
    /// Interaction logic for UserVnList.xaml
    /// </summary>
    public partial class UserVnList : UserControl
    {
        public UserVnList()
        {
            InitializeComponent();
            DataContext = StaticClass.UserVnListViewModelStatic;
        }

        private List<BasicItem> _basicItem;
        private List<DetailsItem> _detailsItem;

        private static readonly List<int> VnIdList = new List<int>();
        //private static readonly List<string> VnNameList = new List<string>();
        private static readonly List<string> NsfwList = new List<string>();
        private List<JSON.UserVnList> UserVnListItem;
        private List<JSON.UserVoteList> UserVoteList;

        public System.Security.SecureString Password
        {
            get { return passwordTxt.SecurePassword; }
        }


        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            return;
        }


        private async void VnListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StaticClass.UserVnListViewModelStatic.BindUserVnCommand.Execute(null);
        }


        private async void Update_OnClick(object sender, RoutedEventArgs e)
        {
            StaticClass.UserVnListViewModelStatic.UpdateUserVnListCommand.Execute(this);
        }


    }
}
