using CommonServiceLocator;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Linq;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DownLoader.ViewModels
{
   public class SettingViewModel : ViewModelBase
    {
        private INavigationService navigationService;
        readonly LiveTile tile = new LiveTile();

        public bool IsDark;

   

        public RelayCommand NavigateCommand { get; private set; }
        public RelayCommand IsLightCommand { get; private set; }


        public SettingViewModel(INavigationService _navigationService)
        {
            navigationService = _navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);
            IsLightCommand = new RelayCommand(IsLightCommandAction);
          
           tile.CreateTileAsync();
        }

        private void IsLightCommandAction()
        {
            if (AppSettings.Theme == ElementTheme.Light)
                IsDark = true;
            else
                IsDark = false;
        }

        private void NavigateCommandAction()
        {
            navigationService.GoBack(); 
        }
       

        private void ChangeThemeAction(ToggleSwitch sender)
        {
            FrameworkElement window = (FrameworkElement)Window.Current.Content;

            if (((ToggleSwitch)sender).IsOn)
            {
                AppSettings.Theme = AppSettings.NONDEFLTHEME;
                window.RequestedTheme = AppSettings.NONDEFLTHEME;
            }
            else
            {
                AppSettings.Theme = AppSettings.DEFAULTTHEME;
                window.RequestedTheme = AppSettings.DEFAULTTHEME;
            }
        }

    }
}