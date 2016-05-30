using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlServerCe;
using System.Windows.Input;
using Newtonsoft.Json;
using VisualNovelManager.JSON;
using VisualNovelManager.Model;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using VisualNovelManager.View;
using VisualNovelManager.CustomClasses;

namespace VisualNovelManager.ViewModel
{
    public class VnListboxViewModel : INotifyPropertyChanged
    {
        #region property changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region collection
        ObservableCollection<VnListboxViewModelCollection> _listboxItems = new ObservableCollection<VnListboxViewModelCollection>();
        ObservableCollectionWithRange _dropdownItems = new ObservableCollectionWithRange();
        ObservableCollectionAddMenuItemRng _addCategory=new ObservableCollectionAddMenuItemRng();
        ObservableCollectionAddMenuItemRng _removeCategory = new ObservableCollectionAddMenuItemRng();

        public ObservableCollection<VnListboxViewModelCollection> ListboxItems
        {
            get { return _listboxItems; }
            set { _listboxItems = value; }
        }

        public ObservableCollectionWithRange DropdownItems
        {
            get { return _dropdownItems; }
            set { _dropdownItems = value; }
        }

        public ObservableCollectionAddMenuItemRng AddCategory
        {
            get { return _addCategory; }
            set { _addCategory = value; }
        }

        public ObservableCollectionAddMenuItemRng RemoveCategory
        {
            get { return _removeCategory; }
            set { _removeCategory = value; }
        }
        #endregion

        #region constructor and class property
        public VnListboxViewModel()
        {
            _vnListboxModel = new VnListboxModel();
        }

        private VnListboxModel _vnListboxModel;

        public VnListboxModel VnListboxModel
        {
            get { return _vnListboxModel; }
            set
            {
                _vnListboxModel = value;
                RaisePropertyChanged("VnListboxModel");
            }
        }
        #endregion


        private string _selCategory;

        public string SelectedCategory
        {
            get { return _selCategory; }
            set
            {
                _selCategory = value;
                RaisePropertyChanged("SelectedCategory");
            }
        }


        #region command initial bind
        async Task BindListboxExecute()
        {
            _listboxItems.Clear();
            List<string> novelList = new List<string>();
            List<BitmapSource>iconList = new List<BitmapSource>();


            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath", con))
                {
                    SqlCeDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string exe = (string)reader["ExePath"];
                        string nvl = (string)reader["Novel"];
                        if (!reader.IsDBNull(reader.GetOrdinal("IconPath")))
                        {
                            string icopth = (string)reader["IconPath"];
                            var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(icopth);
                            var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                sysicon.Handle,
                                System.Windows.Int32Rect.Empty,
                                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                            sysicon.Dispose();

                            novelList.Add(nvl);
                            iconList.Add(bmpSrc);
                            
                        }
                        else if (reader.IsDBNull(reader.GetOrdinal("IconPath")))
                        {
                            var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(exe);
                            var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                        sysicon.Handle,
                                        System.Windows.Int32Rect.Empty,
                                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                            sysicon.Dispose();

                            novelList.Add(nvl);
                            iconList.Add(bmpSrc);
                        }

                        else
                        {
                            //put code here for when the path or icon isn't valid
                        }
                    }
                }
                //con.Close();
            }

            //code if using ObservableCollection with tuple
            //List<Tuple<string, BitmapSource>> ListBoxTuple = new List<Tuple<string, BitmapSource>>();
            //for (int i = 0; i < novelList.Count; i++)
            //{
            //     ListBoxTuple.Add(new Tuple<string, BitmapSource>(novelList[i],iconList[i]));               
            //}  
            //_listboxItems.AddRange(ListBoxTuple);





