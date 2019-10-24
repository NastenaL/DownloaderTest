using DownLoader.Models;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.ViewModels
{
   public class SettingViewModel : ViewModelBase
    {
        #region Fields
        private ICommand addTile;
        private ICommand changeTheme;
        private readonly INavigationService navigationService;
        public bool IsDark;
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
        public RelayCommand NavigateCommand { get; private set; }
        public RelayCommand IsLightCommand { get; private set; }
        #endregion

        #region Methods
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
                AppSettings.Theme = AppSettings.NONDEFLTHEME;
                window.RequestedTheme = AppSettings.NONDEFLTHEME;
            }
            else
            {
                AppSettings.Theme = AppSettings.DEFAULTTHEME;
                window.RequestedTheme = AppSettings.DEFAULTTHEME;
            }
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
        public SettingViewModel(INavigationService _navigationService)
        {
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            Accounts = new ObservableCollection<UserAccount>();

            navigationService = _navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);
            IsLightCommand = new RelayCommand(IsLightCommandAction);
            dataStorage.Load(Accounts);
        }
        #endregion

        //Accounts
        ObservableCollection<UserAccount> Accounts;
        private ICommand openPopUp;
        private ICommand closePopUp;
        private ICommand addAccount;
        readonly DataStorage dataStorage = new DataStorage();
        readonly PopUpControl popUpControl = new PopUpControl();
     
        public ICommand OpenPopUp
        {
            get
            {
                if (openPopUp == null)
                    openPopUp = new RelayCommand<Popup>(i => popUpControl.OpenPopupAction(i));
                return openPopUp;
            }
        }

        public ICommand ClosePopUp
        {
            get
            {
                if (closePopUp == null)
                    closePopUp = new RelayCommand<Popup>(i => popUpControl.ClosePopupAction(i));
                return closePopUp;
            }
        }
        public ICommand AddAccount
        {
            get
            {
                if (addAccount == null)
                    addAccount = new RelayCommand<UserAccount>(i => AddNewAccount(i));
                return addAccount;
            }
        }

        private string url;
        public string Url
        {
            get { return this.url; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.url, value))
                {
                    this.url = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string login;
        public string Login
        {
            get { return this.login; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.login, value))
                {
                    this.login = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }
        private string password;
        public string Password
        {
            get { return this.password; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this.password, value))
                {
                    this.password = value;
                    this.RaisePropertyChanged(); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }

        private void AddNewAccount(UserAccount newAccount)
        {
            newAccount = new UserAccount();
            newAccount.Url = Url;
            newAccount.Login = Login;
            newAccount.Password = Password;
            Accounts.Add(newAccount);
            dataStorage.Save(Accounts);

            Url = Login = Password = "";
        }


    }
}