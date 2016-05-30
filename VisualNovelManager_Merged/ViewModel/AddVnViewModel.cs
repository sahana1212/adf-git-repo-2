using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using VisualNovelManager.Model;


namespace VisualNovelManager.ViewModel
{
    public class AddVnViewModel: INotifyPropertyChanged
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

        #region setup
        public AddVnViewModel()
        {
           _addVnModel = new AddVnModel();
        }

        private AddVnModel _addVnModel;

        public AddVnModel AddVnModel
        {
            get { return _addVnModel; }
            set
            {
                _addVnModel = value;
                RaisePropertyChanged("AddVnModel");
            }
        }
        #endregion


        private bool _isProgressVisible;
        public bool IsProgressVisible
        {
            get { return _isProgressVisible; }
            set
            {
                _isProgressVisible = value;
                RaisePropertyChanged("IsProgressVisible");
            }
        }


        #region commands
        async Task AddVnProgressVisibleTrueExecute()
        {
            IsProgressVisible = true;
        }

        private bool CanAddVnProgressVisibleTrueExecute()
        {
            return true;
        }

        public ICommand AddVnProgressVisibleTrueCommand { get { return new AwaitableDelegateCommand(AddVnProgressVisibleTrueExecute); } }




        async Task AddVnProgressVisibleFalseExecute()
        {
            IsProgressVisible = false;
        }

        private bool CanAddVnProgressVisibleFalseExecute()
        {
            return true;
        }

        public ICommand AddVnProgressVisibleFalseCommand { get { return new AwaitableDelegateCommand(AddVnProgressVisibleFalseExecute);} }


        #endregion

    }
}
