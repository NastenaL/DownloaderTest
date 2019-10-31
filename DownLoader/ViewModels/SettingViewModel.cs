using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
   public class SettingViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Fields
        private ICommand addTile;
        private ICommand changeTheme;
        private readonly INavigationService navigationService;
        readonly LiveTile tile = new LiveTile();
        #endregion

        #region Properties
        public ICommand AddTile
        {
            get
            {
                if (addTile == null)
                    addTile = new RelayCommand<ComboBox>(i => AddTileAction(i));
                return addTile;
            }
        }
        public ICommand ChangeTheme
        {
            get
            {
                if (changeTheme == null)
                    changeTheme = new RelayCommand<ToggleSwitch>(i => ChangeThemeAction(i));
                return changeTheme;
            }
        }
        public RelayCommand IsLightCommand { get; private set; }
        public RelayCommand NavigateCommand { get; private set; }
        #endregion

        #region Methods
        public SettingViewModel(INavigationService _navigationService)
        {
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            navigationService = _navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);
        }
        private void AddTileAction(ComboBox color)
        {
            tile.CreateTileAsync();
            tile.AddColor(color);
        }
        private void ChangeThemeAction(ToggleSwitch sender)
        {
            FrameworkElement window = (FrameworkElement)Window.Current.Content;
            if (((ToggleSwitch)sender).IsOn)
            {
                AppSettings.Theme = AppSettings.darkTheme;
                window.RequestedTheme = AppSettings.darkTheme;
            }
            else
            {
                AppSettings.Theme = AppSettings.lightTheme;
                window.RequestedTheme = AppSettings.lightTheme;
            }
        }
        private void NavigateCommandAction()
        {
            navigationService.GoBack();
        }
        #endregion
    }
}