            for (int i = 0; i < novelList.Count; i++)
            {
                _listboxItems.Add(new VnListboxViewModelCollection { VnListboxModel = new VnListboxModel { ItemName = novelList[i], ItemIcon = iconList[i] } });
            }            
        }

        bool CanBindListboxExecute()
        {
            return true;
        }

        //public ICommand BindListboxInitialCommand { get { return new RelayCommand(BindListboxExecute, CanBindListboxExecute);} }
        public ICommand BindListboxInitialCommand { get { return new AwaitableDelegateCommand(BindListboxExecute);} }
        #endregion
        async Task BindListboxCategoryExecute()
        {





            _listboxItems.Clear();
            List<string> novelList = new List<string>();
            List<BitmapSource> iconList = new List<BitmapSource>();

            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                List<int> vnid = new List<int>();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelCategories WHERE Category LIKE '%" + SelectedCategory + "%'", con))
                {
                    SqlCeDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!(vnid.Contains((int)reader["VnId"])))
                        {
                            vnid.Add((int)reader["VnId"]);
                        }
                    }
                }
                for (int i = 0; i < vnid.Count; i++)
                {
                    using (SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath WHERE VnId=" + vnid[i], con))
                    {
                        SqlCeDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string exe = (string)reader["ExePath"];
                            string nvl = (string)reader["Novel"];
                            if (!reader.IsDBNull(reader.GetOrdinal("IconPath")))
                            {
                                string icopth = (string)reader["IconPath"];
                                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(icopth);
                                var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                            sysicon.Handle,
                                            System.Windows.Int32Rect.Empty,
                                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                                sysicon.Dispose();
                                novelList.Add(nvl);
                                iconList.Add(bmpSrc);

                            }
                            else
                            {
                                var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(exe);
                                var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                            sysicon.Handle,
                                            System.Windows.Int32Rect.Empty,
                                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                                sysicon.Dispose();
                                novelList.Add(nvl);
                                iconList.Add(bmpSrc);

                            }
                        }
                    }
                }
                con.Close();

                ////implement this code later on:

                //foreach (string VN in lbVN.Items)
                //    item_count++;

                //if (cbCategory.SelectedIndex != -1) //Count listbox items and display it in statusstrip
                //    lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: " + cbCategory.SelectedItem;
                //else
                //    lblNumberOfVNS.Text = item_count + " Visual Novel(s) in Category: All";

                /////

            }


            //code if using ObservableCollection with tuple
            //List<Tuple<string, BitmapSource>> ListBoxTuple = new List<Tuple<string, BitmapSource>>();
            //for (int i = 0; i < novelList.Count; i++)
            //{
            //     ListBoxTuple.Add(new Tuple<string, BitmapSource>(novelList[i],iconList[i]));               
            //}  
            //_listboxItems.AddRange(ListBoxTuple);





            for (int i = 0; i < novelList.Count; i++)
            {
                _listboxItems.Add(new VnListboxViewModelCollection { VnListboxModel = new VnListboxModel { ItemName = novelList[i], ItemIcon = iconList[i] } });
            }


        }

        bool CanBindListboxCategoryExecute()
        {
            return true;
        }

        public ICommand BindListboxCategoryCommand { get { return new AwaitableDelegateCommand(BindListboxCategoryExecute);} }


        async Task LoadCategoriesDropdownExecute()
        {
           



            DropdownItems.Clear();
            var CategoryListArr = new List<string>();
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM Categories", con);
                SqlCeDataReader reader = cmd.ExecuteReader();
                string[] vnvalues = { "Category" };
                while (reader.Read())
                {
                    int i = 0;
                    foreach (string value in vnvalues)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(value)))
                        {
                            CategoryListArr.Add(reader[value].ToString());

                        }
                        i++;
                    }
                }
                con.Close();
            }

            DropdownItems.AddRange(CategoryListArr);




            //
            AddCategory.Clear();
            List<MenuItem> menuitmList = new List<MenuItem>();
            foreach (string categoryName in CategoryListArr)//this loop adds the text to the dropdown menu
            {

                if (categoryName.ToString() != "All")
                {
                    MenuItem menuitm2 = new MenuItem();
                    menuitm2.Header = categoryName;
                    menuitm2.Click += VisualNovelsListbox.ListInstance.AddToCategory_Click;
                    menuitmList.Add(menuitm2);

                }
            }
            AddCategory.AddMenuItemRange(menuitmList);


            RemoveCategory.Clear();
            menuitmList.Clear();
            foreach (string categoryName in CategoryListArr)//this loop adds the text to the dropdown menu
            {

                if (categoryName.ToString() != "All")
                {
                    MenuItem menuitm2 = new MenuItem();
                    menuitm2.Header = categoryName;
                    menuitm2.Click += VisualNovelsListbox.ListInstance.RemoveFromCategory_Click;
                    menuitmList.Add(menuitm2);
                }
            }
            RemoveCategory.AddMenuItemRange(menuitmList);

        }

        bool CanLoadCategoriesDropdownExecute()
        {
            return true;
        }
        public ICommand LoadCategoriesDropdownCommand { get { return new AwaitableDelegateCommand(LoadCategoriesDropdownExecute); } }

       
       



    }

    public class ObsAddRng3 : ObservableCollection<Tuple<string,BitmapSource>>
    {
        public void AddRange(IEnumerable<Tuple<string,BitmapSource>> collection)
        {
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public class ObservableCollectionAddMenuItemRng : ObservableCollection<MenuItem>
    {
        public void AddMenuItemRange(IEnumerable<MenuItem> collection)
        {
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    public class VnListboxViewModelCollection
    {
        public VnListboxModel VnListboxModel { get; set; }
    }
}
