using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using VisualNovelManager.ConnectionClient;
using VisualNovelManager.JSON;
using VisualNovelManager.Model;
using VisualNovelManager.View;
using VisualNovelManager.ViewModel;

//my references


namespace VisualNovelManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {

        //public static readonly VnInfoViewModel VnInfoViewModelStatic = new VnInfoViewModel();
        //public static readonly VnListboxViewModel VnListboxViewModelStatic = new VnListboxViewModel();
        //public static readonly CharacterViewModel CharacterViewModelStatic= new CharacterViewModel();
        
 
        private List<BasicItem> _basicItem;


        public MainWindow()
        {
            InitializeComponent();
           

        }



        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
           Settings globalSettings= new Settings();
            globalSettings.Show();
            
        }

        private void btnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            #region createCat
            if (txtCategory.Text == "") //If no category name has been entered abort
            {
                MessageBox.Show("Enter a category name first", "No category name", MessageBoxButton.OK);
                txtCategory.Focus();
                return;
            }

            bool addCategory = true;
            var CatComboBox = new View.VisualNovelsListbox();
            foreach (string existingCategory in CatComboBox.cbCategory.Items)
            {
                if (existingCategory.ToLower() == txtCategory.Text.ToLower())
                    addCategory = false;
            }

            if (addCategory == true)
            {
                using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
                {
                    con.Open();
                    SqlCeCommand cmd = new SqlCeCommand("INSERT INTO Categories(Category) VALUES(@Category)", con);
                    cmd.Parameters.AddWithValue("@Category", CheckForDbNull(txtCategory.Text));
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                addCategory = true;
            }

            if (addCategory == true)
            {
                MenuItem item = new MenuItem();
                item.Header = txtCategory.Text;
                item.Click += CatComboBox.AddToCategory_Click;
                CatComboBox.AddToCategory.Items.Add(item);
            }

            //addCategory = true;

            var removeCategoryItems = CatComboBox.RemoveFromCategory.Items;
            foreach (MenuItem remSubItem in removeCategoryItems)
            {
                if (remSubItem.ToString().ToLower() == txtCategory.Text.ToLower())
                {
                    addCategory = false;
                }
            }

            if (addCategory == true)
            {
                MenuItem item = new MenuItem();
                item.Header = txtCategory.Text;
                item.Click += CatComboBox.RemoveFromCategory_Click;
                CatComboBox.RemoveFromCategory.Items.Add(item);
            }
            #endregion

            


          StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);



        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StaticClass.Vnid <= 0)
            {
                return;
            }
            else
            {
                using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
                {

                    #region sql
                    con.Open();

                    SqlCeCommand delNovelPathcmd = new SqlCeCommand("DELETE FROM NovelPath WHERE VnId=@VnId", con);
                    delNovelPathcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delNovelPathcmd.ExecuteNonQuery();

                    


                    SqlCeCommand delCharactercmd= new SqlCeCommand("DELETE FROM CharacterAPI WHERE VnId=@VnId", con);
                    delCharactercmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delCharactercmd.ExecuteNonQuery();

                    SqlCeCommand delCharacterTraitscmd = new SqlCeCommand("DELETE FROM CharacterTraits WHERE VnId=@VnId", con);
                    delCharacterTraitscmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delCharacterTraitscmd.ExecuteNonQuery();

                    SqlCeCommand delNovelCategorycmd = new SqlCeCommand("DELETE FROM NovelCategories WHERE VnId=@VnId", con);
                    delNovelCategorycmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delNovelCategorycmd.ExecuteNonQuery();

                   

                    SqlCeCommand delReleasecmd = new SqlCeCommand("DELETE FROM ReleaseAPI WHERE VnId=@VnId", con);
                    delReleasecmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delReleasecmd.ExecuteNonQuery();

                    SqlCeCommand delVnAPIcmd = new SqlCeCommand("DELETE FROM VnAPI WHERE VnId=@VnId", con);
                    delVnAPIcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delVnAPIcmd.ExecuteNonQuery();

                    SqlCeCommand delVnRelationcmd = new SqlCeCommand("DELETE FROM VnRelations WHERE OriginalVnId=@OriginalVnId", con);
                    delVnRelationcmd.Parameters.AddWithValue("@OriginalVnId", StaticClass.Vnid);
                    delVnRelationcmd.ExecuteNonQuery();

                    SqlCeCommand delVnTagcmd = new SqlCeCommand("DELETE FROM VnTags WHERE VnId=@VnId", con);
                    delVnTagcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    delVnTagcmd.ExecuteNonQuery();
                    con.Close();
                    SqlCeEngine engine = new SqlCeEngine(@"Data Source=|DataDirectory|\Database.sdf");
                    engine.Shrink();
                    engine.Dispose();
                    #endregion
                    
                    
                    if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid))
                    {
                        File.Delete(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid);
                    }
                    else if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg"))
                    {
                        File.Delete(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg");
                    }
                    Directory.Delete(StaticClass.CurrentDirectory + @"\data\character\"+StaticClass.Vnid,true);
                    Directory.Delete(StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid, true);

                    var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                    var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
                    foreach (var match in json.unique.Where(match => match.VnId.ToString() == StaticClass.Vnid.ToString()))
                    {
                        json.unique.Remove(match);                           
                        File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
                        break;
                    }

                    StaticClass.VnListboxViewModelStatic.BindListboxInitialCommand.Execute(null);
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddVN_Click(object sender, RoutedEventArgs e)
        {
            AddVisualNovel test = new AddVisualNovel();
            test.Show();  
        }

        private async void Testing_Click(object sender, RoutedEventArgs e)
        {
            //use this for testing code out


            return;
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
