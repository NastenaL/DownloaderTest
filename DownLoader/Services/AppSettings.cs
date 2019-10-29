using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;

namespace DownLoader.Servises
{
    class AppSettings
    {
        #region Fields
        const string KEY_THEME = "appColourMode";
        public const ElementTheme darkTheme = ElementTheme.Dark;
        public const ElementTheme defaultTheme = ElementTheme.Default;
        public const ElementTheme lightTheme = ElementTheme.Light;
        static ApplicationDataContainer LOCALSETTINGS = ApplicationData.Current.LocalSettings;
        #endregion

        #region Properties
        public ApplicationDataContainer LocalSettings { get; set; }
        public bool UsePrimaryLanguageOverride
        {
            get => ReadSettings(nameof(UsePrimaryLanguageOverride), false);
            set
            {
                SaveSettings(nameof(UsePrimaryLanguageOverride), value);
                NotifyPropertyChanged();
            }
        }
        public bool ShowTimeSheetSetting
        {
            get
            {
                return ReadSettings(nameof(ShowTimeSheetSetting), true);
            }
            set
            {
                SaveSettings(nameof(ShowTimeSheetSetting), value);
                NotifyPropertyChanged();
            }
        }
        public static ElementTheme Theme
        {
            get
            {
                if (LOCALSETTINGS.Values[KEY_THEME] == null)
                {
                    LOCALSETTINGS.Values[KEY_THEME] = (int)lightTheme;
                    return lightTheme;
                }
                else if ((int)LOCALSETTINGS.Values[KEY_THEME] == (int)lightTheme)
                    return lightTheme;
                else if ((int)LOCALSETTINGS.Values[KEY_THEME] == (int)darkTheme)
                    return darkTheme;
                else
                    return defaultTheme;
            }
            set
            {
                if ((int)value == (int)LOCALSETTINGS.Values[KEY_THEME])
                    return;
                else
                    LOCALSETTINGS.Values[KEY_THEME] = (int)value;
            }
        }
        public string PrimaryLanguageOverride
        {
            get => ReadSettings(nameof(PrimaryLanguageOverride), "ru-RU");
            set
            {
                SaveSettings(nameof(PrimaryLanguageOverride), value);
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }
        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default(T);
        }
        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}