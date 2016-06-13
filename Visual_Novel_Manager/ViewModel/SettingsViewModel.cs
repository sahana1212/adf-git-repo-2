using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.JSON;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
{
    public class SettingsViewModel: INotifyPropertyChanged
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

        #region constructor and properties
        private SettingsModel _settingsModel;

        public SettingsModel SettingsModel
        {
            get { return _settingsModel; }
            set
            {
                _settingsModel = value;
                RaisePropertyChanged("SettingsModel");
            }
        }
        public SettingsViewModel()
        {
            _settingsModel = new SettingsModel();
           // testcmd = new AwaitableDelegateCommand(SaveConfigSettings);
        }
        #endregion



        public ICommand SaveConfigSettingsCommand { get { return new AwaitableDelegateCommand(SaveConfigSettings);} }
        //public IAsyncCommand testcmd { get; set; }

        public async Task SaveConfigSettings()
        {
            #region
            var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
            var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
            json.global.NsfwEnabled = SettingsModel.NsfwEnabledIndex == 0;//sets nsfw to true if first index, false if second

            for (int i = 0; i < json.unique.Count; i++)
            {
                if (json.unique[i].VnId == StaticClass.Vnid)
                {
                    switch (SettingsModel.VnSpoilerIndex)
                    {
                        case 0:
                            json.unique[i].VnSpoilerLevel = 0;
                            break;
                        case 1:
                            json.unique[i].VnSpoilerLevel = 1;
                            break;
                        case 2:
                            json.unique[i].VnSpoilerLevel = 2;
                            break;
                    }

                    switch (SettingsModel.CharacterSpoilerIndex)
                    {
                        case 0:
                            json.unique[i].CharacterSpoilerLevel = 0;
                            break;
                        case 1:
                            json.unique[i].CharacterSpoilerLevel = 1;
                            break;
                        case 2:
                            json.unique[i].CharacterSpoilerLevel = 2;
                            break;
                    }
                }
                StaticClass.VnSpoilerLevel = json.unique[i].VnSpoilerLevel;
                StaticClass.CharacterSpoilerLevel = json.unique[i].CharacterSpoilerLevel;
            }
            File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
            StaticClass.NsfwEnabled = json.global.NsfwEnabled;
            #endregion


        }



        /////////////////////
        //

        private ICommand _command;

        public ICommand Command
        {
            get{_command =new RelayCommands(shit,CanExec);return _command;}
            
        }


        bool CanExec(object param)
        {
            return true;
        }

        void shit(object param)
        {
            Console.WriteLine("");
        }



    }


}
