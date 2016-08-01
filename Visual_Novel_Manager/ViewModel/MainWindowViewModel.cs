using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.Model;
using System.Windows;
using System.Windows.Controls;

namespace Visual_Novel_Manager.ViewModel
{
    public class MainWindowViewModel: INotifyPropertyChanged
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

        #region constructor and class property

        private MainWindowModel _mainWindowModel;

        public MainWindowModel MainWindowModel
        {
            get { return _mainWindowModel; }
            set
            {
                _mainWindowModel = value;
                RaisePropertyChanged("MainWindowMoodel");
            }
        }
        public MainWindowViewModel()
        {
            _mainWindowModel = new MainWindowModel();
        }
        #endregion

        private bool _addCategoryVisibilty = false;

        public bool AddCategoryVisibility
        {
            get { return _addCategoryVisibilty; }
            set
            {
                _addCategoryVisibilty = value;
                RaisePropertyChanged("AddCategoryVisibility");
            }
        }


        public ICommand ShowCategoryCommand { get {return new RelayCommand(ShowCategory);} }
        private void ShowCategory()
        {
            AddCategoryVisibility = true;
        }

        public ICommand CreateCategoryCommand { get { return new RelayCommand(CreateCategory);} }

        private void CreateCategory()
        {
            try
            {
                if (string.IsNullOrEmpty(MainWindowModel.CategoryText))
                {
                    MessageBox.Show("Enter a category name first", "No category name", MessageBoxButton.OK);
                    return;
                }
                else
                {

                    int addCategory = 0;
                    var CatComboBox = new View.VisualNovelsListBox();
                    foreach (string existingCategory in CatComboBox.cbCategory.Items)
                    {
                        if (existingCategory.ToLower() == MainWindowModel.CategoryText.ToLower())
                            addCategory = -1;//set to -1 if the category already exists
                    }

                    if (addCategory == 0)
                    {
                        using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                        {
                            con.Open();
                            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Categories(Category) VALUES(@Category)", con);
                            cmd.Parameters.AddWithValue("@Category", CheckForDbNull(MainWindowModel.CategoryText));
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }


                        MenuItem addItem = new MenuItem();
                        addItem.Header = MainWindowModel.CategoryText;
                        addItem.Click += CatComboBox.AddToCategory;
                        CatComboBox.AddCategory.Items.Add(addItem);//might be violating MVVM. Need to check on it

                        MenuItem remItem = new MenuItem();
                        remItem.Header = MainWindowModel.CategoryText;
                        remItem.Click += CatComboBox.RemoveFromCategory;
                        CatComboBox.RemoveCategory.Items.Add(remItem);//might be violating MVVM

                        StaticClass.VnListboxViewModelStatic.LoadCategoriesDropdownCommand.Execute(null);
                    }

                    if (addCategory == -1)
                    {
                        MainWindowModel.CategoryText = "";
                    }



                    //
                    AddCategoryVisibility = false;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: MainWindowViewModel.cs");
                    sw.WriteLine("Method Name: CreateCategory");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
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
