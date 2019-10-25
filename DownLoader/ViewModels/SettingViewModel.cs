using DownLoader.Models;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        public SettingViewModel(INavigationService _navigationService)
        {
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            navigationService = _navigationService;
            AddNewAccount = new RelayCommand(AddNewAccountAction);
            NavigateCommand = new RelayCommand(NavigateCommandAction);

            Accounts = new ObservableCollection<UserAccount>();
            dataStorage.Load(Accounts);
        }
        #endregion

        //Accounts
        public ObservableCollection<UserAccount> Accounts { get; set; }
        private ICommand openPopUp;
        private ICommand editAccount;
        private ICommand closePopUp;
        private ICommand removeAccount;
        private ICommand updateTable;
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

        public ICommand EditAccount
        {
            get
            {
                if (editAccount == null)
                    editAccount = new RelayCommand<UserAccount>(i => EditAccountAction(i));
                return editAccount;
            }
        }

        public ICommand RemoveAccount
        {
            get
            {
                if (removeAccount == null)
                    removeAccount = new RelayCommand<UserAccount>(i => RemoveAccountAction(i));
                return removeAccount;
            }
        }

        public ICommand UpdateTable
        {
            get
            {
                if (updateTable == null)
                    updateTable = new RelayCommand<DataGrid>(i => UpdateTableAction(i));
                return updateTable;
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
        public RelayCommand AddNewAccount { get; set; }
        private void AddNewAccountAction()
        {
            UserAccount newAccount = new UserAccount
            {
                Id = Guid.NewGuid(),
                Url = Url,
                Login = Login,
                Password = Password
            };
            Accounts.Add(newAccount);
            dataStorage.Save(Accounts);

            Url = Login = Password = "";
        }

        private void EditAccountAction(UserAccount file)
        {
            var item = Accounts.FirstOrDefault(i => i.Id.ToString() == file.Id.ToString());
            if (item != null)
            {
                item.Url = file.Url;
            }
            dataStorage.Save(Accounts);
        }

        private void RemoveAccountAction(UserAccount file)
        {
            var item = Accounts.FirstOrDefault(i => i.Id.ToString() == file.Id.ToString());
            if (item != null)
            {
                Accounts.Remove(item);
            }
            dataStorage.Save(Accounts);
        }

        private void UpdateTableAction(DataGrid accountTable)
        {
            accountTable.ItemsSource = null;
            accountTable.ItemsSource = Accounts;
        }



    }
}