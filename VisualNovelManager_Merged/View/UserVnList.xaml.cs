using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using VisualNovelManager.ConnectionClient;
using VisualNovelManager.JSON;
using VisualNovelManager.ViewModel;

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for VisualNovelListTab.xaml
    /// </summary>
    public partial class UserVnList : UserControl, UserVnListViewModel.IHavePassword
    {
        private List<BasicItem> _basicItem;
        private List<DetailsItem> _detailsItem;

        private static readonly List<int> VnIdList = new List<int>();
        //private static readonly List<string> VnNameList = new List<string>();
        private static readonly List<string> NsfwList= new List<string>();
        private List<JSON.UserVnList> UserVnListItem;
        private List<JSON.UserVoteList> UserVoteList;





        
        public UserVnList()
        {
            InitializeComponent();
            DataContext = StaticClass.UserVnListViewModelStatic;
        }

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




        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }


        public object CheckForDbNull(object value)       //this forces entries that are null to work in the database
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }


    }
}
