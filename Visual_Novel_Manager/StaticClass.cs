using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visual_Novel_Manager.ViewModel;

namespace Visual_Novel_Manager
{
    public class StaticClass
    {
        public static readonly VnInfoViewModel VnInfoViewModelStatic = new VnInfoViewModel();
        public static readonly  VnListBoxViewModel VnListboxViewModelStatic = new VnListBoxViewModel();
        public static readonly CharacterViewModel CharacterViewModelStatic = new CharacterViewModel();
        public static readonly AddVnViewModel AddVnViewModelStatic = new AddVnViewModel();
        public static readonly ReleasesViewModel ReleasesViewModelStatic = new ReleasesViewModel();
        //public static readonly ScreenshotViewModel ScreenshotViewModelStatic = new ScreenshotViewModel();
        //public static readonly UserVnListViewModel UserVnListViewModelStatic = new UserVnListViewModel();
        public static readonly MainWindowViewModel MainWindowViewModelStatic = new MainWindowViewModel();
        public static readonly SettingsViewModel SettingsViewModel = new SettingsViewModel();
        public static List<JSON.WrittenTagsRootObject> PlainTags;
        public static List<JSON.WrittenTraitsRootObject> PlainTraits;
        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        public static int Vnid = 0;
        public static bool NsfwEnabled = false;
        public static int VnSpoilerLevel = 0;
        public static int CharacterSpoilerLevel = 0;








    }
}
