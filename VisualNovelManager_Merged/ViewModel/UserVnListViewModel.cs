using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MS.Internal.Text.TextInterface;
using Newtonsoft.Json;
using VisualNovelManager.ConnectionClient;
using VisualNovelManager.JSON;
using VisualNovelManager.Model;
using VisualNovelManager.View;
using UserVnList = VisualNovelManager.JSON.UserVnList;
using VisualNovelManager.CustomClasses;


namespace VisualNovelManager.ViewModel
{
    public class UserVnListViewModel: INotifyPropertyChanged
    {
        #region property changed & IHavePassword
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public interface IHavePassword
        {
            System.Security.SecureString Password { get; }
        }

        #endregion


        #region collection
        ObservableCollectionWithRange _userVnListCollection= new ObservableCollectionWithRange();

        public ObservableCollectionWithRange UserVnListCollection
        {
            get { return _userVnListCollection; }
            set { _userVnListCollection = value; }
        }
         

        #endregion


        #region constructor and class property

        public UserVnListViewModel()
        {
            _userVnListModel = new UserVnListModel();
        }

        private UserVnListModel _userVnListModel;

        public UserVnListModel UserVnListModel
        {
            get { return _userVnListModel; }
            set
            {
                _userVnListModel = value;
                RaisePropertyChanged("UserVnListModel");
            }
        }

        #endregion

        private List<BasicItem> _basicItem;
        private List<DetailsItem> _detailsItem;

        private static readonly List<int> VnIdList = new List<int>();
        private static readonly List<string> NsfwList = new List<string>();
        private List<JSON.UserVnList> UserVnListItem;
        private List<JSON.UserVoteList> UserVoteList;



        #region static properties
        private double _progbarValue;
        public Double ProgbarValue
        {
            get { return _progbarValue; }
            set
            {
                _progbarValue = value;
                RaisePropertyChanged("ProgbarValue");
            }
        }


        private int _listboxSectedIndex;

        public int ListboxSelectedIndex
        {
            get { return _listboxSectedIndex; }
            set
            {
                _listboxSectedIndex = value;
                RaisePropertyChanged("ListboxSelectedIndex");
            }
        }

        
        #endregion



        #region command

        private async Task GetUserVnListExecute(object parameter)
        {
            UserVnListModel.UsernameEnabled = false;
            UserVnListModel.PasswordEnabled = false;
            UserVnListModel.LoginBtnEnabled = false;
            var passwordContainer = parameter as IHavePassword;
            SecureString passwordSecure = null;
            if (passwordContainer != null)
            {
                passwordSecure = passwordContainer.Password;
               // ConvertToUnsecureString(passwordSecure);
            }
            //checks if the login is correct. If not, sets boxes back to
            var conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(UserVnListModel.Username, ConvertToUnsecureString(passwordSecure)));
            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
                UserVnListModel.UsernameEnabled = true;
                UserVnListModel.PasswordEnabled = true;
                UserVnListModel.LoginBtnEnabled = true;
                await conn.Close();
                return;
            }
            await conn.Close();

            var username = UserVnListModel.Username;
            List<string> VnNameList = new List<string>();
            //put in async Task.Run()
            await Task.Run(() =>
            {
                
                
                VnNameList = GetVnListVoteList(username, passwordSecure).Result;
            });



            //foreach (var vnName in VnNameList)
            //{
            //    UserVnList.Add(new UserVnListViewModelCollection
            //    {
            //        UserVnListModel = new UserVnListModel
            //            {
            //                VnListItem = vnName
            //            }
            //    });

            //}
            UserVnListCollection.AddRange(VnNameList);

