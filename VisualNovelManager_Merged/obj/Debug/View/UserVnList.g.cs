﻿#pragma checksum "..\..\..\View\UserVnList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6F168F759BDFEF70EE9F68E710090B12"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using VisualNovelManager.CustomClasses;
using VisualNovelManager.ViewModel;


namespace VisualNovelManager.View {
    
    
    /// <summary>
    /// UserVnList
    /// </summary>
    public partial class UserVnList : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 2 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal VisualNovelManager.View.UserVnList UC_UserVnList;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox VnListBox;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock voteTextBlock;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock statusTextBlock;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock releaseTextBlock;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cb_Status;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ClearStatusChk;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox VoteTextbox;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ClearVoteChk;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NotesTextBox;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ClearNotesChk;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image vnImage;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox usernameTxt;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox passwordTxt;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LoginBtn;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\View\UserVnList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar _progressBar;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VisualNovelManager;component/view/uservnlist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\UserVnList.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.UC_UserVnList = ((VisualNovelManager.View.UserVnList)(target));
            return;
            case 2:
            this.VnListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 27 "..\..\..\View\UserVnList.xaml"
            this.VnListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.VnListBox_OnSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.voteTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.statusTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.releaseTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.cb_Status = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.ClearStatusChk = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 8:
            this.VoteTextbox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.ClearVoteChk = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.NotesTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.ClearNotesChk = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            
            #line 81 "..\..\..\View\UserVnList.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Update_OnClick);
            
            #line default
            #line hidden
            return;
            case 13:
            this.vnImage = ((System.Windows.Controls.Image)(target));
            return;
            case 14:
            this.usernameTxt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 15:
            this.passwordTxt = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 16:
            this.LoginBtn = ((System.Windows.Controls.Button)(target));
            
            #line 101 "..\..\..\View\UserVnList.xaml"
            this.LoginBtn.Click += new System.Windows.RoutedEventHandler(this.LoginBtn_Click);
            
            #line default
            #line hidden
            return;
            case 17:
            this._progressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
