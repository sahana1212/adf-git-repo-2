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
using Visual_Novel_Manager.View;

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

        #region constructor and setup
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

            PreviousButtonEnabled = true;
            NextButtonEnabled = true;
        }
        #endregion

        #region collection
        ObservableCollectionWithRange _charTraits = new ObservableCollectionWithRange();

        public ObservableCollectionWithRange CharacterViewModelTraits
        {
            get { return _charTraits; }
            set { _charTraits = value; }
        }

        #endregion

        #region static properties

        private int CurrentCharacterCount { get; set; }
        private string CurrentCharacterId { get; set; }
        private int CharacterCountMax { get; set; }



        private string _selectedTraitItem;
        public string SelectedTraitItem
        {
            get { return _selectedTraitItem; }
            set
            {
                CharacterModel.TraitDescription = LoadTraitDescription(CurrentCharacterId).Result;
            }
        }


        private int _selectedTraitIndex;
        public int SelectedTraitIndex
        {
            get { return _selectedTraitIndex; }
            set
            {
                if (_selectedTraitIndex != value)
                {
                    _selectedTraitIndex = value;
                    RaisePropertyChanged("SelectedTraitIndex");
                }
            }
        }


        private bool _previousButtonEnabled;
        public bool PreviousButtonEnabled
        {
            get { return _previousButtonEnabled; }
            set
            {
                if (_previousButtonEnabled != value)
                {
                    _previousButtonEnabled = value;
                    RaisePropertyChanged("PreviousButtonEnabled");
                }
            }
        }

        private bool _nextButtonEnabled;
        public bool NextButtonEnabled
        {
            get { return _nextButtonEnabled; }
            set
            {
                if (_nextButtonEnabled != value)
                {
                    _nextButtonEnabled = value;
                    RaisePropertyChanged("NextButtonEnabled");
                }
            }
        }


        #endregion

        #region commands
        async Task BindCharacterDataExecute()
        {
            var charData = new List<List<string[]>>();
            var TraitList = new List<string[]>();

            if (File.Exists(StaticClass.CurrentDirectory + @"\config.json"))
            {
                var jsonString = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                var jsonData = JsonConvert.DeserializeObject<ConfigRootObject>(jsonString);
                foreach (var vn in jsonData.unique)
                {
                    if (vn.VnId == StaticClass.Vnid)
                    {
                        StaticClass.CharacterSpoilerLevel = vn.CharacterSpoilerLevel;
                        break;
                    }
                }
            }



            await Task.Run(() =>
            {
                charData = LoadMainCharacterData().Result;
            });


            if (Convert.ToInt32(charData[CurrentCharacterCount][0][14]) > StaticClass.CharacterSpoilerLevel)
            {
                await NextCharacterDataExecute();
                //loads the next image if the first character is a spoiler
            }


            CurrentCharacterId = charData[CurrentCharacterCount][0][0];
            ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
            CharacterModel.Description = convRTD.ConvertToFlowDocument(charData[CurrentCharacterCount][0][7]);

            CharacterModel.Name = charData[CurrentCharacterCount][0][1];
            CharacterModel.Original = charData[CurrentCharacterCount][0][2];
            CharacterModel.Gender = charData[CurrentCharacterCount][0][3];
            CharacterModel.BloodType = charData[CurrentCharacterCount][0][4];
            CharacterModel.Birthday = charData[CurrentCharacterCount][0][5];
            CharacterModel.Aliases = charData[CurrentCharacterCount][0][6];
            //CharacterModel.Description = charData[CurrentCharacterCount][0][7];            
            CharacterModel.Bust = charData[CurrentCharacterCount][0][9];
            CharacterModel.Waist = charData[CurrentCharacterCount][0][10];
            CharacterModel.Hips = charData[CurrentCharacterCount][0][11];
            CharacterModel.Height = charData[CurrentCharacterCount][0][12];
            CharacterModel.Weight = charData[CurrentCharacterCount][0][13];




            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid))
            {
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid);
            }
            string charimg = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + charData[CurrentCharacterCount][0][0] + ".jpg";
            if (File.Exists(charimg))
            {

                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();

                CharacterModel.CharImage = source;
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile(new Uri(charData[CurrentCharacterCount][0][8]), StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg");
                string path = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg";
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
                CharacterModel.CharImage = source;
            }

            await Task.Run(() =>
            {
                TraitList = LoadTraits(charData[CurrentCharacterCount][0][0]).Result;
            });


            if (_charTraits != null)
            {
                List<string> traitList = new List<string>();
                _charTraits.Clear();

                for (int i = 0; i < TraitList.Count; (i)++)
                {
                    if (Convert.ToInt32(TraitList[i][1]) <= StaticClass.CharacterSpoilerLevel)
                    {
                        traitList.Add(TraitList[i][0]);
                    }
                }
                CharacterViewModelTraits.AddRange(traitList);
            }


            if (CurrentCharacterCount == 0)
            {
                PreviousButtonEnabled = false;
            }
            else if (CurrentCharacterCount == CharacterCountMax)
            {
                NextButtonEnabled = false;
            }


        }

        public ICommand BindCharacterDataCommand { get { return new AwaitableDelegateCommand(BindCharacterDataExecute); } }




        async Task PreviousCharacterDataExecute()
        {
            //should be redundant, but keep it anyway
            if (CurrentCharacterCount == 0)
            {
                PreviousButtonEnabled = false;
            }
            if (CurrentCharacterCount > 0)
            {
                PreviousButtonEnabled = true;
            }
            if (CurrentCharacterCount == CharacterCountMax)
            {
                NextButtonEnabled = false;
            }
            if (CurrentCharacterCount < CharacterCountMax)
            {
                NextButtonEnabled = true;
            }
            CurrentCharacterCount--;
            //end redundancy

            var charData = new List<List<string[]>>();
            var TraitList = new List<string[]>();

            await Task.Run(() =>
            {
                charData = LoadMainCharacterData().Result;
            });

            if (Convert.ToInt32(charData[CurrentCharacterCount][0][14]) > StaticClass.CharacterSpoilerLevel)
            {

                if (CurrentCharacterCount == CharacterCountMax)
                { NextButtonEnabled = false; }
                if (CurrentCharacterCount == 0)
                { PreviousButtonEnabled = false; }

                if (CurrentCharacterCount > 0)
                {
                    PreviousButtonEnabled = true;
                    PreviousCharacterDataCommand.Execute(null);
                }

                else if (CurrentCharacterCount < CharacterCountMax)
                {
                    NextButtonEnabled = true;
                    NextCharacterDataCommand.Execute(null);
                }



                //loads the next non spoiler image
            }





            CurrentCharacterId = charData[CurrentCharacterCount][0][0];
            ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
            CharacterModel.Description = convRTD.ConvertToFlowDocument(charData[CurrentCharacterCount][0][7]);

            CharacterModel.Name = charData[CurrentCharacterCount][0][1];
            CharacterModel.Original = charData[CurrentCharacterCount][0][2];
            CharacterModel.Gender = charData[CurrentCharacterCount][0][3];
            CharacterModel.BloodType = charData[CurrentCharacterCount][0][4];
            CharacterModel.Birthday = charData[CurrentCharacterCount][0][5];
            CharacterModel.Aliases = charData[CurrentCharacterCount][0][6];
            //CharacterModel.Description = charData[CurrentCharacterCount][0][7];
            CharacterModel.Bust = charData[CurrentCharacterCount][0][9];
            CharacterModel.Waist = charData[CurrentCharacterCount][0][10];
            CharacterModel.Hips = charData[CurrentCharacterCount][0][11];
            CharacterModel.Height = charData[CurrentCharacterCount][0][12];
            CharacterModel.Weight = charData[CurrentCharacterCount][0][13];


            string charimg = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + charData[CurrentCharacterCount][0][0] + ".jpg";
            if (File.Exists(charimg))
            {

                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();

                CharacterModel.CharImage = source;
            }
            else
            {
                WebClient client = new WebClient();
                client.DownloadFile(new Uri(charData[CurrentCharacterCount][0][8]), StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg");
                string path = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg";
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
                CharacterModel.CharImage = source;
            }



            await Task.Run(() =>
            {
                TraitList = LoadTraits(charData[CurrentCharacterCount][0][0]).Result;
            });


            if (_charTraits != null)
            {
                List<string> traitList = new List<string>();
                _charTraits.Clear();

                for (int i = 0; i < TraitList.Count; (i)++)
                {
                    if (Convert.ToInt32(TraitList[i][1]) <= StaticClass.CharacterSpoilerLevel)
                    {
                        traitList.Add(TraitList[i][0]);
                    }
                }
                CharacterViewModelTraits.AddRange(traitList);
            }

            if (CurrentCharacterCount == 0)
            {
                PreviousButtonEnabled = false;
            }
            else if (CurrentCharacterCount > 0)
            {
                PreviousButtonEnabled = true;
            }
            if (CurrentCharacterCount == CharacterCountMax)
            {
                NextButtonEnabled = false;
            }
            if (CurrentCharacterCount != CharacterCountMax)
            {
                NextButtonEnabled = true;
            }


        }

        public ICommand PreviousCharacterDataCommand { get { return new AwaitableDelegateCommand(PreviousCharacterDataExecute); } }





        async Task NextCharacterDataExecute()
        {

            if (CurrentCharacterCount == 0)
            {
                PreviousButtonEnabled = false;
            }
            else if (CurrentCharacterCount > 0)
            {
                PreviousButtonEnabled = true;
            }
            if (CurrentCharacterCount == CharacterCountMax)
            {
                NextButtonEnabled = false;
                return;
            }

            CurrentCharacterCount++;

            var charData = new List<List<string[]>>();
            var TraitList = new List<string[]>();

            await Task.Run(() =>
            {
                charData = LoadMainCharacterData().Result;
            });


            if (Convert.ToInt32(charData[CurrentCharacterCount][0][14]) > StaticClass.CharacterSpoilerLevel)
            {

                if (CurrentCharacterCount == CharacterCountMax)
                { NextButtonEnabled = false; }
                if (CurrentCharacterCount == 0)
                { PreviousButtonEnabled = false; }

                if (CurrentCharacterCount < CharacterCountMax)
                {
                    NextButtonEnabled = true;
                    NextCharacterDataCommand.Execute(null);
                }

                else if (CurrentCharacterCount > 0)
                {
                    PreviousButtonEnabled = true;
                    PreviousCharacterDataCommand.Execute(null);
                }

                //loads the next non spoiler image
            }
            else
            {
                CurrentCharacterId = charData[CurrentCharacterCount][0][0];
                ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
                CharacterModel.Description = convRTD.ConvertToFlowDocument(charData[CurrentCharacterCount][0][7]);

                CharacterModel.Name = charData[CurrentCharacterCount][0][1];
                CharacterModel.Original = charData[CurrentCharacterCount][0][2];
                CharacterModel.Gender = charData[CurrentCharacterCount][0][3];
                CharacterModel.BloodType = charData[CurrentCharacterCount][0][4];
                CharacterModel.Birthday = charData[CurrentCharacterCount][0][5];
                CharacterModel.Aliases = charData[CurrentCharacterCount][0][6];
                //CharacterModel.Description = charData[CurrentCharacterCount][0][7];
                CharacterModel.Bust = charData[CurrentCharacterCount][0][9];
                CharacterModel.Waist = charData[CurrentCharacterCount][0][10];
                CharacterModel.Hips = charData[CurrentCharacterCount][0][11];
                CharacterModel.Height = charData[CurrentCharacterCount][0][12];
                CharacterModel.Weight = charData[CurrentCharacterCount][0][13];



                string charimg = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + charData[CurrentCharacterCount][0][0] + ".jpg";
                if (File.Exists(charimg))
                {

                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();

                    CharacterModel.CharImage = source;
                }
                else
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(new Uri(charData[CurrentCharacterCount][0][8]), StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg");
                    string path = StaticClass.CurrentDirectory + @"\data\character\" + StaticClass.Vnid + @"\" + CurrentCharacterId + ".jpg";
                    var source = new BitmapImage();
                    source.BeginInit();
                    source.UriSource = new Uri(charimg, UriKind.RelativeOrAbsolute);
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();
                    CharacterModel.CharImage = source;
                }


                await Task.Run(() =>
                {
                    TraitList = LoadTraits(charData[CurrentCharacterCount][0][0]).Result;
                });


                if (_charTraits != null)
                {
                    List<string> traitList = new List<string>();
                    _charTraits.Clear();

                    for (int i = 0; i < TraitList.Count; (i)++)
                    {
                        if (Convert.ToInt32(TraitList[i][1]) <= StaticClass.CharacterSpoilerLevel)
                        {
                            traitList.Add(TraitList[i][0]);
                        }
                    }
                    CharacterViewModelTraits.AddRange(traitList);
                }


                if (CurrentCharacterCount == 0)
                {
                    PreviousButtonEnabled = false;
                }
                if (CurrentCharacterCount > 0)
                {
                    PreviousButtonEnabled = true;
                }
                if (CurrentCharacterCount == CharacterCountMax)
                {
                    NextButtonEnabled = false;
                }
                if (CurrentCharacterCount < CharacterCountMax)
                {
                    NextButtonEnabled = true;
                }
            }

        }

        public ICommand NextCharacterDataCommand { get { return new AwaitableDelegateCommand(NextCharacterDataExecute); } }

        #endregion

        #region return methods

        private async Task<List<List<string[]>>> LoadMainCharacterData()
        {

            CharacterCountMax = 0;
            int characterCount = 0;
            var characterList = new List<List<string[]>>();

            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();


                SQLiteCommand cmd2 = new SQLiteCommand("SELECT * FROM CharacterAPI WHERE VnId=@VnId", con);
                cmd2.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                SQLiteDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    CharacterCountMax++;
                    characterCount++;
                }
                CharacterCountMax = characterCount - 1;
                for (int i = 0; i < characterCount; i++)
                {
                    var inlst = new List<string[]>();
                    characterList.Add(inlst);
                }

                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM CharacterAPI WHERE VnId=@VnId", con);
                cmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                string[] sqlArr =
                {
                   "CharacterId", "Name", "Original", "Gender", "BloodType", "Birthday", "Aliases", "Description",
                    "Image", "Bust", "Waist", "Hip", "Height", "Weight", "vns"
                };


                int h = 0;
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var bindString = new string[15];
                    int i = 0;
                    foreach (string value in sqlArr)
                    {
                        bindString[i] = "";
                        if (!reader.IsDBNull(reader.GetOrdinal(value)))
                        {
                            bindString[i] += reader[value];
                        }
                        i++;
                    }

                    characterList[h].Add(bindString);

                    h++;
                }
                con.Close();
            }


            return characterList;
        }

        private async Task<List<string[]>> LoadTraits(string charid)
        {
            List<string[]> traitList = new List<string[]>();


            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();


                SQLiteCommand gettraitscmd = new SQLiteCommand("SELECT * FROM CharacterTraits WHERE VnId=@VnId AND CharacterId=@CharacterId", con);
                gettraitscmd.Parameters.AddWithValue("@Vnid", StaticClass.Vnid);
                gettraitscmd.Parameters.AddWithValue("@CharacterId", charid);
                SQLiteDataReader traitsreader = gettraitscmd.ExecuteReader();
                while (traitsreader.Read())
                {
                    string traitname = (string)traitsreader["TraitName"];
                    int spoiler = (int)traitsreader["SpoilerLevel"];
                    traitList.Add(new[] { traitname, spoiler.ToString() });
                }
                con.Close();
            }




            return traitList;
        }

        private async Task<FlowDocument> LoadTraitDescription(string charid)
        {
            string traitDescription = "";
            int traitindex = SelectedTraitIndex;
            if (traitindex < 0)
            {
                //return "trait index was -1";
            }



            List<int> TraitListArray = new List<int>();


            using (SQLiteConnection con = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                con.Open();



                SQLiteCommand traitdesccmd = new SQLiteCommand("SELECT * FROM CharacterTraits WHERE VnId=@VnId AND CharacterId=@CharacterId AND SpoilerLevel<=@SpoilerLevel", con);
                traitdesccmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid);
                traitdesccmd.Parameters.AddWithValue("@CharacterId", charid);
                traitdesccmd.Parameters.AddWithValue("@SpoilerLevel", StaticClass.CharacterSpoilerLevel);
                SQLiteDataReader traitdescreader = traitdesccmd.ExecuteReader();
                while (traitdescreader.Read())
                {
                    TraitListArray.Add((int)traitdescreader["TraitId"]);
                }
                con.Close();
            }


            foreach (var tmp2 in StaticClass.PlainTraits)
            {
                if (!TraitListArray.Any())
                {
                    //return "";
                }
                else if (TraitListArray[traitindex] == tmp2.id)
                {
                    traitDescription = tmp2.description;
                }

            }


            ConvertRichTextDocument convRTD = new ConvertRichTextDocument();
            return convRTD.ConvertToFlowDocument(traitDescription);



            //return traitDescription;
        }


        #endregion
    }
}
