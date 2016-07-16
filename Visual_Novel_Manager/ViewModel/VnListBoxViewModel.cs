using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Visual_Novel_Manager.Model;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.View;

namespace Visual_Novel_Manager.ViewModel
{
    public class VnListBoxViewModel: INotifyPropertyChanged
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

        #region constructor and setup
        private VnListboxModel _listboxModel;
        public VnListboxModel VnListboxModel
        {
            get { return _listboxModel; }
            set
            {
                _listboxModel = value;
                RaisePropertyChanged("VnListboxModel");
            }
        }

        public VnListBoxViewModel()
        {
            _listboxModel = new VnListboxModel();
        }
        #endregion

        #region collection
        ObservableCollection<VnListboxViewModelCollection> _listboxItems = new ObservableCollection<VnListboxViewModelCollection>();
        ObservableCollectionWithRange _dropdownItems = new ObservableCollectionWithRange();
        ObservableCollectionAddMenuItemRange _addCategory = new ObservableCollectionAddMenuItemRange();
        ObservableCollectionAddMenuItemRange _removeCategory = new ObservableCollectionAddMenuItemRange();
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

        public ObservableCollectionAddMenuItemRange AddCategory
        {
            get { return _addCategory; }
            set { _addCategory = value; }
        }

        public ObservableCollectionAddMenuItemRange RemoveCategory
        {
            get { return _removeCategory; }
            set { _removeCategory = value; }
        }
        #endregion

        #region properties
        private int VnListboxSelectedIndex { get; set; }





        private string _selectedVn;
        public string SelectedVn
        {
            get { return _selectedVn; }
            set
            {
                VnSelectedIndexChanged();
            }
        }


        private string _selectedCatChanged;
        public string SelectedCatChanged
        {
            get { return _selectedCatChanged; }
            set
            {
                _selectedCatChanged = value;
                RaisePropertyChanged("SelectedCatChanged");
                SelectedCategoryChanged();
            }
        }
        #endregion


        private string _fileName;

        public string FileName => $"_{0 + 1} - {_fileName}";


