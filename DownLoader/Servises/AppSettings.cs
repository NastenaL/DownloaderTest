using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;

namespace DownLoader.Servises
{
    class AppSettings
    {
        public const ElementTheme lightTheme = ElementTheme.Light;
        public const ElementTheme darkTheme = ElementTheme.Dark;
        public const ElementTheme defaultTheme = ElementTheme.Default;

        const string KEY_THEME = "appColourMode";
        static ApplicationDataContainer LOCALSETTINGS = ApplicationData.Current.LocalSettings;

        public static ElementTheme Theme
        {
            get
            {
                if (LOCALSETTINGS.Values[KEY_THEME] == null)
                {
                    LOCALSETTINGS.Values[KEY_THEME] = (int)lightTheme;
                    return lightTheme;
                }
                // Previously set to default theme
                else if ((int)LOCALSETTINGS.Values[KEY_THEME] == (int)lightTheme)
                    return lightTheme;
                // Previously set to non-default theme
                else if ((int)LOCALSETTINGS.Values[KEY_THEME] == (int)darkTheme)
                    return darkTheme;
                else
                    return defaultTheme;
            }
            set
            {
                // Error check
                if (value == ElementTheme.Default)
                    throw new System.Exception("Only set the theme to light or dark mode!");
                // Never set
                else if (LOCALSETTINGS.Values[KEY_THEME] == null)
                    LOCALSETTINGS.Values[KEY_THEME] = (int)value;
                // No change
                else if ((int)value == (int)LOCALSETTINGS.Values[KEY_THEME])
                    return;
                // Change
                else
                    LOCALSETTINGS.Values[KEY_THEME] = (int)value;
            }
        }
        //For change language
        public bool UsePrimaryLanguageOverride
        {
            get => ReadSettings(nameof(UsePrimaryLanguageOverride), false);
            set
            {
                SaveSettings(nameof(UsePrimaryLanguageOverride), value);
                NotifyPropertyChanged();
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

        public ApplicationDataContainer LocalSettings { get; set; }

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}