using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace DownLoader
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void LvDownloads_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

            if (args.ItemIndex % 2 == 0)
            {
                //lighter colour 
                args.ItemContainer.Background = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                //Dark colour 
                args.ItemContainer.Background = new SolidColorBrush(Colors.DarkGray);
            }
        }
    }
}