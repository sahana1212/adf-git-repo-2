using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using Visual_Novel_Manager.ConnectionClient;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.JSON;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
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

        #region setup constructor
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
        public AddVnViewModel()
        {
            _addVnModel = new AddVnModel();
        }
        #endregion

        BackgroundWorker worker = null;

        // private ICommand _exePathBrowseCommand;
        //public ICommand ExePathBrowseCommand
        // {
        //     get { _exePathBrowseCommand= new RelayCommands(ExePathBrowseExecute,CanExecute);
        //         return _exePathBrowseCommand;
        //     }
        // }

        public ICommand ExePathBrowseCommand { get { return new AwaitableDelegateCommand(ExePathBrowseExecute);} }
        public ICommand IconPathBrowseCommand { get { return new AwaitableDelegateCommand(IconBrowseExecute);} }
        public ICommand AddVnExecuteCommand { get { return new AwaitableDelegateCommand(AddVnExecute);} }


        async Task ExePathBrowseExecute()
        {
            
             var dlg = new OpenFileDialog
            {
                FileName = "",
                DefaultExt = ".exe",
                Filter = "Applications (*.exe)|*.exe;",
                DereferenceLinks = true

            };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                AddVnModel.ExePath = dlg.FileName;
            }

           
        }


        async Task IconBrowseExecute()
        {
            var dlg = new OpenFileDialog
            {
                FileName = "",
                DefaultExt = ".ico",
                Filter = "Icons (*.ico, *.exe) | *.ico; *.exe; "
            };
            // Default file name
            // Default file extension
            //dlg.Filter = "Icons (.exe),(.ico)|*.exe,*.ico"; // Filter files by extension 
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                AddVnModel.IconPath = dlg.FileName;
            }
        }


        async Task AddVnExecute()
        {
            AddToDatabase.CreateAddVnVariables(AddVnModel.VnId, AddVnModel.ExePath, "", "");
            if (await CheckVnId() == 0)
            {
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_completed);
                worker.RunWorkerAsync();
            }

        }



        #region return methods

        async Task<int> CheckVnId()
        {
            if (string.IsNullOrEmpty(AddVnModel.VnId))
            {
                MessageBox.Show("You have to enter an ID first!", "No ID");
                return -1;
            }

            if (AddVnModel.VnId == "0")
            {
                MessageBox.Show("A VNDB ID cannot be zero!");
                return -1;
            }

            if (string.IsNullOrEmpty(AddVnModel.ExePath))
            {
                MessageBox.Show("You have not entered a path to the application", "No path added", MessageBoxButton.OK);

                return -1;
            }

            if (File.Exists(AddVnModel.ExePath))
            {
                var twoBytes = new byte[2];
                using (var fileStream = File.Open(AddVnModel.ExePath, FileMode.Open))
                {
                    fileStream.Read(twoBytes, 0, 2);
                }

                if (Encoding.UTF8.GetString(twoBytes) !="MZ")//checks the firsst two bytes tto see if the exe can be run(prevents adding .ink)
                {
                    return -1;
                }
            }

            
            List<int>vnIdList = new List<int>();
            using (SQLiteConnection dbConnection = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
            {
                dbConnection.Open();
                SQLiteCommand command= new SQLiteCommand("SELECT VnId FROM NovelPath",dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vnIdList.Add(reader.GetInt32(0));                   
                }
                reader.Close();
            }
            
            //checks if an id already exists in the sql db
            List<int> x= new List<int>();
            if (vnIdList.Any(id => Convert.ToInt32(AddVnModel.VnId) == id))
            {
                MessageBox.Show("This Vn is already in the database", "Duplicate Entry", MessageBoxButton.OK);
                return -1;
            }


            //I need to have it check from the database for the id, if not found, then if not found from the DB, return a warning or error
            if (Regex.IsMatch(AddVnModel.VnId, @"^([1-9][0-9]*)$"))
            {
                var conn = new Connection();
                await conn.Open();
                int responseCode = Convert.ToInt32(await conn.Login(null, null));

                if (responseCode == -1)
                { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }

                while (true)
                {
                    responseCode = Convert.ToInt32(await conn.Query("get vn basic (id = " + AddVnModel.VnId + " )"));
                    if (responseCode == -1)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                        if (error.id == "throttled")
                        {
                            Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                        }
                        else
                        {
                            MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                        }
                    }

                    if (responseCode == 0)
                    {

                        var vn = JsonConvert.DeserializeObject<BasicRootObject>(conn.JsonResponse);
                        await conn.Close();

                        if (vn.items.Count > 0)
                        {
                            //this should run only if the id entered is a valid vn #

                            //!!!Put the AddToDatabase code here



                            //perhaps set the spoiler levels to the static spoiler levels
                            var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                            var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
                            foreach (var match in json.unique)
                            {
                                if (match.VnId.ToString() == AddVnModel.VnId)//remove the previous entrie(s), and add a new one
                                {
                                    json.unique.Remove(match);
                                    json.unique.Add(new Unique {VnId = Convert.ToInt32(AddVnModel.VnId), VnSpoilerLevel = 0, CharacterSpoilerLevel = 0});
                                    File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
                                }
                                else if(json.unique.Contains(match))
                                {
                                    //don't do anything with previous entries
                                }
                                else//add new entry
                                {
                                    json.unique.Add(new Unique { VnId = Convert.ToInt32(AddVnModel.VnId), VnSpoilerLevel = 0, CharacterSpoilerLevel = 0 });
                                    File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
                                }
                            }
                            //adds the new data to the json object, then serializes it, and writes it to the file
                            return 0;
                        }
                        else if (vn.items.Count <= 0)
                        {
                            MessageBox.Show("Not a valid VNDB ID");
                            return -1;
                        }
                        break;
                    }


                }
            }



            MessageBox.Show("Please enter a valid VNDB number ID");
            return -1;
        }






        #endregion




        #region worker

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            AddToDatabase addToDatabase = new AddToDatabase();
            for (int switchvar = 0; switchvar < 17; switchvar++)
            {
                if (worker.CancellationPending)
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            "Do you want to cancel this task? Cancelling could cause issue with the database file",
                            "Cancel Operation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                switch (switchvar)
                {
                    case 0:
                        addToDatabase.fetch_vn_basic();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 1:
                        addToDatabase.fetch_vn_details();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 2:
                        addToDatabase.fetch_vn_relations();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 3:
                        addToDatabase.fetch_vn_stats();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;

                    case 4:
                        addToDatabase.fetch_vn_screens();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 5:
                        addToDatabase.fetch_releases();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 6:
                        addToDatabase.fetch_characters();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 7:
                        addToDatabase.fetch_tags();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 8:
                        addToDatabase.fetch_traits();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 9:
                        addToDatabase.AddNovelData();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 10:
                        addToDatabase.AddNovelCategory();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 11:
                        addToDatabase.AddVnApi();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 12:
                        addToDatabase.AddVnRelations();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 13:
                        addToDatabase.AddVnTags();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 14:
                        addToDatabase.AddReleaseApi();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 15:
                        addToDatabase.AddCharacterApi();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;
                    case 16:
                        addToDatabase.AddCharacterTraits();
                        (sender as BackgroundWorker).ReportProgress(switchvar);
                        break;

                }
                Thread.Sleep(220);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                AddVnModel.ProgressBar = e.ProgressPercentage;

                AddVnModel.ProgressPercent = string.Format("{0}% Complete", Convert.ToInt32(AddVnModel.ProgressBar));
                //progress.Value = e.ProgressPercentage;
            }
            else
            {
                AddVnModel.ProgressBar = e.ProgressPercentage;
                AddVnModel.ProgressBar = AddVnModel.ProgressBar * 6.25;

                AddVnModel.ProgressPercent = string.Format("{0}% Complete", Convert.ToInt32(AddVnModel.ProgressBar));
                //have prog_value multiply by (100 % number of switch cases I have)

                //progress.Value = prog_value;
                //lblProgress.Content = string.Format("{0}% Complete", Convert.ToInt32(prog_value));
            }

            //pbStatus.Value = e.ProgressPercentage;
        }

        private void worker_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Complete");
        }
        #endregion

    }
}
