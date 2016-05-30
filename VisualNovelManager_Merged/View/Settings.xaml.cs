using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using VisualNovelManager.JSON;

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            CheckVnSelected();
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");            
            var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);           
            json.global.NsfwEnabled = NsfwCmb.SelectedIndex == 0;//sets nsfw to true if first index, false if second

            foreach (var match in json.unique)
            {
                if (match.VnId == StaticClass.Vnid)
                {
                    switch (VnSpoilerCmb.SelectedIndex)
                    {
                        case 0:
                            match.VnSpoilerLevel = 0;
                            break;
                        case 1:
                            match.VnSpoilerLevel = 1;
                            break;
                        case 2:
                            match.VnSpoilerLevel = 2;
                            break;
                    }

                    switch (CharSpoilerCmb.SelectedIndex)
                    {
                        case 0:
                            match.CharacterSpoilerLevel = 0;
                            break;
                        case 1:
                            match.CharacterSpoilerLevel = 1;
                            break;
                        case 2:
                            match.CharacterSpoilerLevel = 2;
                            break;
                    }
                }
                StaticClass.VnSpoilerLevel = match.VnSpoilerLevel;
                StaticClass.CharacterSpoilerLevel = match.CharacterSpoilerLevel;
            }
            File.WriteAllText(StaticClass.CurrentDirectory + @"\config.json", JsonConvert.SerializeObject(json));
            StaticClass.NsfwEnabled = json.global.NsfwEnabled;
            Close();
            
        }

        private void CheckVnSelected()
        {
            if (StaticClass.Vnid <= 0)
            {
                VnSpoilerCmb.IsEnabled = false;
                CharSpoilerCmb.IsEnabled = false;
            }
        }
    }
}
