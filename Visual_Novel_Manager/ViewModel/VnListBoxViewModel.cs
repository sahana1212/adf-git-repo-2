using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Visual_Novel_Manager.Model;
using System.Drawing;
using System.Windows.Input;
using Visual_Novel_Manager.CustomClasses;

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

        #region properties
        private bool? VnListboxSelectedIndex { get; set; }
        ObservableCollection<VnListboxViewModelCollection> _listboxItems = new ObservableCollection<VnListboxViewModelCollection>();
        public ObservableCollection<VnListboxViewModelCollection> ListboxItems
        {
            get { return _listboxItems; }
            set { _listboxItems = value; }
        }


        private string _selectedTag;
        public string SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                VnSelectedIndexChanged();
            }
        }
        #endregion



        private void VnSelectedIndexChanged()
        {
            //put the code to check if downloading here once I set it up
            var VnIndex = VnListboxModel.VnSelectedIndex;
            VnIndex++;
            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM VnAPI WHERE RowID=@SelectedIndex", con);
                cmd.Parameters.AddWithValue("@SelectedIndex", VnIndex);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    StaticClass.Vnid = (int)reader["VnId"];
                }

                con.Close();
            }
            StaticClass.VnInfoViewModelStatic.BindVnDataCommand.Execute(null);
            StaticClass.CharacterViewModelStatic.BindCharacterDataCommand.Execute(null);
            StaticClass.ReleasesViewModelStatic.BindReleasesCommand.Execute(null);
        }



        public ICommand BindListboxInitialCommand { get { return new AwaitableDelegateCommand(BindListboxExecute); } }
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
            for (int i = 0; i < novelList.Count; i++)
            {
                _listboxItems.Add(new VnListboxViewModelCollection { VnListboxModel = new VnListboxModel { ItemName = novelList[i], ItemIcon = iconList[i] } });
            }


        }









    }

    public class VnListboxViewModelCollection
    {
        public VnListboxModel VnListboxModel { get; set; }
    }
}