            Application.Current.Dispatcher.Invoke(() =>
            {
                ProgbarValue = Convert.ToDouble(100);
            });
            

        }


        private async Task BindUserVnExecute()
        {
            //Clear previous entries when switching
            UserVnListModel.StatusInfo = "";
            UserVnListModel.VoteInfo = "";
            UserVnListModel.VnImage = null;

            var selectedVnId = VnIdList[ListboxSelectedIndex];            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null,null));
            if (responseCode == -1)
            { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }


            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn details (id = " + selectedVnId + " )"));
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
                    if (responseCode == 1)
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                    DetailsRootObject detailsInformation = JsonConvert.DeserializeObject<DetailsRootObject>(conn.JsonResponse); //deserialize it            


                    if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\vnlist\"))
                    {
                        Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\vnlist\");
                    }


                    //binds the image to the UI. if it doesn't exist, it downloads it. Uses extensionless files for NSFW images
                    if (NsfwList[ListboxSelectedIndex] == "True")
                    {
                        if (StaticClass.NsfwEnabled == false)
                        {
                            UserVnListModel.VnImage = StaticClass.CurrentDirectory + @"\res\nsfw\cover.jpg";
                        }
                        else
                        {
                            string vnListImage = StaticClass.CurrentDirectory + @"\data\vnlist\" + selectedVnId;
                            if (!File.Exists(vnListImage))
                            {
                                WebClient client = new WebClient();
                                client.DownloadFile(new Uri(detailsInformation.items[0].image), StaticClass.CurrentDirectory + @"\data\vnlist\" + selectedVnId);
                            }
                            UserVnListModel.VnImage = vnListImage;
                        }


                    }
                    else if (NsfwList[ListboxSelectedIndex] == "False")
                    {
                        string vnListImage = StaticClass.CurrentDirectory + @"\data\vnlist\" + selectedVnId + ".jpg";


                        if (!File.Exists(vnListImage))
                        {
                            WebClient client = new WebClient();
                            client.DownloadFile(new Uri(detailsInformation.items[0].image), StaticClass.CurrentDirectory + @"\data\vnlist\" + selectedVnId + ".jpg");
                        }
                        UserVnListModel.VnImage = vnListImage;

                    }


                    //goes through the votelist, and if the selected vn is on there, sets the vote textbox to the vote
                    foreach (var item in UserVoteList)
                    {
                        if (item.vn == selectedVnId)
                        {
                            double d = (double)item.vote / 10;
                            UserVnListModel.VoteInfo = d.ToString();
                            break;
                            //voteTextBlock.Text = d.ToString();
                        }
                    }


                    if (UserVnListItem[ListboxSelectedIndex].status.ToString() == "0")
                    {
                        UserVnListModel.StatusInfo = "Unknown";
                        //statusTextBlock.Text = "Unknown";
                    }
                    else if (UserVnListItem[ListboxSelectedIndex].status.ToString() == "1")
                    {
                        UserVnListModel.StatusInfo = "Playing";
                    }
                    else if (UserVnListItem[ListboxSelectedIndex].status.ToString() == "2")
                    {
                        UserVnListModel.StatusInfo = "Finished";
                    }
                    else if (UserVnListItem[ListboxSelectedIndex].status.ToString() == "3")
                    {
                        UserVnListModel.StatusInfo = "Stalled";
                    }
                    else if (UserVnListItem[ListboxSelectedIndex].status.ToString() == "4")
                    {
                        UserVnListModel.StatusInfo = "Dropped";
                    }
                    //stopwatch.Stop();
                    //MessageBox.Show("seconds: " + stopwatch.Elapsed.Seconds + "ms: " + stopwatch.Elapsed.Milliseconds);

                    Thread.Sleep(300);//do this so the userr doesn't try to download too many screenshots in too short a period of time
                    await conn.Close();
                    break;
                }
            }

            


            
        }

        private async Task UpdateUserVnListExecute(object parameter)
        {
            var passwordContainer = parameter as IHavePassword;
            SecureString passwordSecure = null;
            if (passwordContainer != null)
            {
                passwordSecure = passwordContainer.Password;
                // ConvertToUnsecureString(passwordSecure);
            }
            
            
            if (VnIdList[ListboxSelectedIndex] == -1)
            {
                MessageBox.Show("You didn't select a vn to update", "No Vn Selected");
                return;
            }

            if (!Regex.IsMatch(UserVnListModel.UpdateVote, @"^([1-9]|[1-9][.][1-9]|10)$"))//must match either 1-9, 1.1-9.9, or 10
            {
                if (UserVnListModel.UpdateVote != "")
                {
                    MessageBox.Show(
                    "Number must be greater than 1.0 and less than or equual to 10\n The number can only have one decimal",
                    "Invalid number entered");
                    return;
                }

            }

            var conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(UserVnListModel.Username, ConvertToUnsecureString(passwordSecure)));
            if (responseCode == -1)
            { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }

            short status = new short();
            if (UserVnListModel.StatusComboBoxIndex != -1)
            {
                if (UserVnListModel.StatusComboBoxIndex == 0)
                {
                    status = 0;
                }

                if (UserVnListModel.StatusComboBoxIndex == 1)
                {
                    status = 1;
                }

                if (UserVnListModel.StatusComboBoxIndex == 2)
                {
                    status = 2;
                }

                if (UserVnListModel.StatusComboBoxIndex == 3)
                {
                    status = 3;
                }
                if (UserVnListModel.StatusComboBoxIndex == 4)
                {
                    status = 4;
                }
            }


            string query = null;
            //update status only
            if (status > 0 && UserVnListModel.UpdateNote == "" && UserVnListModel.ClearNote == false && UserVnListModel.ClearStatus == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"status\":\"" + status + "\"}";
            }

            //update status and clear notes
            if (status > 0 && UserVnListModel.ClearNote == true && UserVnListModel.ClearStatus == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"status\":\"" + status + "\",\"notes\":\"" + "" + "\"}";
            }

            //add note only
            if (UserVnListModel.UpdateNote != "" && UserVnListModel.ClearNote == false && UserVnListModel.ClearStatus == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"notes\":\"" + UserVnListModel.UpdateNote + "\"}";
            }

            //clear notes only
            if (UserVnListModel.ClearNote == true && UserVnListModel.ClearStatus == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"notes\":\"" + "" + "\"}";
            }

            //clear status and add notes
            if (UserVnListModel.ClearStatus == true && UserVnListModel.UpdateNote != "" && UserVnListModel.ClearNote == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"status\":\"\",\"notes\":\"" + UserVnListModel.UpdateNote + "\"}";
            }

            //clear status and clear notes
            if (UserVnListModel.ClearStatus == true && UserVnListModel.ClearNote == true)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"status\":\"\",\"notes\":\"" + "" + "\"}";
            }

            //add status and notes
            if (status > 0 && UserVnListModel.ClearStatus == false && UserVnListModel.UpdateNote != "" && UserVnListModel.ClearNote == false)
            {
                query = "set vnlist " + VnIdList[ListboxSelectedIndex] + " {\"status\":\"" + status + "\",\"notes\":\"" + UserVnListModel.UpdateNote + "\"}";
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query(query));
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
                    if (responseCode == -1)
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }


                    if (UserVnListModel.UpdateVote != "")
                    {
                        double voteDouble = Convert.ToDouble(UserVnListModel.UpdateVote);
                        int vote = Convert.ToInt32(voteDouble * 10);

                        while (true)
                        {
                            responseCode = Convert.ToInt32(await conn.Query("set votelist " + VnIdList[ListboxSelectedIndex] + " {\"vote\":\"" + vote + "\"}"));
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
                                //deserialize
                                break;
                            }
                        }
                    }


                    if (UserVnListModel.ClearVote == true)
                    {

                        while (true)
                        {
                            responseCode = Convert.ToInt32(await conn.Query("set votelist " + VnIdList[ListboxSelectedIndex]));
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
                                //deserialize
                                await conn.Close();
                                break;
                            }
                        }
                    }
                    await conn.Close();
                    break;
                }
            }


            

            
                
            


        }



        public ICommand GetUserVnListCommand
        {
            get { return new AwaitableDelegateCommand<object>(GetUserVnListExecute); }
        }

        public ICommand BindUserVnCommand
        {
            get { return new AwaitableDelegateCommand(BindUserVnExecute);}
        }


        public ICommand UpdateUserVnListCommand
        {
            get { return new AwaitableDelegateCommand<object>(UpdateUserVnListExecute);}
        }
        #endregion



       

        private async Task<List<string>>  GetVnListVoteList(string username, SecureString passwordSecure)
        {
            try
            {
                List<string> VnNameList = new List<string>();
                Thread.Sleep(300);


                var conn = new Connection();
                await conn.Open();

                int responseCode = Convert.ToInt32(await conn.Login(username, ConvertToUnsecureString(passwordSecure)));
                if (responseCode == -1)
                { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }

                #region gets the user's list of vns, then adds them to a list
                int vnlistPage = 1;
                while (true)
                {
                    responseCode = Convert.ToInt32(await conn.Query("get vnlist basic (uid=0)" + "{\"results\":100,\"page\":" + vnlistPage + "}"));

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
                        var uservnlist = JsonConvert.DeserializeObject<UserVnListRootObject>(conn.JsonResponse);
                        UserVnListItem = uservnlist.items;


                        if (vnlistPage > 1)
                        {
                            UserVnListItem.AddRange(uservnlist.items);
                        }

                        if (uservnlist.more == false)
                        {
                            break;
                        }
                        vnlistPage++;
                    }

                    
                }
                #endregion
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgbarValue = 12.5;
                });
                #region get the user's votes, then adds them to a list
                int votelistPage = 1;
                while (true)
                {
                    responseCode = Convert.ToInt32(await conn.Query("get votelist basic (uid=0)" + "{\"results\":100,\"page\":" + vnlistPage + "}"));

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
                        var uservotelist = JsonConvert.DeserializeObject<VoteListRootObject>(conn.JsonResponse);
                        UserVoteList = uservotelist.items;

                        if (votelistPage > 1)
                        {
                            UserVoteList.AddRange(uservotelist.items);
                        }

                        if (uservotelist.more == false)
                        {
                            break;
                        }

                        votelistPage++;
                    }                   
                }
                #endregion
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgbarValue = Convert.ToDouble(25);
                });
                List<int> currentVnList = new List<int>();
                foreach (var id in UserVnListItem)
                {
                    VnIdList.Add(id.vn);
                    currentVnList.Add(id.vn);
                }

                #region creates a list of IDs from the sql table
                List<int> SqlVnList = new List<int>();
                const string connectionString = @"Data Source=|DataDirectory|\Database.sdf";
                var con = new SqlCeConnection(connectionString);
                con.Open();
                SqlCeCommand vnlistsqlcmd = new SqlCeCommand("SELECT * FROM VnList", con);
                SqlCeDataReader reader = vnlistsqlcmd.ExecuteReader();
                while (reader.Read())
                {
                    SqlVnList.Add((int)reader["VnId"]);
                }
                #endregion

                #region delete a row if the sql table has an id that is no longer in the user's vn list
                foreach (var id in SqlVnList)
                {
                    if (!VnIdList.Contains(id))
                    {
                        var deleteVncmd = new SqlCeCommand("DELETE FROM VnList WHERE VnId=@VnId", con);
                        deleteVncmd.Parameters.AddWithValue("@VnId", id);
                        deleteVncmd.ExecuteNonQuery();
                    }
                }
                #endregion
                await conn.Close();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgbarValue = 37.5;
                });


                VnNameList = GetVnName(currentVnList, SqlVnList, connectionString, username, passwordSecure).Result;
                //var foo = GetVnName(currentVnList, SqlVnList, connectionString, username, passwordSecure).Result;



                #region for items not in SQLdb, add items to SQLdb
                var cmd = new SqlCeCommand("INSERT INTO VnList(VnId, Title, NSFW, Vote, Status, Note) VALUES(@VnId, @Title, @NSFW, @Vote, @Status, @Note)", con) { Connection = con };
                for (int i = 0; i < VnIdList.Count; i++)
                {
                    if (!SqlVnList.Contains(VnIdList[i]))
                    {
                        cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(VnIdList[i]));
                        cmd.Parameters.AddWithValue("@Title", CheckForDbNull(VnNameList[i]));
                        cmd.Parameters.AddWithValue("@NSFW", CheckForDbNull(NsfwList[i]));
                        var userVote = UserVoteList.SingleOrDefault(item => item.vn == VnIdList[i]);
                        cmd.Parameters.AddWithValue("@Vote", CheckForDbNull(userVote != null ? userVote.vote.ToString() : ""));
                        cmd.Parameters.AddWithValue("@Status", CheckForDbNull(UserVnListItem[i].status));
                        cmd.Parameters.AddWithValue("@Note", CheckForDbNull(UserVnListItem[i].notes));
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }
                #endregion

                return VnNameList;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
        }

        private async Task<List<string>> GetVnName(List<int> currentVnList, List<int> SqlVnList, string connectionString, string username, SecureString passwordSecure)
        {
            try
            {
                List<string> vnNameList = new List<string>();
                var conn = new Connection();
                await conn.Open();
                int responseCode = Convert.ToInt32(await conn.Login(null, null));
                if (responseCode == 1)
                {
                    MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                }

                #region create a string of ids from currentVnList
                string idString = null;
                foreach (var id in currentVnList)
                {
                    idString = idString + id.ToString() + ",";
                }
                idString = idString.TrimEnd(',');
                #endregion

                #region get basic info from list of VNs

                while (true)
                {
                    responseCode = Convert.ToInt32(await conn.Query("get vn basic (id = [" + idString + "] )" + "{\"results\":25,\"page\":" + 1 + "}"));
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
                        BasicRootObject basicInformation = JsonConvert.DeserializeObject<BasicRootObject>(conn.JsonResponse); //deserialize it
                        _basicItem = basicInformation.items;
                #endregion
                        List<List<string>> sqlList = new List<List<string>>();
                        await conn.Close();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProgbarValue = Convert.ToDouble(50);
                        });

                        //get list of items from sql db, or from api if missing
                        sqlList = AddItem(passwordSecure, UserVnListItem).Result;

                        await conn.Open();
                        await conn.Login(null, null);

                        var con = new SqlCeConnection(connectionString);
                        int page = 1;
                        int counter = 0;
                        while (true)
                        {
                            if (currentVnList.Count == 0)
                            {
                                break;
                            }

                            #region page1

                            if (page == 1)
                            {
                                for (int i = 0; i < _basicItem.Count; i++)
                                {
                                    if (!SqlVnList.Contains(_basicItem[i].id))//if the sql db doesn't contain the name, add the name from API to the name lisst
                                    {



                                        if (_basicItem != null)
                                        {
                                            vnNameList.Add(_basicItem[i].title);
                                        }

                                    }
                                    else
                                    {
                                        //var con = new SqlCeConnection(connectionString);
                                        con.Open();
                                        var sqlnamecmd = new SqlCeCommand("SELECT * FROM VnList WHERE VnId=@VnId", con);
                                        sqlnamecmd.Parameters.AddWithValue("@VnId", _basicItem[i].id);
                                        SqlCeDataReader nameReader = sqlnamecmd.ExecuteReader();
                                        while (nameReader.Read())
                                        {
                                            vnNameList.Add((string)nameReader["Title"]);
                                        }
                                        con.Close();
                                    }

                                    con.Open();

                                    #region move outside forloop
                                    foreach (var listItem in sqlList)//updates the entries
                                    {
                                        if (listItem[0].Equals(_basicItem[i].id.ToString()))
                                        {

                                            var cmd = new SqlCeCommand("UPDATE VnList SET NSFW=@NSFW, Status=@Status, Note=@Note, Vote=@Vote WHERE VnId=@VnId", con) { Connection = con };
                                            cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[i].id));
                                            cmd.Parameters.AddWithValue("@Status", CheckForDbNull(sqlList[counter][1]).ToString());
                                            cmd.Parameters.AddWithValue("@Note", CheckForDbNull(sqlList[counter][2]).ToString());
                                            cmd.Parameters.AddWithValue("@Vote", CheckForDbNull(sqlList[counter][3]).ToString());
                                            cmd.Parameters.AddWithValue("@NSFW", CheckForDbNull(NsfwList[counter]));
                                            cmd.ExecuteNonQuery();
                                            cmd.Parameters.Clear();
                                            counter++;
                                        }
                                    }
                                    con.Close();
                                    #endregion
                                }

                                currentVnList.RemoveRange(0, _basicItem.Count);
                                sqlList.RemoveRange(0, _basicItem.Count);
                                idString = null;
                                if (currentVnList.Count > 0)
                                { }
                                if (currentVnList.Count > 25)
                                {
                                    for (int i = 0; i < 25; i++)
                                    {
                                        idString = idString + currentVnList[i] + ',';
                                    }
                                }
                                else
                                {
                                    foreach (var id in currentVnList)
                                    {
                                        idString = idString + id + ',';
                                    }
                                }
                                idString = idString.TrimEnd(',');

                            }
                            #endregion

                            #region page 2+
                            if (page > 1)
                            {
                                while (true)
                                {
                                    responseCode = Convert.ToInt32(await conn.Query("get vn basic (id = [" + idString + "] )" + "{\"results\":25,\"page\":" + 1 + "}"));
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
                                        basicInformation = JsonConvert.DeserializeObject<BasicRootObject>(conn.JsonResponse); //deserialize it
                                _basicItem = basicInformation.items;

                                for (int i = 0; i < _basicItem.Count; i++)
                                {

                                    if (!SqlVnList.Contains(_basicItem[i].id))
                                    {


                                        if (_basicItem != null)
                                        {
                                            vnNameList.Add(_basicItem[i].title);
                                        }

                                    }
                                    else
                                    {
                                        //var con = new SqlCeConnection(connectionString);
                                        con.Open();
                                        var sqlnamecmd = new SqlCeCommand("SELECT * FROM VnList WHERE VnId=@VnId", con);
                                        sqlnamecmd.Parameters.AddWithValue("@VnId", _basicItem[i].id);
                                        SqlCeDataReader nameReader = sqlnamecmd.ExecuteReader();
                                        while (nameReader.Read())
                                        {
                                            vnNameList.Add((string)nameReader["Title"]);
                                        }
                                        con.Close();
                                    }

                                    con.Open();




                                    //foreach (var listItem in sqlList)
                                    //{

                                    //    if (listItem[0].Equals(_basicItem[i].id.ToString()))
                                    //    {

                                    //        var cmd = new SqlCeCommand("UPDATE VnList SET NSFW=@NSFW, Status=@Status, Note=@Note, Vote=@Vote WHERE VnId=@VnId", con) { Connection = con };
                                    //        cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[i].id));
                                    //        cmd.Parameters.AddWithValue("@Status", CheckForDbNull(sqlList[counter][1]).ToString());
                                    //        cmd.Parameters.AddWithValue("@Note", CheckForDbNull(sqlList[counter][2]).ToString());
                                    //        cmd.Parameters.AddWithValue("@Vote", CheckForDbNull(sqlList[counter][3]).ToString());
                                    //        cmd.Parameters.AddWithValue("@NSFW", CheckForDbNull(sqlList[counter][4]));
                                    //        cmd.ExecuteNonQuery();
                                    //        cmd.Parameters.Clear();
                                    //        counter++;
                                    //    }
                                    //}
                                    con.Close();






                                }
                                //I think it gives the error on the second pass here, when there are the two remaining items in the sqlist
                                con.Open();
                                if (sqlList.Count > 25)
                                {
                                    for (int j = 0; j < 25; j++)
                                    {
                                        var cmd = new SqlCeCommand("UPDATE VnList SET NSFW=@NSFW, Status=@Status, Note=@Note, Vote=@Vote WHERE VnId=@VnId", con) { Connection = con };
                                        cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[j].id));
                                        cmd.Parameters.AddWithValue("@Status", CheckForDbNull(sqlList[j][1]).ToString());
                                        cmd.Parameters.AddWithValue("@Note", CheckForDbNull(sqlList[j][2]).ToString());
                                        cmd.Parameters.AddWithValue("@Vote", CheckForDbNull(sqlList[j][3]).ToString());
                                        cmd.Parameters.AddWithValue("@NSFW", CheckForDbNull(NsfwList[j]));
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < sqlList.Count; j++)
                                    {
                                        var cmd = new SqlCeCommand("UPDATE VnList SET NSFW=@NSFW, Status=@Status, Note=@Note, Vote=@Vote WHERE VnId=@VnId", con) { Connection = con };
                                        cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[j].id));
                                        cmd.Parameters.AddWithValue("@Status", CheckForDbNull(sqlList[j][1]).ToString());
                                        cmd.Parameters.AddWithValue("@Note", CheckForDbNull(sqlList[j][2]).ToString());
                                        cmd.Parameters.AddWithValue("@Vote", CheckForDbNull(sqlList[j][3]).ToString());
                                        cmd.Parameters.AddWithValue("@NSFW", CheckForDbNull(NsfwList[j]));
                                        cmd.ExecuteNonQuery();
                                        cmd.Parameters.Clear();
                                    }
                                }
                                con.Close();

                                currentVnList.RemoveRange(0, _basicItem.Count);
                                sqlList.RemoveRange(0, _basicItem.Count);
                                idString = null;
                                if (currentVnList.Count > 25)
                                {
                                    for (int i = 0; i < 25; i++)
                                    {
                                        idString = idString + currentVnList[i] + ',';
                                    }
                                }

                                if (currentVnList.Count == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    foreach (var id in currentVnList)
                                    {
                                        idString = idString + id + ',';
                                    }
                                }
                                idString = idString.TrimEnd(',');
                                        break;
                                    }
                                }
                                
                                
                               

                                

                            }

                            #endregion

                            page++;

                        }

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ProgbarValue = 87.5;
                        });



                        return vnNameList;

                        break;
                    }
                }
                
                

                
                
                
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
            
            
        }

        private async Task<List<List<string>>> AddItem(SecureString passwordSecure, List<JSON.UserVnList> userVnLists)
        {
            try
            {
                List<List<string>> sqlList = new List<List<string>>();
                List<List<string>> currentSqlList = new List<List<string>>();

                List<JSON.UserVnList> UserVnList= new List<UserVnList>();
                UserVnList.AddRange(userVnLists);


                #region get all items from db vnlist
                const string connectionString = @"Data Source=|DataDirectory|\Database.sdf";
                var con = new SqlCeConnection(connectionString);
                con.Open();
                SqlCeCommand vnlistsqlcmd = new SqlCeCommand("SELECT * FROM VnList", con);
                SqlCeDataReader reader = vnlistsqlcmd.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    int id = 0;
                    string status = "";
                    string vote = "";
                    string notes = "";
                    string nsfw = "";
                    id = (int)reader["VnId"];

                    if (reader["Status"] != DBNull.Value)
                    {
                        status = (string)reader["Status"];
                    }

                    if (reader["Vote"] != DBNull.Value)
                    {
                        vote = (string)reader["Vote"];
                    }

                    if (reader["Note"] != DBNull.Value)
                    {
                        notes = (string)reader["Note"];
                    }

                    if (reader["NSFW"] != DBNull.Value)
                    {
                        nsfw = (string)reader["NSFW"];
                    }
                    var innerList = new List<string>();
                    sqlList.Add(innerList);
                    sqlList[count].Add(id.ToString());
                    sqlList[count].Add(status.ToString());
                    sqlList[count].Add(notes.ToString()??"");
                    sqlList[count].Add(vote);
                    sqlList[count].Add(nsfw);
                    NsfwList.Add(nsfw);
                    currentSqlList.Add(innerList);
                    count++;

                }
                con.Close();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProgbarValue = 62.5;
                });
                #endregion

                #region get ids NOT in sql database
                List<int> nonDbIds = new List<int>();
                while (true)
                {
                    List<int> sqlIds = sqlList.Select(dbItem => Convert.ToInt32(dbItem[0])).ToList();
                    List<int> UserIds = UserVnListItem.Select(userVn => userVn.vn).ToList();
                    nonDbIds = UserIds.Except(sqlIds).ToList();
                    break;
                }
                #endregion

                for (int i = 0; i < nonDbIds.Count; i++)//add list to current and sql list
                {
                    var innerList = new List<string>();
                    sqlList.Add(innerList);
                    currentSqlList.Add(innerList);
                    
                    //idList = idList + nonDbIds[i] + ',';
                }
                //idList = idList.TrimEnd(',');

                var conn = new Connection();
                await conn.Open();
                int responseCode = Convert.ToInt32(await conn.Login(null, null));
                if (responseCode == -1)
                { MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK); }








                int CurrSqlList25more = 0;//index keeping track of how many times currentsqllist is >25(0=0,25=1,50=2
                while (true)
                {

                    
                    if (nonDbIds.Count == 0)
                    {
                        break;
                    }
                    var idList = "";
                    for (int i = 0; i < nonDbIds.Count; i++)//make a comma seperated string of the ids not in the database
                    {
                        idList = idList + nonDbIds[i] + ',';
                    }
                    idList = idList.TrimEnd(',');

                    while (true)
                    {
                        responseCode = Convert.ToInt32(await conn.Query("get vn details (id = [" + idList + "] )" + "{\"results\":25,\"page\":1}"));
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
                            DetailsRootObject detailsInformation = JsonConvert.DeserializeObject<DetailsRootObject>(conn.JsonResponse); //deserialize it
                            _detailsItem = detailsInformation.items;



                            if (currentSqlList.Count > 25)//if active sql count is more than 25(API returns only 25 results, so needd to be limited to that)
                            {
                                int counter = -1;
                                int j = 0;
                                for (int i = CurrSqlList25more; i < (CurrSqlList25more + 25); i++)
                                {
                                    if (sqlList[i].Count == 0)//activates if nothing in db
                                    {
                                        counter++;
                                        if (nonDbIds.Count == 0) { break; }
                                        var index = userVnLists.FindIndex(idx => idx.vn == nonDbIds[0]);// breaks on reverse
                                        sqlList[i].Add(userVnLists[index].vn.ToString());   //is adding to the top of the list. Needs to find the empty list, and add to that
                                        sqlList[i].Add(userVnLists[index].status.ToString());
                                        sqlList[i].Add(userVnLists[index].notes != null ? UserVnListItem[i].notes.ToString() : "");//sets notes if not null, otherwise set ""
                                        NsfwList.Add(_detailsItem[j].image_nsfw.ToString());

                                        var userVote = UserVoteList.SingleOrDefault(v => v.vn == userVnLists[index].vn);//gets the vote of the vn using index from votelist, otherwise returns null
                                        sqlList[i].Add(userVote != null ? userVote.vote.ToString() : "");//if not null, add uservote to list, otherwise add empty string
                                        sqlList[i].Add(_detailsItem[j].image);

                                        nonDbIds.RemoveAt(0);
                                        j = j + 1;
                                    }
                                }
                                CurrSqlList25more = CurrSqlList25more + 25;
                            }


                            else if (currentSqlList.Count <= 25)
                            {
                                int counter = -1;
                                int j = 0;
                                int nondbCount = nonDbIds.Count;
                                for (int i = CurrSqlList25more; i < (CurrSqlList25more + nondbCount); i++)
                                {
                                    if (sqlList[i].Count == 0)//activates if nothing in db
                                    {
                                        counter++;
                                        if (nonDbIds.Count == 0) { break; }
                                        var index = userVnLists.FindIndex(idx => idx.vn == nonDbIds[0]);// breaks on reverse
                                        sqlList[i].Add(userVnLists[index].vn.ToString());   //is adding to the top of the list. Needs to find the empty list, and add to that
                                        sqlList[i].Add(userVnLists[index].status.ToString());
                                        sqlList[i].Add(userVnLists[index].notes != null ? UserVnListItem[i].notes.ToString() : "");//sets notes if not null, otherwise set ""
                                        NsfwList.Add(_detailsItem[j].image_nsfw.ToString());

                                        var userVote = UserVoteList.SingleOrDefault(v => v.vn == userVnLists[index].vn);//gets the vote of the vn using index from votelist, otherwise returns null
                                        sqlList[i].Add(userVote != null ? userVote.vote.ToString() : "");//if not null, add uservote to list, otherwise add empty string
                                        sqlList[i].Add(_detailsItem[j].image);

                                        nonDbIds.RemoveAt(0);
                                        j = j + 1;
                                    }
                                }
                            }




                            if (currentSqlList.Count > 25)
                            {
                                currentSqlList.RemoveRange(0, 25);//remove 25 entries from the current sql list
                            }
                            else if (currentSqlList.Count <= 25)
                            {
                                currentSqlList.RemoveRange(0, currentSqlList.Count);
                            }
                            break;
                        }
                    }

                   
                    

                }

                await conn.Close();
                return sqlList;
            }
            catch (Exception exception)
            {
               
                Console.WriteLine(exception);
                throw;
            }
            
            
            
        }




        public object CheckForDbNull(object value)       //this forces entries that are null to work in the database
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }





        private string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                return string.Empty;
            }

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        #region password examples
        //private void Login(object parameter)
        //{
        //    var passwordContainer = parameter as IHavePassword;
        //    if (passwordContainer != null)
        //    {
        //        var secureString = passwordContainer.Password;
        //        //NEED to dispose of this asap
        //        var PasswordInVM = ConvertToUnsecureString(secureString);
        //    }
        //}

        

        #endregion


    }

   

}
