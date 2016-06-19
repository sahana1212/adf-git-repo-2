using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Novel_Manager.Model;

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









        private void SetVnId()
        {
            
        }


    }
}
