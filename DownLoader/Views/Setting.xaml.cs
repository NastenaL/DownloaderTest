using DownLoader.Servises;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DownLoader.Views
{
    public sealed partial class Setting : Page
    {
        public Setting()
        {
            this.InitializeComponent();

            // Set theme for window root
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;
           
        }

       


        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = CmbLanguage.SelectedValue.ToString();
            //Frame.Navigate(this.GetType());
            Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();
            // Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse().Reset();
        }

    }
}
