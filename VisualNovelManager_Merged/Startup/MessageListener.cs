using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace VisualNovelManager.Startup
{
    public class MessageListener :DependencyObject
    {
        #region setup
        /// <summary>
        /// 
        /// </summary>
        private static MessageListener mInstance;

        /// <summary>
        /// 
        /// </summary>
        private MessageListener()
        {

        }

        /// <summary>
        /// Get MessageListener instance
        /// </summary>
        public static MessageListener Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MessageListener();
                return mInstance;
            }
        }
        #endregion


        #region checklib
        public void RecieveCheckLibs(string lib)
        {
            CheckLibs = lib;
            DispatchHelper.DoEvents();
        }

        public string CheckLibs
        {
            get { return (string)GetValue(CheckLibsProperty); }
            set { SetValue(CheckLibsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckLibs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckLibsProperty =
            DependencyProperty.Register("CheckLibs", typeof(string), typeof(MessageListener), new PropertyMetadata(""));

        #endregion


        #region checkdb
        public void RecieveCheckDb(string db)
        {
            CheckDb = db;
            DispatchHelper.DoEvents();
        }


        public string CheckDb
        {
            get { return (string)GetValue(CheckDbProperty); }
            set { SetValue(CheckDbProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckDb.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckDbProperty =
            DependencyProperty.Register("CheckDb", typeof(string), typeof(MessageListener), new PropertyMetadata(""));
        #endregion


        #region check file structure
        public void RecieveCheckFileStructure(string file)
        {
            CheckFileStructure = file;
            DispatchHelper.DoEvents();
        }

        public string CheckFileStructure
        {
            get { return (string)GetValue(CheckFileStructureProperty); }
            set { SetValue(CheckFileStructureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckFileStructure.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckFileStructureProperty =
            DependencyProperty.Register("CheckFileStructure", typeof(string), typeof(MessageListener), new PropertyMetadata(""));
        #endregion

        #region load tags and traits
        public void RecieveLoadTagsandTraits(string tagsandtraits)
        {
            LoadTagsandTraits = tagsandtraits;
            DispatchHelper.DoEvents();
        }


        public string LoadTagsandTraits
        {
            get { return (string)GetValue(LoadTagsandTraitsProperty); }
            set { SetValue(LoadTagsandTraitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadTagsandTraits.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadTagsandTraitsProperty =
            DependencyProperty.Register("LoadTagsandTraits", typeof(string), typeof(MessageListener), new PropertyMetadata(""));
        #endregion




        #region check config
        public void RecieveCheckConfig(string config)
        {
            CheckConfig = config;
            DispatchHelper.DoEvents();
        }


        public string CheckConfig
        {
            get { return (string)GetValue(CheckConfigProperty); }
            set { SetValue(CheckConfigProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckConfig.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckConfigProperty =
            DependencyProperty.Register("CheckConfig", typeof(string), typeof(MessageListener), new PropertyMetadata(""));
        #endregion

        #region warning

        public void RecieveCheckWarning(string warning)
        {
            CheckWarning = warning;
            DispatchHelper.DoEvents();
        }


        public string CheckWarning
        {
            get { return (string)GetValue(CheckWarningProperty); }
            set { SetValue(CheckWarningProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckWarning.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckWarningProperty =
            DependencyProperty.Register("CheckWarning", typeof(string), typeof(MessageListener), new PropertyMetadata(""));


        #endregion

    }
}
