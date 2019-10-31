using CommonServiceLocator;
using DownLoader.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DownLoader.Servises
{
    class ApiPurchase : Page
    {
        #region Fields
        internal LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        string LicenseMode;
        #endregion

        #region Methods
        internal async void PurchaseFullLicense()
        {
            MainPageViewModel mainPageVM = ServiceLocator.Current.GetInstance<MainPageViewModel>();
            licenseInformation = CurrentAppSimulator.LicenseInformation;
            if (licenseInformation.IsTrial)
            {
                try
                {
                    await CurrentAppSimulator.RequestAppPurchaseAsync(false);
                    if (!licenseInformation.IsTrial && licenseInformation.IsActive)
                    {
                        mainPageVM.ResumeDownloadAction();
                    }
                    else
                    {
                        mainPageVM.CancelDownloadAction();
                    }
                }
                catch (Exception)
                {
                    mainPageVM.CancelDownloadAction();
                }
            }
        }
        private async void DisplayFailDialog()
        {
            ContentDialog noWifiDialog = new ContentDialog()
            {
                Title = "Notification",
                Content = "LoadListingInformationAsync API call failed",
                CloseButtonText = "Ok"
            };
            await noWifiDialog.ShowAsync();
        }
        private void OnLicenseInformationChanged()
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
                if (licenseInformation.IsActive)
                {
                    if (licenseInformation.IsTrial)
                    {
                        LicenseMode = "Trial license";
                    }
                    else { LicenseMode = "Full license"; }
                }
                else { LicenseMode = "Inactive license"; }
            });
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string PurchasePrice;
            CurrentAppSimulator.LicenseInformation.LicenseChanged += OnLicenseInformationChanged;
            await ConfigureSimulatorAsync("trial-mode.xml");

            try
            {
                ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();
                PurchasePrice = listing.FormattedPrice;
            }
            catch (Exception)
            {
                DisplayFailDialog();
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            CurrentAppSimulator.LicenseInformation.LicenseChanged -= OnLicenseInformationChanged;
            base.OnNavigatingFrom(e);
        }
        public static async Task ConfigureSimulatorAsync(string filename)
        {
            StorageFile proxyFile = await Package.Current.InstalledLocation.GetFileAsync("data\\" + filename);
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }
        #endregion
    }
} 
