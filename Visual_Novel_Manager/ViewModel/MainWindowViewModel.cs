using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.Model;
using System.Windows;

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


        //public ICommand CreateCategoryCommand { get { return new AwaitableDelegateCommand(CreateCategory);} }

        async Task CreateCategory()
        {
            if (MainWindowModel.CategoryText != null || MainWindowModel.CategoryText != "")
            {
                MessageBox.Show("Enter a category name first", "No category name", MessageBoxButton.OK);
                return;
            }
            else
            {
                bool addCategory = true;
                

            }
        }


    }
}
