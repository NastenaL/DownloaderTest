using DownLoader.Models;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.ViewModels
{
    public class AccountViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Fields
        private ICommand closePopUp;
        private ICommand openPopUp;
        private ICommand removeAccount;
        private ICommand updateTable;
        private readonly ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        readonly DataStorage dataStorage = new DataStorage();
        readonly LiveTile tile = new LiveTile();
        readonly PopUpControl popUpControl = new PopUpControl();
        private string login;
        private string password;
        private string url;
        UserAccount selectedItem;
        #endregion

        #region Properties
        public ICommand ClosePopUp
        {
            get
            {
                if (closePopUp == null)
                    closePopUp = new RelayCommand<Popup>(i => popUpControl.ClosePopupAction(i));
                return closePopUp;
            }
        }
        public ICommand OpenPopUp
        {
            get
            {
                if (openPopUp == null)
                    openPopUp = new RelayCommand<Popup>(i => popUpControl.OpenPopupAction(i));
                return openPopUp;
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
        public ObservableCollection<UserAccount> Accounts { get; set; }
        public RelayCommand AddNewAccount { get; set; }
        public RelayCommand EditAccount { get; set; }
        public UserAccount SelectedItem
        {
            get { return (selectedItem); }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }
        public string Url
        {
            get { return url; }
            set
            {
                if (!string.Equals(url, value))
                {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                if (!string.Equals(login, value))
                {
                    login = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (!string.Equals(password, value))
                {
                    password = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        private async void RemoveAccountAction(UserAccount file)
        {
            if (file == null)
            {
                ContentDialog notSelectAccountDialog = new ContentDialog()
                {
                    Title = resourceMap.GetValue("titleErrorDeleteAccountDialog", resourceContext).ValueAsString,
                    Content = resourceMap.GetValue("contentErrorRemoveAccountDialog", resourceContext).ValueAsString,
                    PrimaryButtonText = "ОК"
                };
                ContentDialogResult result = await notSelectAccountDialog.ShowAsync();
                return;
            }

            var item = Accounts.FirstOrDefault(i => i.Id.ToString() == file.Id.ToString());
            if (item != null)
            {
                Accounts.Remove(item);
            }
            dataStorage.Save(Accounts);
        }
        public AccountViewModel()
        {
            AddNewAccount = new RelayCommand(AddNewAccountAction);
            EditAccount = new RelayCommand(EditAccountAction);

            Accounts = new ObservableCollection<UserAccount>();
            dataStorage.Load(Accounts);
        }
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
        }
        private void EditAccountAction()
        {
            var item = Accounts.FirstOrDefault(i => i.Id.ToString() == SelectedItem.Id.ToString());
            if (item != null)
            {
                item.Url = SelectedItem.Url;
                item.Login = SelectedItem.Login;
                item.Password = SelectedItem.Password;
            }
            dataStorage.Save(Accounts);
        }
        private void UpdateTableAction(DataGrid accountTable)
        {
            accountTable.ItemsSource = null;
            accountTable.ItemsSource = Accounts;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        #endregion
    }
}