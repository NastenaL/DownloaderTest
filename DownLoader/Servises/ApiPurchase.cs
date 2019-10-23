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
        internal LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        string LicenseMode;

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
                NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }
        }


        public static async Task ConfigureSimulatorAsync(string filename)
        {
            StorageFile proxyFile = await Package.Current.InstalledLocation.GetFileAsync("data\\" + filename);
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            CurrentAppSimulator.LicenseInformation.LicenseChanged -= OnLicenseInformationChanged;
            base.OnNavigatingFrom(e);
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
                    else
                    {
                        LicenseMode = "Full license";
                    }
                }
                else
                {
                    LicenseMode = "Inactive license";
                }
            });
        }

      

        /// <summary>
        /// Invoked when the user asks purchase the app.
        /// </summary>
        internal async void PurchaseFullLicense()
        {
            MainPageViewModel mainPageVM = ServiceLocator.Current.GetInstance<MainPageViewModel>();
            licenseInformation = CurrentAppSimulator.LicenseInformation;
            NotifyUser("Buying the full license...", NotifyType.StatusMessage);
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
                    NotifyUser("The upgrade transaction failed. You still have a trial license for this app.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                NotifyUser("You already bought this app and have a fully-licensed version.", NotifyType.ErrorMessage);
            }
        }





        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
            //    UpdateStatus(strMessage, type);
            }
            else
            {
         //       var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

       

      
    }
} 
