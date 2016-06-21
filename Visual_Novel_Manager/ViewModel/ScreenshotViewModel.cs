using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
{
    public class ScreenshotViewModel: INotifyPropertyChanged
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

        ObservableCollection<ScreenshotViewModelCollection> _screenshotCollection = new ObservableCollection<ScreenshotViewModelCollection>();

        public ObservableCollection<ScreenshotViewModelCollection> ScreenshotCollection
        {
            get { return _screenshotCollection; }
            set { _screenshotCollection = value; }
        }


        #endregion collection

        #region constructor and properties

        public ScreenshotViewModel()
        {
            _screenshotModel = new ScreenshotModel();
            MyBrush = System.Windows.Media.Brushes.Black;
        }

        private ScreenshotModel _screenshotModel;

        public ScreenshotModel ScreenshotModel
        {
            get { return _screenshotModel; }
            set
            {
                _screenshotModel = value;
                RaisePropertyChanged("ScreenshotModel");
            }
        }

        #endregion

        #region static properties


        private string _selectedScreenItem;
        public string SelectedScreenItem
        {
            get { return _selectedScreenItem; }
            set
            {
                string path = LoadLargeScreenshot().Result;
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
                ScreenshotModel.Screenshot = source;
                _selectedScreenItem = value;
            }

        }


        private int _selectedScreenIndex;
        public int SelectedScreenIndex
        {
            get { return _selectedScreenIndex; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _selectedScreenIndex = value;
                RaisePropertyChanged("SelectedScreenIndex");
            }
        }



        private System.Windows.Media.Brush _brush;
        public System.Windows.Media.Brush MyBrush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                RaisePropertyChanged("MyBrush");
            }
        }

        private bool _isDownloading;

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                _isDownloading = value;
                RaisePropertyChanged("IsDownloading");
            }
        }






        #endregion


        #region commands

        async Task BindScreenshotsExecute()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsDownloading = true;
            });

            int vnId = StaticClass.Vnid;
            var screenarray = new List<List<string>>();
            await Task.Run(() =>
            {
                screenarray = GetScreenshotList().Result;
            });



            if (_screenshotCollection != null)
            {
                _screenshotCollection.Clear();


                foreach (var image in screenarray)
                {


                    if (image[1] == "True")
                    {
                        string filenameNoExt = Path.GetFileNameWithoutExtension(image[0]);
                        if (StaticClass.NsfwEnabled == false)
                        {
                            string path = StaticClass.CurrentDirectory + @"\res\nsfw\thumb.jpg";
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.EndInit();

                            _screenshotCollection.Add(new ScreenshotViewModelCollection
                            {
                                ScreenshotModel = new ScreenshotModel
                                {
                                    Screenshot = source


                                }
                            });
                        }
                        else if (StaticClass.NsfwEnabled == true)
                        {
                            string path = StaticClass.CurrentDirectory + @"\data\screenshots\" + vnId + @"\" + filenameNoExt;
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.EndInit();

                            _screenshotCollection.Add(new ScreenshotViewModelCollection
                            {
                                ScreenshotModel = new ScreenshotModel
                                {
                                    Screenshot = source


                                }
                            });
                        }
                        else
                        {
                            _screenshotCollection.Add(new ScreenshotViewModelCollection { ScreenshotModel = new ScreenshotModel { Screenshot = null } });
                        }



                    }
                    else if (image[1] == "False")
                    {
                        string filename = Path.GetFileName(image[0]);
                        string path = StaticClass.CurrentDirectory + @"\data\screenshots\" + vnId + @"\" + filename;
                        var source = new BitmapImage();
                        source.BeginInit();
                        source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        source.CacheOption = BitmapCacheOption.OnLoad;
                        source.EndInit();


                        _screenshotCollection.Add(new ScreenshotViewModelCollection
                        {
                            ScreenshotModel = new ScreenshotModel
                            {
                                Screenshot = source
                            }
                        });
                    }
                    else
                    {
                        _screenshotCollection.Add(new ScreenshotViewModelCollection { ScreenshotModel = new ScreenshotModel { Screenshot = null } });
                    }

                }

            }


            Application.Current.Dispatcher.Invoke(() =>
            {
                IsDownloading = false;
            });




        }



        public ICommand BindScreenshotsCommand { get { return new AwaitableDelegateCommand(BindScreenshotsExecute); } }



        #endregion


        #region return methods


        private async Task<string> LoadLargeScreenshot()
        {
            string screenshots = null;
            List<List<string>> screenarray = new List<List<string>>();
            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();



                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM VnAPI WHERE VnId=" + StaticClass.Vnid, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screenshots = (string)reader["Screenshots"];

                }
                con.Close();
            }

            var sample = screenshots.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            int j = 0;
            for (int i = 0; i < sample.Count(); i++)
            {
                screenarray.Add(new List<string>());
                var screentmp = sample[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                screenarray[i].AddRange(screentmp);
            }



            if (screenarray[SelectedScreenIndex][1] == "True")
            {
                MyBrush = System.Windows.Media.Brushes.Red;
                string filenameNoExt = Path.GetFileNameWithoutExtension(screenarray[SelectedScreenIndex][0]);
                if (StaticClass.NsfwEnabled == false)
                {
                    return StaticClass.CurrentDirectory + @"\res\nsfw\screenshot.jpg";
                }
                return StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filenameNoExt;
            }
            else
            {
                MyBrush = System.Windows.Media.Brushes.Black;
                //return screenarray[SelectedScreenIndex][0];
                string filename = Path.GetFileName(screenarray[SelectedScreenIndex][0]);
                return StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filename;
            }




        }

        private async Task<List<List<string>>> GetScreenshotList()
        {
            string screenshots = null;
            List<List<string>> screenarray = new List<List<string>>();
            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();



                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM VnAPI WHERE VnId=" + StaticClass.Vnid, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screenshots = (string)reader["Screenshots"];

                }
                con.Close();
            }

            var sample = screenshots.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            int j = 0;
            for (int i = 0; i < sample.Count(); i++)
            {
                screenarray.Add(new List<string>());
                var screentmp = sample[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                screenarray[i].AddRange(screentmp);
            }
            Thread.Sleep(0);

            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\"))
            {
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\");
            }

            foreach (var image in screenarray)
            {


                if (image[1] == "True")
                {
                    string filenameNoExt = Path.GetFileNameWithoutExtension(image[0]);

                    if (!File.Exists(StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filenameNoExt))
                    {
                        WebClient client = new WebClient();
                        client.DownloadFile(new Uri(image[0]), StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filenameNoExt);

                    }
                }
                else
                {
                    string filename = Path.GetFileName(image[0]);

                    if (!File.Exists(StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filename))
                    {
                        WebClient client = new WebClient();
                        client.DownloadFile(new Uri(image[0]), StaticClass.CurrentDirectory + @"\data\screenshots\" + StaticClass.Vnid + @"\" + filename);

                    }

                }
                System.Threading.Thread.Sleep(100);
            }

            return screenarray;
        }





        #endregion
    }

    public class ScreenshotViewModelCollection
    {
        public ScreenshotModel ScreenshotModel { get; set; }
    }
}