        private void VnSelectedIndexChanged()
        {
            Console.WriteLine(VnListboxModel.VnSelectedIndex);
           // return;
            //put the code to check if downloading here once I set it up
            if (StaticClass.ScreenshotViewModelStatic.IsDownloading == true)
            {
                VnListboxModel.VnSelectedIndex = -1;

            }
            else
            {
                if (VnListboxModel.VnSelectedIndex >= -1)
                {
                    var VnIndex = VnListboxModel.VnSelectedIndex;
                    VnIndex++;
                    using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                    {
                        con.Open();
                        List<int> vnList = new List<int>();
                        if (SelectedCatChanged == null)
                        {
                            SelectedCatChanged = "All";
                        }
                        SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM NovelCategories WHERE Category LIKE @query ", con);
                        cmd.Parameters.AddWithValue("@query", "%" + SelectedCatChanged + "%");
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            vnList.Add((int)reader["VnId"]);
                            StaticClass.Vnid = (int)reader["VnId"];
                        }

                        StaticClass.Vnid = vnList[VnIndex -1];
                        //SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM VnAPI WHERE RowID=@SelectedIndex", con);
                        //cmd.Parameters.AddWithValue("@SelectedIndex", VnIndex);
                        //SQLiteDataReader reader = cmd.ExecuteReader();
                        //while (reader.Read())
                        //{
                        //    StaticClass.Vnid = (int)reader["VnId"];
                        //}

                        con.Close();
                    }
                    StaticClass.VnInfoViewModelStatic.BindVnDataCommand.Execute(null);
                    //StaticClass.CharacterViewModelStatic.BindCharacterDataCommand.Execute(null);
                    //StaticClass.ReleasesViewModelStatic.BindReleasesCommand.Execute(null);
                    StaticClass.ScreenshotViewModelStatic.BindScreenshotsCommand.Execute(null);
                }
                else
                {
                    
                }

            }



        }



        private void SelectedCategoryChanged()
        {
            if (SelectedCatChanged == "All" || SelectedCatChanged == null || SelectedCatChanged == "")
            {
                BindListboxInitialCommand.Execute(null);
            }
            else
            {
                ListBoxSelectedCategoryChanged();
                StaticClass.VnInfoViewModelStatic.BindVnDataCommand.Execute(null);
            }
        }






        public ICommand LoadCategoriesDropdownCommand { get { return new AwaitableDelegateCommand(LoadCategoriesDropdown);} }

        async Task LoadCategoriesDropdown()
        {
            DropdownItems.Clear();
            var CategoryListArr = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Categories", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
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


            //return;

            //
            AddCategory.Clear();
            List<MenuItem> menuitmList = new List<MenuItem>();
            foreach (string categoryName in CategoryListArr)//this loop adds the text to the dropdown menu
            {

                if (categoryName.ToString() != "All")
                {
                    MenuItem menuitm2 = new MenuItem();
                    menuitm2.Header = categoryName;
                    menuitm2.Click += VisualNovelsListBox.ListInstance.AddToCategory;
                    //menuitm2.Command = AddToCategoryCommand;
                    //menuitm2.Click += VisualNovelsListbox.ListInstance.AddToCategory_Click;
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
                    menuitm2.Click += VisualNovelsListBox.ListInstance.RemoveFromCategory;
                    menuitmList.Add(menuitm2);
                }
            }
            RemoveCategory.AddMenuItemRange(menuitmList);
        }


        //BindListboxSelectedCategoryChangedExecute is used when a different category is selected




        private RelayCommand<object> _addCategoryCommand;
        public ICommand AddCategoryCommand
        {
            get
            {
                _addCategoryCommand= new RelayCommand<object>(AddToCategory_Click);
                return _addCategoryCommand;
            }
        }


        private void AddToCategory_Click(object header)
        {

            int SelIndex = VnListboxModel.VnSelectedIndex;
            SelIndex++;


            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();

                //MenuItem item = new MenuItem();
                MenuItem item = header as MenuItem;
                if (item != null)
                {
                    var categoryname = item.Header.ToString();

                    int CategoryCounter = 0;
                    SQLiteCommand checkcmd = new SQLiteCommand("SELECT * FROM NovelCategories WHERE VnId=@VnId", con);
                    checkcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    SQLiteDataReader checkreader = checkcmd.ExecuteReader();
                    while (checkreader.Read())
                    {
                        string category = (string)checkreader["Category"];
                        if (category == categoryname)
                        {
                            CategoryCounter++;
                        }
                    }

                    if (!(CategoryCounter >= 1))
                    {
                        SQLiteCommand cmd2 = new SQLiteCommand("INSERT INTO NovelCategories(VnId, Category) VALUES(@VnId, @Category)", con);
                        cmd2.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                        cmd2.Parameters.AddWithValue("@Category", categoryname);
                        cmd2.ExecuteNonQuery();
                    }


                }
                con.Close();

            }

        }



        private RelayCommand<object> _removeFromCategoryCommand;
        public ICommand RemoveFromCategoryCommand
        {
            get
            {
                _removeFromCategoryCommand = new RelayCommand<object>(RemoveFromCategory_Click);
                return _removeFromCategoryCommand;
            }
        }
        private void RemoveFromCategory_Click(object header)
        {
            int SelIndex = VnListboxModel.VnSelectedIndex;
            SelIndex++;
            if (VnListboxModel.VnSelectedIndex > -1 && VnListboxModel.VnSelectedIndex != -1)
            //runs as long as the selected index is 0 or greater
            {


                MenuItem item = header as MenuItem;
                if (item != null)
                {
                    var categoryname = item.Header.ToString();//gets the name of the category to remove
                    if (categoryname == "All")
                    {
                        MessageBox.Show("Visual Novels can't be removed from category: 'All'", "Removal impossible", MessageBoxButton.OK);
                        return;
                    }

                    using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                    {

                        con.Open();


                        int CategoryCounter = 0;
                        SQLiteCommand checkallcmd = new SQLiteCommand("Select * FROM NovelCategories WHERE Category=@Category", con);
                        checkallcmd.Parameters.AddWithValue("@Category", categoryname);
                        SQLiteDataReader checker = checkallcmd.ExecuteReader();
                        while (checker.Read())
                        {
                            CategoryCounter++;
                        }


                        SQLiteCommand deletecatcmd = new SQLiteCommand("DELETE FROM NovelCategories WHERE VnId=@VnId AND Category=@Category ", con);
                        deletecatcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                        deletecatcmd.Parameters.AddWithValue("@Category", categoryname);
                        deletecatcmd.ExecuteNonQuery();


                        //string removeCatSql = "UPDATE NovelPath SET Category = REPLACE('" + novelCategories + "','," +
                        //                      categoryname + "','') WHERE RowId=" + VnListBoxSelectedIndex;


                        //SQLiteCommand cmd2 = new SQLiteCommand(removeCatSql, con);

                        if (CategoryCounter == 1)
                        {
                            SQLiteCommand deleteAllcatcmd = new SQLiteCommand("DELETE FROM Categories WHERE Category=@Category", con);
                            deleteAllcatcmd.Parameters.AddWithValue("@Category", categoryname);
                            deleteAllcatcmd.ExecuteNonQuery();
                            //cbCategory.Items.Remove(cbCategory.SelectedItem);
                        }
                        //SQLiteEngine engine = new SQLiteEngine(@"Data Source=|DataDirectory|\Database.sdf");
                        //engine.Shrink();
                        //engine.Dispose();


                        con.Close();
                    }
                }


                //lblStatus2.Text = novel.englishName + " has been removed from category: " + novel.Category[index - 1].ToString();
            }
            StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);
        }






        #region Listbox
        public ICommand BindListboxInitialCommand { get { return new AwaitableDelegateCommand(BindListboxExecute); } }
        //BindListboxEcecute binds items to listbox without checking category, used for 'All' category
        async Task BindListboxExecute()
        {
            _listboxItems.Clear();
            List<string> novelList = new List<string>();
            List<BitmapSource> iconList = new List<BitmapSource>();


            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM NovelPath", con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string exe = (string)reader["ExePath"];
                        string nvl = (string)reader["Novel"];

                        if (!reader.IsDBNull(reader.GetOrdinal("IconPath")))
                        {
                            string icopth = (string)reader["IconPath"];
                            if (File.Exists(icopth))
                            {
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
                                var bmpSrc = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null,
                                    new byte[] { 0, 0, 0, 0 }, 4);
                                novelList.Add(nvl);
                                iconList.Add(bmpSrc);
                            }
                            

                        }
                        else if (reader.IsDBNull(reader.GetOrdinal("IconPath")))
                        {
                            if (File.Exists(exe))
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
                                var bmpSrc = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null,
                                    new byte[] {0, 0, 0, 0}, 4);
                                novelList.Add(nvl);
                                iconList.Add(bmpSrc);
                            }
                            
                        }

                        else
                        {
                            //put code here for when the path or icon isn't valid
                        }
                    }
                }
                con.Close();
            }
            for (int i = 0; i < novelList.Count; i++)
            {
                _listboxItems.Add(new VnListboxViewModelCollection { VnListboxModel = new VnListboxModel { ItemName = novelList[i], ItemIcon = iconList[i] } });
            }


        }



         private void ListBoxSelectedCategoryChanged()
        {
            var SelectedCategory = SelectedCatChanged;
            _listboxItems.Clear();
            List<string> novelList = new List<string>();
            List<BitmapSource> iconList = new List<BitmapSource>();

            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();

                List<int> vnid = new List<int>();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM NovelCategories WHERE Category LIKE '%" + SelectedCategory + "%'", con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
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
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM NovelPath WHERE VnId=" + vnid[i], con))
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
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

            for (int i = 0; i < novelList.Count; i++)
            {
                _listboxItems.Add(new VnListboxViewModelCollection { VnListboxModel = new VnListboxModel { ItemName = novelList[i], ItemIcon = iconList[i] } });
            }
        }

        #endregion



    }

    public class ObservableCollectionAddMenuItemRange : ObservableCollection<MenuItem>
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
