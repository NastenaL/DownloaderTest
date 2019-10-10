using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using DownLoader.Servises;
using GalaSoft.MvvmLight.Command;
using Windows.Globalization;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
    class LanguageViewModel
    {
        #region Fields

        internal AppSettings appSettings = new AppSettings();
        public event PropertyChangedEventHandler PropertyChanged;
        Models.Language selectedLanguage;
        private ObservableCollection<Models.Language> languages;

        #endregion
   //     CultureInfo ci;
        
        #region Properties
        private ICommand changeL;
        public ICommand ChangeL
        {
            get
            {
                if (changeL == null)
                    changeL = new RelayCommand<string>(i => CmbLanguage_SelectionChanged(i));
                return changeL;
            }
        }

        public ObservableCollection<Models.Language> Languages
        {
            get => languages;
            set { languages = value; RaisePropertyChanged(); }
        }
        public Models.Language SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                selectedLanguage = value;
                appSettings.PrimaryLanguageOverride = value.LanguageCode;
                RaisePropertyChanged();
            }
        }

        private void CmbLanguage_SelectionChanged(string selectedLanguage)
        {
          //   selectedLanguage = System.Globalization.CultureInfo.CurrentCulture;//"ru-RU";
            ApplicationLanguages.PrimaryLanguageOverride = selectedLanguage;
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            // Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
        }

        #endregion

        #region Methods

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