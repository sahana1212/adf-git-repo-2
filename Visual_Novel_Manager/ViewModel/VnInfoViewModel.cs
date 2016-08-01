using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.JSON;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
{
    public class VnInfoViewModel: INotifyPropertyChanged
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
        public VnInfoViewModel()
        {
            _vnInfoModel = new VnInfoModel();
        }

        private VnInfoModel _vnInfoModel;
        public VnInfoModel VnInfoModel
        {
            get { return _vnInfoModel; }
            set
            {
                _vnInfoModel = value;
                RaisePropertyChanged("VnInfoModel");
            }
        }
        #endregion

        #region observable collection
        ObservableCollectionWithRange _vnInfoTags = new ObservableCollectionWithRange();
        ObservableCollectionWithRange _vnInfoRelations = new ObservableCollectionWithRange();
        public ObservableCollectionWithRange VnInfoViewModelTags
        {
            get { return _vnInfoTags; }
            set { _vnInfoTags = value; }
        }

        public ObservableCollectionWithRange VnInfoViewModelRelations
        {
            get { return _vnInfoRelations; }
            set { _vnInfoRelations = value; }
        }
        #endregion

        #region INot properties

        private string _selectedTagItem;
        public string SelectedTagItem
        {
            get { return _selectedTagItem; }
            set
            {
                VnInfoModel.TagDescription = LoadTagDescription().Result;
            }
        }

        private int _selectedTagIndex;

        public int SelectedTagIndex
        {
            get { return _selectedTagIndex; }
            set
            {
                if (_selectedTagIndex != value)
                {
                    _selectedTagIndex = value;
                    RaisePropertyChanged("SelectedTagIndex");
                }
            }
        }
        #endregion

        #region properties
        public string bdRelTitle { get; set; }
        public string bdRelOriginal { get; set; }
        public string bdRelRelation { get; set; }
        #endregion

        #region commands
        async Task BindVnDataExecute()
        {
            try
            {
                if (StaticClass.Vnid >0)
                {
                    string[] VnApiData = { };
                    string[] TimePlayed = { };
                    string[] Developers = { };
                    var Taglist = new List<string[]>();
                    var RelationItems = new List<string[]>();


                    if (File.Exists(StaticClass.CurrentDirectory + @"\config.json"))
                    {
                        var jsonString = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                        var jsonData = JsonConvert.DeserializeObject<ConfigRootObject>(jsonString);
                        foreach (var vn in jsonData.unique)
                        {
                            if (vn.VnId == StaticClass.Vnid)
                            {
                                StaticClass.VnSpoilerLevel = vn.VnSpoilerLevel;
                                break;
                            }
                        }
                    }







                    await Task.Run(() =>
                    {
                        VnApiData = LoadNovelSQLData(11, new string[] { "VnId", "title", "original", "aliases", "released", "length", "description", "popularity", "rating", "image", "image_nsfw" },
                            "SELECT * FROM VnAPI WHERE VnId=").Result;

                        TimePlayed = LoadNovelSQLData(2, new[] { "PlayTime", "LastPlayed" }, "Select * FROM NovelPath WHERE VnId=").Result;

                        Developers = LoadNovelSQLData(1, new[] { "ProducerName" }, "SELECT * FROM ReleaseAPI WHERE ProducerDeveloper='True' AND VnId=").Result;



                    });



                    ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
                    VnInfoModel.VnDescription = convRTD.ConvertToFlowDocument(VnApiData[6]);
                    //VisualNovelsInformation.vndesc.Document = convRTD.ConvertToFlowDocument(VnApiData[6]);

                    //var flowdocument = new FlowDocument();
                    //var paragraph= new Paragraph();
                    //flowdocument.Blocks.Add(paragraph);
                    //paragraph.Inlines.Add(new Run("no tag selected"));




                    VnInfoModel.Name = VnApiData[1];
                    VnInfoModel.Original = VnApiData[2];
                    VnInfoModel.Aliases = VnApiData[3];
                    VnInfoModel.Released = VnApiData[4];
                    VnInfoModel.Length = VnApiData[5];
                    //VnInfoModel.VnDescription = VnApiData[6];
                    VnInfoModel.Popularity = VnApiData[7];
                    VnInfoModel.Rating = VnApiData[8];
                    //VnInfoModel.TagDescription = "no tag selected";
                    VnInfoModel.Developers = Developers[0];

                    #region playtime
                    var tmpSplitPlayTime = TimePlayed[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<int> timecount = new List<int>();
                    for (int i = 0; i < tmpSplitPlayTime.Count(); i++)
                    {
                        timecount.Add(new int());
                        timecount[i] = Convert.ToInt32(tmpSplitPlayTime[i]);
                    }
                    TimeSpan timeSpan = new TimeSpan(timecount[0], timecount[1], timecount[2], timecount[3]);

                    if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes == 0)
                    {
                        VnInfoModel.PlayTime = "< 1 minute";
                    }
                    else
                    {
                        string formatted = string.Format("{0}{1}{2}",
                        timeSpan.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", timeSpan.Days, timeSpan.Days == 1 ? String.Empty : "s") : string.Empty,
                        timeSpan.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", timeSpan.Hours, timeSpan.Hours == 1 ? String.Empty : "s") : string.Empty,
                        timeSpan.Duration().Minutes > 0 ? string.Format("{0:0} minute{1} ", timeSpan.Minutes, timeSpan.Minutes == 1 ? String.Empty : "s") : string.Empty);

                        VnInfoModel.PlayTime = formatted;
                    }

                    if (TimePlayed[1] == "")
                    {
                        VnInfoModel.LastPlayed = "Never";
                    }
                    else
                    {
                        if ((Convert.ToDateTime(TimePlayed[1]) - DateTime.Today).Days > -7)//need to set to negative, for the difference in days
                        {
                            if (Convert.ToDateTime(TimePlayed[1]) == DateTime.Today)
                            {
                                VnInfoModel.LastPlayed = "Today";
                            }
                            else if ((Convert.ToDateTime(TimePlayed[1]) - DateTime.Today).Days > -2 && (Convert.ToDateTime(TimePlayed[1]) - DateTime.Today).Days < 0)
                            {
                                VnInfoModel.LastPlayed = "Yesterday";
                            }
                            else
                            {
                                VnInfoModel.LastPlayed = Convert.ToDateTime(TimePlayed[1]).DayOfWeek.ToString();
                            }
                        }
                        else
                        {
                            VnInfoModel.LastPlayed = TimePlayed[1];
                        }
                    }
                    #endregion

                    #region nsfw
                    if (VnApiData[10] == "True")
                    {
                        if (StaticClass.NsfwEnabled == false)
                        {
                            string path = StaticClass.CurrentDirectory + @"\res\nsfw\cover.jpg";
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.EndInit();
                            VnInfoModel.VnImage = source;
                        }
                        else
                        {
                            if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0]))
                            {
                                string path = StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0];
                                var source = new BitmapImage();
                                source.BeginInit();
                                source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                                source.CacheOption = BitmapCacheOption.OnLoad;
                                source.EndInit();
                                VnInfoModel.VnImage = source;
                            }
                            else
                            {
                                WebClient client = new WebClient();
                                client.DownloadFile(new Uri(VnApiData[9]), StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0]);
                                string path = StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0];
                                var source = new BitmapImage();
                                source.BeginInit();
                                source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                                source.CacheOption = BitmapCacheOption.OnLoad;
                                source.EndInit();
                                VnInfoModel.VnImage = source;
                            }
                        }
                    }
                    else
                    {
                        if (File.Exists(StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0] + ".jpg"))
                        {
                            string path = StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0] + ".jpg";
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.EndInit();
                            VnInfoModel.VnImage = source;
                        }
                        else
                        {
                            WebClient client = new WebClient();
                            client.DownloadFile(new Uri(VnApiData[9]), StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0] + ".jpg");
                            string path = StaticClass.CurrentDirectory + @"\data\cover\" + VnApiData[0] + ".jpg";
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.EndInit();
                            VnInfoModel.VnImage = source;
                        }
                    }
                    #endregion
                    await Task.Run(() =>
                    {
                        Taglist = LoadTags().Result;
                        RelationItems = LoadRelations().Result;
                    });


                    if (_vnInfoTags != null)
                    {
                        List<string> tgList = new List<string>();

                        _vnInfoTags.Clear();//clears observable collection so the new items don't add to existing list
                        for (int i = 0; i < Taglist.Count; (i)++)
                        {
                            if (Convert.ToInt32(Taglist[i][1]) <= StaticClass.VnSpoilerLevel)
                            {
                                tgList.Add(Taglist[i][0]);
                            }
                        }
                        VnInfoViewModelTags.AddRange(tgList);
                    }


                    if (_vnInfoRelations != null)
                    {
                        List<string> relList = new List<string>();
                        _vnInfoRelations.Clear();
                        for (int i = 0; i < RelationItems.Count; i++)
                        {
                            relList.Add(RelationItems[i][0] + ", " + RelationItems[i][1] + ", " + RelationItems[i][2]);
                        }
                        VnInfoViewModelRelations.AddRange(relList);
                    }
                }
                else
                {
                    
                }
                
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: VnInfoViewModel.cs");
                    sw.WriteLine("Method Name: BindVnDataExecute");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
            }

            

        }




        public ICommand BindVnDataCommand { get { return new AwaitableDelegateCommand(BindVnDataExecute); } }


        #endregion

        #region Return Methods

        private async Task<string[]> LoadNovelSQLData(int count, string[] vnvalues, string sqlcmd)
        {
            try
            {
                string[] bindString = null;


                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                {
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlcmd + StaticClass.Vnid, con);
                    //cmd.Parameters.AddWithValue("@SQLcmd", sqlcmd);
                    //cmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    string[] sqlcol = vnvalues;
                    bindString = new string[count];
                    while (reader.Read())
                    {
                        int i = 0;
                        foreach (string value in sqlcol)
                        {
                            bindString[i] = "";
                            if (!reader.IsDBNull(reader.GetOrdinal(value)))
                            {
                                bindString[i] += reader[value];
                            }
                            i++;
                        }

                    }
                    con.Close();
                }
                return bindString;
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: VnInfoViewModel.cs");
                    sw.WriteLine("Method Name: LoadNovelSQLData");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
            }
            
        }


        private async Task<List<string[]>> LoadTags()
        {
            try
            {
                List<string[]> tagList = new List<string[]>();
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                {
                    con.Open();
                    SQLiteCommand gettagscmd = new SQLiteCommand("SELECT * FROM VnTags WHERE VnId=" + StaticClass.Vnid, con);
                    SQLiteDataReader tagreader = gettagscmd.ExecuteReader();
                    while (tagreader.Read())
                    {
                        string tagname = (string)tagreader["TagName"];
                        int spoiler = (int)tagreader["Spoiler"];
                        tagList.Add(new[] { tagname, spoiler.ToString() });
                    }
                    con.Close();
                }
                return tagList;
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: VnInfoViewModel.cs");
                    sw.WriteLine("Method Name: LoadTags");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
            }
            
        }

        private async Task<List<string[]>> LoadRelations()
        {
            try
            {
                List<string[]> relList = new List<string[]>();

                //int SelIndex = View.VisualNovelsListbox.VnListBoxSelectedIndex;
                int SelIndex = 0;
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                {
                    con.Open();


                    SQLiteCommand relationcmd = new SQLiteCommand("SELECT * FROM VnRelations WHERE OriginalVnId=" + StaticClass.Vnid, con);
                    SQLiteDataReader relationreader = relationcmd.ExecuteReader();
                    while (relationreader.Read())
                    {
                        string title = (string)relationreader["Title"];
                        string original = (string)relationreader["Original"];
                        string relation = (string)relationreader["Relation"];
                        relList.Add(new[] { title, original, relation });
                    }




                    for (int i = 0; i < relList.Count; i++)
                    {
                        switch (relList[i][2])
                        {
                            case "preq":
                                relList[i][2] = "Prequel";
                                break;
                            case "seq":
                                relList[i][2] = "Sequel";
                                break;
                            case "char":
                                relList[i][2] = "Shares Characters";
                                break;
                            case "fan":
                                relList[i][2] = "Fandisk";
                                break;
                            case "set":
                                relList[i][2] = "Same Setting";
                                break;
                            case "ser":
                                relList[i][2] = "Same Series";
                                break;
                            case "side":
                                relList[i][2] = "Side Story";
                                break;
                        }

                    }
                    con.Close();
                }
                return relList;
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: VnInfoViewModel.cs");
                    sw.WriteLine("Method Name: LoadRelations");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
            }
        }

        private async Task<FlowDocument> LoadTagDescription()
        {
            try
            {
                string tagdescription = "";
                int tagidx = SelectedTagIndex;
                if (tagidx < 0)
                {
                    //return "tag index was -1";
                    return null;
                }

                int vnSpoiler = StaticClass.VnSpoilerLevel;
                List<int> TagListArray = new List<int>();
                using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                {
                    con.Open();
                    SQLiteCommand tagdesccmd = new SQLiteCommand("SELECT * FROM VnTags WHERE VnId=" + StaticClass.Vnid + " AND Spoiler<=" + vnSpoiler, con);
                    SQLiteDataReader tagdescreader = tagdesccmd.ExecuteReader();
                    while (tagdescreader.Read())
                    {
                        TagListArray.Add((int)tagdescreader["TagId"]);
                    }
                    con.Close();
                }


                foreach (WrittenTagsRootObject tmp2 in StaticClass.PlainTags)
                {
                    if (!TagListArray.Any())
                    {
                        //return "";
                    }
                    else if (TagListArray[tagidx] == tmp2.id)
                    {
                        tagdescription = tmp2.description;
                    }
                }
                //return tagdescription;
                ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
                return convRTD.ConvertToFlowDocument(tagdescription);
                //return "not supossed to happen";
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(StaticClass.CurrentDirectory + @"\debug.log"))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("Exception Found:\tType: {0}", ex.GetType().FullName);
                    sw.WriteLine("Class File: VnInfoViewModel.cs");
                    sw.WriteLine("Method Name: LoadTagDescription");
                    sw.WriteLine("\nMessage: {0}", ex.Message);
                    sw.WriteLine("Source: {0}", ex.Source);
                    sw.WriteLine("StackTrace: {0}", ex.StackTrace);
                    sw.WriteLine("Target Site: {0}", ex.TargetSite);


                    sw.WriteLine("\n\n");
                }
                throw;
            }
            

        }



        #endregion

    }
}
