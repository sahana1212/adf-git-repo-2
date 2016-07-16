using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using Visual_Novel_Manager.ConnectionClient;
using Visual_Novel_Manager.CustomClasses;
using Visual_Novel_Manager.JSON;
using Visual_Novel_Manager.Model;

namespace Visual_Novel_Manager.ViewModel
{
    public class UserVnListViewModel: INotifyPropertyChanged
    {
        #region property changed, ConvertToUnsecureString, and CheckforDbNull
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
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



        //this forces entries that are null to work in the database
        public object CheckForDbNull(object value)       
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }
        #endregion

        #region constructor and setup

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

        public UserVnListViewModel()
        {
            _userVnListModel = new UserVnListModel();
        }

        #endregion


        private List<BasicItem> _basicItem;
        private List<DetailsItem> _detailsItem;
        private static readonly List<int> VnIdList = new List<int>();
        private static readonly List<string> NsfwList = new List<string>();
        private List<JSON.UserVnList> _userVnListItem;
        private List<JSON.UserVoteList> _userVoteList;




        private SecureString EncryptedPassword { get; set; }
        private string Username { get; set; }


        #region LoginArea--Commands and return methods



        //put commands in here
        #region commands
        private RelayCommand<SecureString> _setEncPassCommand;
        public ICommand SetEncPassCommand
        {
            get
            {
                _setEncPassCommand = new RelayCommand<SecureString>(SetEncryptedPassword);
                return _setEncPassCommand;
            }
        }

        public ICommand GetUserVnListCommand { get { return new AwaitableDelegateCommand(GetUserVnList);} }


        #endregion


        //put non return methods here
        #region methods
        private void SetEncryptedPassword(SecureString encryptedPassword)
        {
            EncryptedPassword = encryptedPassword;
        }

        private async Task GetUserVnList()
        {

            var conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(UserVnListModel.Username, ConvertToUnsecureString(EncryptedPassword)));
            if (responseCode == -1)
            {
                await conn.Close();
                return;
            }
            else if (responseCode == 0)
            {
                await conn.Close();
                Username = UserVnListModel.Username;
            }
            
            //put in async Task.Run()
            await Task.Run(() =>
            {


                var foo = GetVnListInfo().Result;
            });


        }

        #endregion



        //put return only methods here
        #region return methods

        private async Task<Tuple<List<int>, List<int>>> GetVnListInfo()
        {
            try
            {
                var conn = new Connection();
                await conn.Open();
                int responseCode = Convert.ToInt32(await conn.Login(Username, ConvertToUnsecureString(EncryptedPassword)));
                if (responseCode != -1)
                {
                    
                    //gets the user's vn list, and adds them to a list
                    #region gets the user's vn list, and adds them to a list
                    int vnlistPage = 1;
                    while (true)
                    {
                        responseCode = Convert.ToInt32(await conn.Query("get vnlist basic (uid=0)" + "{\"results\":100,\"page\":" + vnlistPage + "}"));
                        if (responseCode == 0)
                        {
                            var uservnlist = JsonConvert.DeserializeObject<UserVnListRootObject>(conn.JsonResponse);
                            _userVnListItem = uservnlist.items;


                            if (vnlistPage > 1)
                            {
                                _userVnListItem.AddRange(uservnlist.items);
                            }

                            if (uservnlist.more == false)
                            {
                                break;
                            }
                            vnlistPage++;
                        }

                        else if (responseCode == -1)
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

                    }
                    #endregion

                    //get the user's votes, then adds them to a list
                    #region get the user's votes, then adds them to a list
                    int votelistPage = 1;
                    while (true)
                    {
                        responseCode = Convert.ToInt32(await conn.Query("get votelist basic (uid=0)" + "{\"results\":100,\"page\":" + vnlistPage + "}"));
                        if (responseCode == 0)
                        {
                            var uservotelist = JsonConvert.DeserializeObject<VoteListRootObject>(conn.JsonResponse);
                            _userVoteList = uservotelist.items;

                            if (votelistPage > 1)
                            {
                                _userVoteList.AddRange(uservotelist.items);
                            }

                            if (uservotelist.more == false)
                            {
                                break;
                            }

                            votelistPage++;
                        }
                        else if (responseCode == -1)
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

                    }

                    #endregion

                    List<int> currentVnList = new List<int>();
                    foreach (var id in _userVnListItem)
                    {
                        VnIdList.Add(id.vn);
                        currentVnList.Add(id.vn);
                    }

                    //creates a list of IDs from the sql table
                    #region creates a list of IDs from the sql table
                    List<int> SqlVnList = new List<int>();
                    using (SQLiteConnection dbConnection = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                    {
                        dbConnection.Open();
                        SQLiteCommand command = new SQLiteCommand("SELECT VnId FROM NovelPath", dbConnection);
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            SqlVnList.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                    }
                    #endregion

                    //delete a row if the sql table has an id that is no longer in the user's vn list
                    #region delete a row if the sql table has an id that is no longer in the user's vn list
                    using (SQLiteConnection dbConnection = new SQLiteConnection(@"Data Source=|DataDirectory|\Database.db"))
                    {
                        dbConnection.Open();
                        foreach (var id in SqlVnList)
                        {
                            if (!VnIdList.Contains(id))
                            {
                                SQLiteCommand deleteVnCommand= new SQLiteCommand("DELETE FROM VnList WHERE VnId=@VnId",dbConnection);
                                deleteVnCommand.Parameters.AddWithValue("@VnId", id);
                                deleteVnCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    #endregion


                    var vnListTuple = Tuple.Create(currentVnList, SqlVnList);
                    return vnListTuple;
                }
                else
                {
                    MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
                    return null;
                }
                
                return null;
            }
            catch (Exception)
            {

                throw;
            }



        }
        #endregion







        #endregion
        //end of login area




        #region commands

        #endregion

        #region methods

        #endregion

        #region return methods

        #endregion

    }
}
