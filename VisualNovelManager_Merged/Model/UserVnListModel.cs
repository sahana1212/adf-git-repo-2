using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualNovelManager.Model
{
    public class UserVnListModel :DependencyObject
    {



        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Username.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(UserVnListModel), new PropertyMetadata(""));

        

        public bool UsernameEnabled
        {
            get { return (bool)GetValue(UsernameEnabledProperty); }
            set { SetValue(UsernameEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UsernameReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UsernameEnabledProperty =
            DependencyProperty.Register("UsernameEnabled", typeof(bool), typeof(UserVnListModel), new PropertyMetadata(true));







        public bool PasswordEnabled
        {
            get { return (bool)GetValue(PasswordEnabledProperty); }
            set { SetValue(PasswordEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PasswordEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordEnabledProperty =
            DependencyProperty.Register("PasswordEnabled", typeof(bool), typeof(UserVnListModel), new PropertyMetadata(true));

        




        public bool LoginBtnEnabled
        {
            get { return (bool)GetValue(LoginBtnEnabledProperty); }
            set { SetValue(LoginBtnEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoginBtnEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoginBtnEnabledProperty =
            DependencyProperty.Register("LoginBtnEnabled", typeof(bool), typeof(UserVnListModel), new PropertyMetadata(true));

        


        public string VoteInfo
        {
            get { return (string)GetValue(VoteInfoProperty); }
            set { SetValue(VoteInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VoteInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VoteInfoProperty =
            DependencyProperty.Register("VoteInfo", typeof(string), typeof(UserVnListModel), new PropertyMetadata(""));





        public string StatusInfo
        {
            get { return (string)GetValue(StatusInfoProperty); }
            set { SetValue(StatusInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusInfoProperty =
            DependencyProperty.Register("StatusInfo", typeof(string), typeof(UserVnListModel), new PropertyMetadata(""));




        public string VnImage
        {
            get { return (string)GetValue(VnImageProperty); }
            set { SetValue(VnImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnImageProperty =
            DependencyProperty.Register("VnImage", typeof(string), typeof(UserVnListModel), new PropertyMetadata(null));


        #region update

        public int StatusComboBoxIndex
        {
            get { return (int)GetValue(StatusComboBoxIndexProperty); }
            set { SetValue(StatusComboBoxIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusComboBoxIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusComboBoxIndexProperty =
            DependencyProperty.Register("StatusComboBoxIndex", typeof(int), typeof(UserVnListModel), new PropertyMetadata(0));





        public bool ClearStatus
        {
            get { return (bool)GetValue(ClearStatusProperty); }
            set { SetValue(ClearStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearStatusProperty =
            DependencyProperty.Register("ClearStatus", typeof(bool), typeof(UserVnListModel ), new PropertyMetadata(false));



        public string UpdateVote
        {
            get { return (string)GetValue(UpdateVoteProperty); }
            set { SetValue(UpdateVoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateVote.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateVoteProperty =
            DependencyProperty.Register("UpdateVote", typeof(string), typeof(UserVnListModel), new PropertyMetadata(""));




        public bool ClearVote
        {
            get { return (bool)GetValue(ClearVoteProperty); }
            set { SetValue(ClearVoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearVote.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearVoteProperty =
            DependencyProperty.Register("ClearVote", typeof(bool), typeof(UserVnListModel), new PropertyMetadata(false));






        public string UpdateNote
        {
            get { return (string)GetValue(UpdateNoteProperty); }
            set { SetValue(UpdateNoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateNote.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateNoteProperty =
            DependencyProperty.Register("UpdateNote", typeof(string), typeof(UserVnListModel), new PropertyMetadata(""));




        public bool ClearNote
        {
            get { return (bool)GetValue(ClearNoteProperty); }
            set { SetValue(ClearNoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClearNote.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearNoteProperty =
            DependencyProperty.Register("ClearNote", typeof(bool), typeof(UserVnListModel), new PropertyMetadata(false));

        

        #endregion












    }
}
