using CommonServiceLocator;
using DownLoader.Models;
using DownLoader.Servises;
using DownLoader.ViewModels;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader
{
    public sealed partial class MainPage : Page
    {
        MainPageViewModel viewModel = ServiceLocator.Current.GetInstance<MainPageViewModel>();
        public MainPage()
        {
            this.InitializeComponent();

            // Set theme for window root
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;
        }

    }
}