using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
{
    public class CharacterViewModel: INotifyPropertyChanged
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

        private CharacterModel _characterModel;

        public CharacterModel CharacterModel
        {
            get { return _characterModel; }
            set
            {
                _characterModel = value;
                RaisePropertyChanged("CharacterModel");
            }
        }

        public CharacterViewModel()
        {
            _characterModel = new CharacterModel();
        }
    }
}
