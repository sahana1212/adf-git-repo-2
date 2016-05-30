using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlServerCe;
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
using VisualNovelManager.ConnectionClient;
using VisualNovelManager.JSON;

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for Add_Visual_Novel.xaml
    /// </summary>
    public partial class AddVisualNovel
    {
        #region properties
        public string AddVnName
        {
            get { return txtName.Text; }
            private set { txtName.Text = value; }
        }

        public string AddVnExePath
        {
            get { return txtPath.Text; }
            private set { txtPath.Text = value; }
        }

        public string AddVnId
        {
            get { return txtID.Text; }
            private set { txtID.Text = value; }
        }

        public string AddVnIcon
        {
            get { return txtIcon.Text; }
            private set { txtIcon.Text = value; }
        }
        #endregion

        private BackgroundWorker worker = null;

        public AddVisualNovel()
        {
            InitializeComponent();
            DataContext = StaticClass.AddVnProgressStatic;
        }


        private void ExePathBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                FileName = "",
                DefaultExt = ".exe",
                Filter = "Applications (*.exe)|*.exe;",
                DereferenceLinks = true

            };
            // Default file name
            // Default file extension
            // Filter files by extension 
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                txtPath.Text = dlg.FileName;
            }
        }

        private void IconPathBrowseBtn_Click(object sender, RoutedEventArgs e)
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
                txtIcon.Text = dlg.FileName;
            }
        }


        private async void btnFetch_Click(object sender, RoutedEventArgs e)
        {


            if ((await CheckVnInfo()) == 0)
            {
               StaticClass.AddVnProgressStatic.AddVnProgressVisibleTrueCommand.Execute(null);
                AddToConfig();
                worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_completed);
                worker.RunWorkerAsync();
            }

            
                      
        }

        #region background worker
        void worker_DoWork(object sender, DoWorkEventArgs e)
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

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                progress.Value = e.ProgressPercentage;
            }
            else
            {

                double prog_value = e.ProgressPercentage;
                prog_value = prog_value * 5.8823529411764705882352941176471;//have prog_value multiply by (100 % number of switch cases I have)

                progress.Value = prog_value;
                lblProgress.Content = string.Format("{0}% Complete", Convert.ToInt32(prog_value));
            }

            //pbStatus.Value = e.ProgressPercentage;
        }

        void worker_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus1.Text = "Visual Novel with ID: " + txtID.Text + " added"; //And set status text
            txtID.Text = "";
            txtPath.Text = ""; //Reset textboxes
            txtName.Text = "";
            txtIcon.Text = "";
            progress.Value = 0;
            lblProgress.Content = "";
            Thread.Sleep(300);
            
            StaticClass.AddVnProgressStatic.AddVnProgressVisibleFalseCommand.Execute(null);
            StaticClass.VnListboxViewModelStatic.BindListboxInitialCommand.Execute(null);
        }
        #endregion


      





        private async Task<int> CheckVnInfo()
        {

            //Fetches inforamtion from 'VNDB.org'
            string vnName = null;
            string vnIcon = null;
            if (txtID.Text == "") //Check if ID has been entered
            {
                MessageBox.Show("You have to enter an ID first!", "No ID");
                return -1;
            }

            if (txtPath.Text == "") //Check if path to VN has been entered
            {
                MessageBox.Show("You have not entered a path to the application", "No path added", MessageBoxButton.OK);

                return -1;
            }



            if (File.Exists(txtPath.Text))
            {
                var twoBytes = new byte[2];
                using (var fileStream = File.Open(txtPath.Text, FileMode.Open))
                {
                    fileStream.Read(twoBytes, 0, 2);
                }

                if (Encoding.UTF8.GetString(twoBytes) != "MZ")//checks the first two bytes to see if it is a runnable exe
                {
                    return -1;
                }

            }



            List<string> vnIdList = new List<string>();
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath", con))
                {
                    SqlCeDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string vnid = Convert.ToString((int)reader["VnId"]);
                        vnIdList.Add(vnid);
                    }
                }
                con.Close();
            }

            if (vnIdList.Any(value => value == txtID.Text))
            {
                MessageBox.Show("This Vn is already in the database", "Duplicate Entry", MessageBoxButton.OK);
                //enable when done with testing
                return -1;
            }





            AddVnId = txtID.Text;//set the properties
            AddVnExePath = txtPath.Text;
            if (txtName.Text != "")
            {
                vnName = txtName.Text;
            }

            if (txtIcon.Text != "")
            {
                vnIcon = txtIcon.Text;
            }





            if (AddVnId == "")
            {
                MessageBox.Show("you need to input a novel ID");
                return -1;
            }

            else if (AddVnId == "0")
            {
                MessageBox.Show("A VNDB ID cannot be zero!");
                return -1;
            }













            else if (Regex.IsMatch(AddVnId, @"^([1-9][0-9]*)$"))//I need to have it check from the database for the id, if not found, then if not found from the DB, return a warning or error
            {
                var conn = new Connection();
                await conn.Open();
                int responseCode = Convert.ToInt32(await conn.Login(null, null));

                //error = conn.Login(Convert.ToString("asd" + loginId));
                if (responseCode == -1)
                { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }

                while (true)
                {
                    responseCode = Convert.ToInt32(await conn.Query("dbstats"));
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
                        var dbstats = JsonConvert.DeserializeObject<dbStatsRootObject>(conn.JsonResponse);
                        await conn.Close();

                        var vnid = Convert.ToInt32(AddVnId);
                        if (vnid <= dbstats.vn && vnid > 0)
                        {
                            //var test = ((MainWindow)System.Windows.Application.Current.MainWindow).txtPath.Text;



                            if (AddVnId == "")
                            {
                                var msg = MessageBox.Show("You have not entered a path to the executeable, are you sure you want to add the VN without being able to launch it?", "No path added", MessageBoxButton.YesNo);
                                if (msg == MessageBoxResult.No)
                                {
                                    return -1;
                                }

                            }
                            //this should run only if the id entered is a valid vn #

                            //should end up here if all parameters are correct
                            AddToDatabase.CreateAddVnVariables(AddVnId, AddVnExePath, vnName, vnIcon);

                            //add section to unique part of json

                            var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                            var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
                            foreach (var match in json.unique)
                            {
                                if (match.VnId.ToString() == txtID.Text)//remove the previous entrie(s), and add a new one
                                {
                                    json.unique.Remove(match);
                                    json.unique.Add(new Unique { VnId = Convert.ToInt32(txtID.Text), VnSpoilerLevel = 0, CharacterSpoilerLevel = 0 });
                                    File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
                                }
                                else if (json.unique.Contains(match))
                                {
                                    //don't do anything with previous entries
                                }
                                else
                                {
                                    json.unique.Add(new Unique { VnId = Convert.ToInt32(txtID.Text), VnSpoilerLevel = 0, CharacterSpoilerLevel = 0 });
                                    File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
                                }
                            }

                            //adds the new data to the json object, then serializes it, and writes it to the file





                            return 0;
                        }
                        else if (!(vnid <= dbstats.vn && vnid > 0))
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




        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }




        private void AddToConfig()
        {
            var jsonString = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
            var jsonData = JsonConvert.DeserializeObject<ConfigRootObject>(jsonString);
            jsonData.unique.Add(new Unique { VnId = Convert.ToInt32(txtID.Text), VnSpoilerLevel = StaticClass.VnSpoilerLevel, CharacterSpoilerLevel = StaticClass.CharacterSpoilerLevel });
            File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(jsonData)); 
        }

        

        



       



    }
}
