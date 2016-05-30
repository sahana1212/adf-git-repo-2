using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlServerCe;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisualNovelManager.Model;

using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VisualNovelManager.CustomClasses;

namespace VisualNovelManager.ViewModel
{
    public class ReleasesViewModel: INotifyPropertyChanged
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

        #region collection
        //ObservableCollection<ReleasesViewModelCollection> _releasesItems = new ObservableCollection<ReleasesViewModelCollection>();

        //public ObservableCollection<ReleasesViewModelCollection> ReleasesItems
        //{
        //    get { return _releasesItems; }
        //    set { _releasesItems = value; }
        //}
        ObservableCollectionWithRange _releasesItems = new ObservableCollectionWithRange();

        public ObservableCollectionWithRange ReleasesItems
        {
            get { return _releasesItems; }
            set { _releasesItems = value; }
        }
        #endregion


        #region constructor and class property

        public ReleasesViewModel()
        {
            _releasesModel = new ReleasesModel();
        }

        private ReleasesModel _releasesModel;

        public ReleasesModel ReleasesModel
        {
            get { return _releasesModel; }
            set
            {
                _releasesModel = value;
                RaisePropertyChanged("ReleasesModel");
            }
        }


        #endregion


        #region INot properties

        private string _selectedReleaseItem;

        public string SelectedReleaseItem
        {
            get { return _selectedReleaseItem; }
            set
            {
                BindData();
                _selectedReleaseItem = value;
            }
        }


        private int _selectedReleaseIndex;

        public int SelectedReleaseIndex
        {
            get { return _selectedReleaseIndex; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _selectedReleaseIndex = value;
                RaisePropertyChanged("SelectedReleaseIndex");
            }
        }



        #endregion






        async Task BindReleasesExecute()
        {
            _releasesItems.Clear();

            var relationlist = new List<string>();

            await Task.Run(() =>
            {
                relationlist = LoadRelations().Result;
            });


            //for (int i = 0; i < relationlist.Count; i++)
            //{
            //    _releasesItems.Add(new ReleasesViewModelCollection
            //    {
            //        ReleasesModel = new ReleasesModel
            //        {
            //            Title = relationlist[i]
            //        }
            //    });
            //}
            List<string> relList = new List<string>();
            for (int i = 0; i < relationlist.Count; i++)
            {
                relList.Add(relationlist[i]);                
            }
            _releasesItems.AddRange(relList);
        }

        public ICommand BindReleasesCommand { get { return new AwaitableDelegateCommand(BindReleasesExecute);} }



        async Task BindData()
        {
            string[] vnData = { };
            var releaseData = new List<string>();

            await Task.Run(() =>
            {
                vnData = LoadVnNsfw().Result;
                releaseData = LoadReleaseData().Result;
            });
            
           

            ReleasesModel.Title = releaseData[0];
            ReleasesModel.Original = releaseData[1];
            ReleasesModel.Released = releaseData[2];
            ReleasesModel.Type = releaseData[3];
            ReleasesModel.Patch = releaseData[4];
            ReleasesModel.Freeware = releaseData[5];
            ReleasesModel.Doujin = releaseData[6];
            ReleasesModel.Languages = releaseData[7];
            ReleasesModel.Website = releaseData[8];
            ReleasesModel.Notes = releaseData[9];
            ReleasesModel.MinAge = releaseData[10];
            ReleasesModel.Gtin = releaseData[11];
            ReleasesModel.Catalog = releaseData[12];
            ReleasesModel.Platforms = releaseData[13];

            if (!(releaseData[14] == "" || releaseData[14] == null))
            {
                var test = releaseData[14].Split(',');
                ReleasesModel.Media = test[0] + " " + test[1];
            }


            #region nssfw
            if (vnData[0] == "True")
            {
                if (StaticClass.NsfwEnabled == false)
                {
                    string path = StaticClass.CurrentDirectory + @"\res\nsfw\cover.jpg";
                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    source.CacheOption = BitmapCacheOption.OnLoad;  
                    source.EndInit();
                    ReleasesModel.VnImage = source;
                }
                else
                {
                    if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid))
                    {
                        string path = StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid;
                        var source = new BitmapImage();
                        source.BeginInit();
                        source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        source.CacheOption = BitmapCacheOption.OnLoad;  
                        source.EndInit();
                        ReleasesModel.VnImage= source;
                    }
                    else
                    {
                        WebClient client = new WebClient();
                        client.DownloadFile(new Uri(vnData[1]), StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid);
                        string path = StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid;
                        var source = new BitmapImage();
                        source.BeginInit();
                        source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        source.CacheOption = BitmapCacheOption.OnLoad;
                        source.EndInit();
                        ReleasesModel.VnImage = source;
                    }
                }
            }
            else
            {
                if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg"))
                {
                    string path = StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg";
                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();
                    ReleasesModel.VnImage = source;
                }
                else
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(new Uri(vnData[1]), StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg");
                    string path = StaticClass.CurrentDirectory + @"\data\cover\" + StaticClass.Vnid + ".jpg";
                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();
                    ReleasesModel.VnImage = source;
                }
            }
            #endregion








            ReleasesModel.ProducerName = releaseData[15];
            ReleasesModel.ProducerOriginal = releaseData[16];
            ReleasesModel.ProducerDeveloper = releaseData[17];
            ReleasesModel.ProducerPublisher = releaseData[18];
            ReleasesModel.ProducerType = releaseData[19];


        }






        private async Task<List<string>> LoadRelations()
        {
            List<string> relationList = new List<string>();

            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM ReleaseAPI WHERE VnId="+StaticClass.Vnid, con);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string title = (string) reader["Title"];
                    relationList.Add(title);
                }
                con.Close();
            }




            return relationList;
        }

        private async Task<List<string>> LoadReleaseData()
        {
            var releaselist = new List<string>();
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                List<int> releaseIdList = new List<int>();

                SqlCeCommand relidcmd = new SqlCeCommand("SELECT * FROM ReleaseAPI WHERE VnId=@VnId", con);
                relidcmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                SqlCeDataReader relidReader = relidcmd.ExecuteReader();
                while (relidReader.Read())
                {
                    int relid = (int) relidReader["ReleaseId"];
                    releaseIdList.Add(relid);
                }


                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM ReleaseAPI WHERE VnId=@VnId AND ReleaseId=@ReleaseId", con);
                cmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                cmd.Parameters.AddWithValue("@ReleaseId", releaseIdList[SelectedReleaseIndex]);

                string[] sqlArr =
                {
                   "Title", "Original", "Released", "Type", "Patch", "Freeware", "Doujin", "Languages",
                    "Website", "Notes", "MinAge", "GTIN", "Catalog", "Platforms", "Media", "ProducerName", "ProducerOriginal",
                    "ProducerDeveloper", "ProducerPublisher", "ProducerType"
                };

                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foreach (var value in sqlArr)
                    {
                        releaselist.Add(reader[value].ToString());
                    }
                }




                con.Close();
            }





            return releaselist;
        }


        private async Task<string[]> LoadVnNsfw()
        {
            string[] vndataList = new string[2];
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM VnAPI WHERE VnId=@VnId", con);
                cmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {                  
                    vndataList[0] = (string)reader["image_nsfw"];
                    vndataList[1] = (string)reader["image"];
                }
                con.Close();
            }
            return vndataList;
        }


        
        
    }
}
