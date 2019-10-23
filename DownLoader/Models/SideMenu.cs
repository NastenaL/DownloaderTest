using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownLoader.Models
{
    public class SideMenu
    {
        public string Header { get; set; }
        public List<string> SubHeader { get; set; }

        public SideMenu()
        {  
      //      ObservableCollection<SideMenu> mainMenu = new ObservableCollection<SideMenu>();
      //      mainMenu.Add(AllDownloadMenu());
        }

    /*    private SideMenu AllDownloadMenu()
        {
     //       List<string> AllDownloadSubMenu = new List<string> { "Picture", "Music", "Video", "Program"};
     //       SideMenu AllDownloadMenu = new SideMenu() { Header = "All downloads", SubHeader = AllDownloadSubMenu };
     //       return AllDownloadMenu;
        }*/
    }
}
