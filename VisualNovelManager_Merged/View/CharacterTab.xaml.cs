using System.Windows;
using System.Windows.Controls;
//my references

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for CharacterTab.xaml
    /// </summary>
    public partial class CharacterTab : UserControl
    {

        public static RichTextBox chardesc;
        public static RichTextBox traitdesc;
        
        public CharacterTab()
        {
            InitializeComponent();
            DataContext = StaticClass.CharacterViewModelStatic;
            chardesc = this.chardescription;
            traitdesc = this.traitdescription;
        }


        private void CharTraits_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void Char_Button_Click_Previous(object sender, RoutedEventArgs e)
        {
            
        }

        private void Char_Button_Click_Next(object sender, RoutedEventArgs e)
        {

        }
    }
}
