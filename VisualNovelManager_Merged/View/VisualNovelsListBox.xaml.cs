using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using VisualNovelManager.ViewModel;

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for Visual_Novels_Listbox.xaml
    /// </summary>
    public partial class VisualNovelsListbox
    {

        private static readonly string CurrDirectory = Directory.GetCurrentDirectory();
        public static VisualNovelsListbox ListInstance { get; private set; }

        public static int VnListBoxSelectedIndex{get; private set;}

        public VisualNovelsListbox()
        {
            InitializeComponent();
            cbCategory.SelectedIndex = 0;
            DataContext = StaticClass.VnListboxViewModelStatic;
            StaticClass.VnListboxViewModelStatic.BindListboxInitialCommand.Execute(null);
            ListInstance = this;
            StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);
            
        }

        public void LoadCategoriesForDropdown()
        {

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
            bool addCategory = true;
            foreach (string categoryName in CategoryListArr)//this loop adds the text to the dropdown menu
            {
                foreach (string existingCategory in cbCategory.Items)//skips if no values
                {
                    if (categoryName.ToLower() == existingCategory.ToLower())//checks if the item already exists, if so, doesn't add to the dropdown
                        addCategory = false;
                }
                if (addCategory == true)
                {
                    cbCategory.Items.Add(categoryName.ToString()); //adds the category name to the dropdown
                }
                if (addCategory == true && categoryName.ToString() != "All")
                {
                    MenuItem menuitm = new MenuItem();
                    menuitm.Header = categoryName.ToString();
                    menuitm.Click += AddToCategory_Click;
                    AddToCategory.Items.Add(menuitm);
                }

                //this foreachloop may not be needed
                //foreach (MenuItem existingCategory in AddToCategory.Items)
                //{
                //    //this SHOULD fix the multiple items issue
                //    if (AddToCategory.Items.Count > cbCategory.Items.Count - 2)//if the menu rightclick has more items than the main dropdown 
                //        addCategory = false;
                //}
                addCategory = true;
            }







            foreach (string categoryName in CategoryListArr)//this loop adds the text to the dropdown menu
            {

                if (categoryName.ToString() != "All")
                {
                    MenuItem menuitm2 = new MenuItem();
                    menuitm2.Header = categoryName;
                    menuitm2.Click += RemoveFromCategory_Click;
                    RemoveFromCategory.Items.Add(menuitm2);

                }
            }

        }







        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCategory.SelectedItem == null)
            {
                StaticClass.VnListboxViewModelStatic.BindListboxCategoryCommand.Execute(null);
            }
            else
            {
                //sets the Selected category property to the selected category from the dropdown
                StaticClass.VnListboxViewModelStatic.SelectedCategory = cbCategory.SelectedItem.ToString();
                //loads the list with only novels in the selected category
                StaticClass.VnListboxViewModelStatic.BindListboxCategoryCommand.Execute(null);

            }
        }



        private void lbVN_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {                     
            //When a VN has been selected
            //search through Visual Novel list for matching VN
            //and display information in controls
            if (StaticClass.ScreenshotViewModelStatic.IsDownloading == true)
            {
                lbVN.SelectedIndex = -1;

            }
            else
            {
                VnListBoxSelectedIndex = lbVN.SelectedIndex;
                VnListBoxSelectedIndex++;
                SetVnId();
                if (VnListBoxSelectedIndex >= 1)
                {
                    StaticClass.VnInfoViewModelStatic.BindVnDataCommand.Execute(null);
                    StaticClass.CharacterViewModelStatic.BindCharacterDataCommand.Execute(null);
                    StaticClass.ReleasesViewModelStatic.BindReleasesCommand.Execute(null);
                    StaticClass.ScreenshotViewModelStatic.BindScreenshotsCommand.Execute(null);
                } 
            }
            
            
                      
        }


        private void SetVnId()
        {
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM VnAPI WHERE RowID=@SelectedIndex", con);
                cmd.Parameters.AddWithValue("@SelectedIndex", VnListBoxSelectedIndex);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    StaticClass.Vnid = (int)reader["VnId"];
                }

                con.Close();
            }
        }

       

        #region right click menu
        public void Start_Click(object sender, RoutedEventArgs e)
        {

            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath WHERE RowID=" + VnListBoxSelectedIndex, con);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string exepath = (string)reader["ExePath"];
                    string dirpath = System.IO.Path.GetDirectoryName(exepath);
                    Directory.SetCurrentDirectory(dirpath);
                    System.Diagnostics.Process.Start(exepath);
                    break;
                }
                
                
                con.Close();
            } 

        }

        internal void AddToCategory_Click(object sender, RoutedEventArgs e)
        {
            int SelIndex = lbVN.SelectedIndex;
            SelIndex++;
            

            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                
                MenuItem item = sender as MenuItem;
                if (item != null)
                {
                    var categoryname = item.Header.ToString();

                    int CategoryCounter = 0;
                    SqlCeCommand checkcmd = new SqlCeCommand("SELECT * FROM NovelCategories WHERE VnId=@VnId", con);
                    checkcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    SqlCeDataReader checkreader = checkcmd.ExecuteReader();
                    while (checkreader.Read())
                    {
                        string category = (string) checkreader["Category"];
                        if (category == categoryname)
                        {
                            CategoryCounter++;
                        }
                    }

                    if (!(CategoryCounter >= 1))
                    {
                        SqlCeCommand cmd2 = new SqlCeCommand("INSERT INTO NovelCategories(VnId, Category) VALUES(@VnId, @Category)", con);
                        cmd2.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                        cmd2.Parameters.AddWithValue("@Category", categoryname);
                        cmd2.ExecuteNonQuery();  
                    }

                    
                }
                con.Close();

            }
            
        }



        internal void RemoveFromCategory_Click(object sender, RoutedEventArgs e)
        {
            int SelIndex = lbVN.SelectedIndex;
            SelIndex++;
            if (lbVN.SelectedIndex > -1 && lbVN.SelectedIndex != -1)
            //runs as long as the selected index is 0 or greater
            {


                MenuItem item = sender as MenuItem;
                if (item != null)
                {
                    var categoryname = item.Header.ToString();//gets the name of the category to remove
                    if (categoryname == "All")
                    {
                        MessageBox.Show("Visual Novels can't be removed from category: 'All'", "Removal impossible", MessageBoxButton.OK);
                        return;
                    }

                    using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
                    {
                        
                        con.Open();
                        

                        int CategoryCounter = 0;
                        SqlCeCommand checkallcmd = new SqlCeCommand("Select * FROM NovelCategories WHERE Category=@Category", con);
                        checkallcmd.Parameters.AddWithValue("@Category", categoryname);
                        SqlCeDataReader checker = checkallcmd.ExecuteReader();
                        while (checker.Read())
                        {
                            CategoryCounter++;
                        }


                        SqlCeCommand deletecatcmd = new SqlCeCommand("DELETE FROM NovelCategories WHERE VnId=@VnId AND Category=@Category ", con);
                        deletecatcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                        deletecatcmd.Parameters.AddWithValue("@Category", categoryname);
                        deletecatcmd.ExecuteNonQuery(); 


                        //string removeCatSql = "UPDATE NovelPath SET Category = REPLACE('" + novelCategories + "','," +
                        //                      categoryname + "','') WHERE RowId=" + VnListBoxSelectedIndex;


                        //SqlCeCommand cmd2 = new SqlCeCommand(removeCatSql, con);

                        if (CategoryCounter == 1)
                        {
                            SqlCeCommand deleteAllcatcmd=new SqlCeCommand("DELETE FROM Categories WHERE Category=@Category", con);
                            deleteAllcatcmd.Parameters.AddWithValue("@Category", categoryname);
                            deleteAllcatcmd.ExecuteNonQuery();
                            //cbCategory.Items.Remove(cbCategory.SelectedItem);
                        }
                        SqlCeEngine engine = new SqlCeEngine(@"Data Source=|DataDirectory|\Database.sdf");
                        engine.Shrink();
                        engine.Dispose();


                        con.Close();
                    }
                }

                
                //lblStatus2.Text = novel.englishName + " has been removed from category: " + novel.Category[index - 1].ToString();
            }
            StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);
        }

        private void viewOnVNDB_Click(object sender, RoutedEventArgs e)
        {
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath WHERE RowID=" + VnListBoxSelectedIndex, con);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string vnid = (string)reader["VNID"];
                    System.Diagnostics.Process.Start("https://vndb.org/v" + vnid);
                    //lblStatus2.Text = novel.englishName + " has been opened on 'VNDB'.";
                           break;
                }
                con.Close();
            } 
        }
        #endregion


    }

}

 
