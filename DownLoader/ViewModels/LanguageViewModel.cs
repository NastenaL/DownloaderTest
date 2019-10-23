using DownLoader.Servises;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
    class LanguageViewModel
    {
        #region Fields
        internal AppSettings appSettings = new AppSettings();
        Models.Language selectedLanguage;
        private ObservableCollection<Models.Language> languages;
        private ICommand changeLanguage;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public ICommand ChangeLanguage
        {
            get
            {
                if (changeLanguage == null)
                    changeLanguage = new RelayCommand<string>(i => ChangeLanguageAction(i));
                return changeLanguage;
            }
        }
        public Models.Language SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                try
                {
                    selectedLanguage = value;
                    appSettings.PrimaryLanguageOverride = value.LanguageCode;
                    RaisePropertyChanged();
                }
                catch (Exception) { }
            }
        }
        public ObservableCollection<Models.Language> Languages
        {
            get => languages;
            set { languages = value; RaisePropertyChanged(); }
        }
        #endregion

        #region Methods
        private void ChangeLanguageAction(string CmbLanguage)
        {
            CmbLanguage = SelectedLanguage.LanguageCode;
            ApplicationLanguages.PrimaryLanguageOverride = CmbLanguage;
       
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
        }
        public LanguageViewModel()
        {
            Languages = new ObservableCollection<Models.Language>
                {
                    new Models.Language { DisplayName = "English", LanguageCode = "en-US" },
                    new Models.Language { DisplayName = "Русский", LanguageCode = "ru-RU" }
                };

            var selectedLanguage = appSettings.PrimaryLanguageOverride;
            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                var f = Languages.FirstOrDefault(l => l.LanguageCode == selectedLanguage);
                if (null != f)
                {
                    SelectedLanguage = f;
                }
            }
            else
            {
                SelectedLanguage = Languages.FirstOrDefault();
            }
        }
        private void RaisePropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
        #endregion
    }
}