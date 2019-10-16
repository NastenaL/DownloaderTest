using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
   public class SettingViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        public bool IsDark;
        readonly LiveTile tile = new LiveTile();

        public RelayCommand NavigateCommand { get; private set; }
   //     public RelayCommand AddTile { get; set; }
        public RelayCommand IsLightCommand { get; private set; }


        public SettingViewModel(INavigationService _navigationService)
        {
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            navigationService = _navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);
            IsLightCommand = new RelayCommand(IsLightCommandAction);
    //        AddTile = new RelayCommand(AddTileAction);
          
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

        private void AddTileAction(ComboBox color)
        {
            tile.CreateTileAsync();
            tile.AddColor(color);
        }

        private ICommand changeTheme;
        public ICommand ChangeTheme
        {
            get
            {
                if (changeTheme == null)
                    changeTheme = new RelayCommand<ToggleSwitch>(i => ChangeThemeAction(i));
                return changeTheme;
            }
        }

        private ICommand addTile;
        public ICommand AddTile
        {
            get
            {
                if (addTile == null)
                    addTile = new RelayCommand<ComboBox>(i => AddTileAction(i));
                return addTile;
            }
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