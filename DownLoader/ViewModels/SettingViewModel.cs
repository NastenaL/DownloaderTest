using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
   public class SettingViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private ICommand changeTheme;
    
        public bool IsDark;

      
        public ICommand ChangeTheme
        {
            get
            {
                if (changeTheme == null)
                    changeTheme = new RelayCommand<ToggleSwitch>(i => ChangeThemeAction(i));
                return changeTheme;
            }
        }

        public RelayCommand NavigateCommand { get; private set; }
        public RelayCommand IsLightCommand { get; private set; }


        public SettingViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);
            IsLightCommand = new RelayCommand(IsLightCommandAction);
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
            _navigationService.GoBack(); 
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