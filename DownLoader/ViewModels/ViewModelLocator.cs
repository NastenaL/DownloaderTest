using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using DownLoader.Views;
using CommonServiceLocator;

namespace DownLoader.ViewModels
{
    public class ViewModelLocator
    {
        #region Fields
        public const string MainPageKey = "MainPage";
        public const string NewKey = "Setting";
        #endregion

        #region Properties
        public MainPageViewModel MainPageLocator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }
        public SettingViewModel SettingPageLocator
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingViewModel>();
            }
        }
        #endregion

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();
            nav.Configure(MainPageKey, typeof(MainPage));
            nav.Configure(NewKey, typeof(Setting));

            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
        }
    }
}