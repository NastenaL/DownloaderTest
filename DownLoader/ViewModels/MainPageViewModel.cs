using DownLoader.Models;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;


namespace DownLoader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        ApiPurchase api = new ApiPurchase();
        ContentDialog dialog;


        #region Fields
        private CancellationTokenSource cancellationToken;
        private DownloadOperation downloadOperation;
        private ICommand closePopUp;
        private ICommand downloadCommand;
        private ICommand downloadCommandAs;
        private ICommand openPopUp;
        private ICommand enableButton;
        private ICommand refreshDataList;
        private ICommand updateFileDescription;
        private readonly INavigationService navigationService;
        private readonly BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        private readonly ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        readonly DataStorage dataStorage = new DataStorage();
        readonly PopUpControl popUpControl = new PopUpControl();
        readonly ToastNotificationViewModel toastNotification = new ToastNotificationViewModel();
        internal string linkURL;
        #endregion

        #region Properties

        public FileType FType { get; set; }
        public IEnumerable<FileType> FileTypes
        {
            get
            {
                return Enum.GetValues(typeof(FileType)).Cast<FileType>();
            }
        }
        public RelayCommand GoToSettings { get; private set; }
        public string Description { get; set; }
        public string Status {get;set;}
        public ICommand UpdateFileDescription
        {
            get
            {
                if (updateFileDescription == null)
                    updateFileDescription = new RelayCommand<DownloadFile>(i => UpdateDescription(i));
                return updateFileDescription;
            }
        }

        public ICommand RefreshDataListView
        {
            get
            {
                if (refreshDataList == null)
                    refreshDataList = new RelayCommand<ListView>(i => RefreshListView(i));
                return refreshDataList;
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
        public ICommand EnableButton
        {
            get
            {
                if (enableButton == null)
                    enableButton = new RelayCommand<Button>(i => EnableButtonAction(i));
                return enableButton;
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
        public ICommand DownloadCommand
        {
            get
            {
                if (downloadCommand == null)
                    downloadCommand = new RelayCommand<string>(i => Download(i));
                return downloadCommand;
            }
        }
        public ICommand DownloadCommandAs
        {
            get
            {
                if (downloadCommandAs == null)
                    downloadCommandAs = new RelayCommand<string>(i => SaveAs(i));
                return downloadCommandAs;
            }
        }
        public RelayCommand CancelDownload { get; set; }
        public RelayCommand ResumeDownload { get; set; }
        public RelayCommand StopDownload { get; set; }
        public RelayCommand UpdateTile { get; set; }
        public ObservableCollection<DownloadFile> Files { get; set; }
       
        #endregion

        private void EnableButtonAction(Button button)
        {
            try
            {
                if (downloadOperation.Progress.Status == BackgroundTransferStatus.Running)
                {
                    button.IsEnabled = true;
                }
                button.IsEnabled = false;
            }
            catch(Exception)
            { }
        }
        public MainPageViewModel(INavigationService NavigationService)
        {

            // Set theme for window root
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            navigationService = NavigationService;
            GoToSettings = new RelayCommand(NavigateCommandAction);
            ResumeDownload = new RelayCommand(ResumeDownloadAction);
            StopDownload = new RelayCommand(StopDownloadAction);
            CancelDownload = new RelayCommand(CancelDownloadAction);

            Files = new ObservableCollection<DownloadFile>();
            dataStorage.Load(Files);
        }

        #region Methods
        private void UpdateTileAction()
        {
            LiveTile tile = new LiveTile();
            tile.CreateTileAsync();
        }

        internal void CancelDownloadAction()
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();

            cancellationToken = new CancellationTokenSource();
            var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
            if(item != null)
            {
                Files.Remove(item);
                dataStorage.Save(Files);
            }
        }

        private void NavigateCommandAction()
        {
            navigationService.NavigateTo("Setting");
        }

        private void RefreshListView(ListView dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Files;
        }
        private void UpdateDescription(DownloadFile file)
        {
            var item = Files.FirstOrDefault(i => i.Id.ToString() == file.Id.ToString());
            if (item != null)
            {
                item.Description = file.Description;
            }
            dataStorage.Save(Files);
        }

        internal void StopDownloadAction()
        {
           if(downloadOperation.Progress.Status == BackgroundTransferStatus.Running)
         downloadOperation.Pause();
        }

        internal void ResumeDownloadAction()
        {
            downloadOperation.Resume();
        }
  
        public async void Download(string link)
        {
            linkURL = link;
            Progress<DownloadOperation> progress = null;
            if (link == null || link == "")
            {
                ContentDialog notFoundLinkFileDialog = new ContentDialog()
                {
                    Title = resourceMap.GetValue("titleErrorDownloadFileDialog", resourceContext).ValueAsString,
                    Content = resourceMap.GetValue("contentErrorDownloadFileDialog", resourceContext).ValueAsString,
                    PrimaryButtonText = "ОК"
                };
                ContentDialogResult result = await notFoundLinkFileDialog.ShowAsync();
                return;
            }
            FolderPicker folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                ViewMode = PickerViewMode.Thumbnail
            };
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Uri downloadUrl = new Uri(link);
                String fileName = Path.GetFileName(downloadUrl.ToString());
                var request = HttpWebRequest.Create(downloadUrl) as HttpWebRequest;
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                downloadOperation = backgroundDownloader.CreateDownload(downloadUrl, file);
                progress = new Progress<DownloadOperation>(x => ProgressChanged(downloadOperation));
                cancellationToken = new CancellationTokenSource();
                try
                {
                    DownloadFile newFile = new DownloadFile
                    {
                        Id = downloadOperation.Guid,
                        Name = fileName
                    };
                    toastNotification.SendUpdatableToastWithProgress(newFile.Name);

                    newFile.FileSize = (downloadOperation.Progress.TotalBytesToReceive / 1024).ToString() + " kb";
                    newFile.DateTime = DateTime.Now;
                    newFile.Type = FType;
                    newFile.Description = Description;
                    newFile.Status = Status;
                    Files.Add(newFile);
                
                    await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                    
                    if(downloadOperation.Progress.Status == BackgroundTransferStatus.Completed)
                    {
                        toastNotification.SendCompletedToast(fileName);
                        dataStorage.Save(Files);
                        UpdateTileAction();
                    }
                    
                }
                catch (TaskCanceledException)
                {
                    Status = resourceMap.GetValue("canceledStatus", resourceContext).ValueAsString;
                    await downloadOperation.ResultFile.DeleteAsync();
                    downloadOperation = null;
                }
                catch (Exception)
                {
                    Status = "File not found";
                    var messageDialog = new MessageDialog("No internet connection has been found.");

                    await downloadOperation.ResultFile.DeleteAsync();
                    downloadOperation = null;
                }
            }
        }

        private void Purchase()
        {
         api.PurchaseFullLicense();
            dialog.Hide();
        }

        private void CancelPurchase()
        {
            CancelDownloadAction();
        }


        public async void ProgressChanged(DownloadOperation downloadOperation)
        {
            int oneUse = 0;
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
            var NewTotalBytesToReceive = (double)downloadOperation.Progress.TotalBytesToReceive;

            if (downloadOperation.GetResponseInformation().Headers.ContainsKey("Content-Length"))
            {
                NewTotalBytesToReceive = Convert.ToDouble(downloadOperation.GetResponseInformation().Headers["Content-Length"]);
            }
            if (NewTotalBytesToReceive >= 50000000 && api.licenseInformation.IsTrial && oneUse == 0)
            {
                StopDownloadAction();
                oneUse++;

                dialog = new ContentDialog
                {
                    Title = "Title",
                    Content = "Для скачивания файлов размером более 50 МБ купите полную версию",
                    PrimaryButtonText = "Купить",
                    PrimaryButtonCommand = new RelayCommand(Purchase),
                    CloseButtonText = "Закрыть",
                    CloseButtonCommand = new RelayCommand(CancelPurchase)
                };
                await ContentDialogMaker.CreateContentDialogAsync(dialog, true);
            }
          
                switch (downloadOperation.Progress.Status)
                {
                    case BackgroundTransferStatus.Running:
                        {
                            var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
                            if (item != null)
                            {
                                item.FileSize = (Convert.ToInt32(NewTotalBytesToReceive) / 1024).ToString() + " kb";
                                progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / Convert.ToInt32(NewTotalBytesToReceive)));
                                item.State = progress;
                                item.Status = string.Format(resourceMap.GetValue("runningStatus", resourceContext).ValueAsString, downloadOperation.Progress.BytesReceived / 1024, Convert.ToInt32(NewTotalBytesToReceive) / 1024);
                                toastNotification.UpdateProgress(NewTotalBytesToReceive, (double)downloadOperation.Progress.BytesReceived, item.Status);
                            }
                            break;
                        }
                    case BackgroundTransferStatus.Completed:
                        {
                            var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
                            if (item != null)
                            {
                                item.Status = string.Format(resourceMap.GetValue("runningStatus", resourceContext).ValueAsString, downloadOperation.Progress.BytesReceived / 1024, downloadOperation.Progress.TotalBytesToReceive / 1024);
                            }
                    
                            break;
                        }
                    case BackgroundTransferStatus.PausedByApplication:
                        {
                            var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
                            if (item != null)
                            {
                                item.Status = resourceMap.GetValue("pausedByApplicationStatus", resourceContext).ValueAsString;
                            }
                            break;
                        }
                    case BackgroundTransferStatus.PausedCostedNetwork:
                        {
                            Status = resourceMap.GetValue("pausedCostedNetworkStatus", resourceContext).ValueAsString;
                            break;
                        }
                    case BackgroundTransferStatus.PausedNoNetwork:
                        {
                            Status = resourceMap.GetValue("pausedNoNetworkStatus", resourceContext).ValueAsString;
                            break;
                        }
                    case BackgroundTransferStatus.Error:
                        {
                            Status = resourceMap.GetValue("errorStatus", resourceContext).ValueAsString;
                            break;
                        }
                    case BackgroundTransferStatus.Canceled:
                        {
                            Status = resourceMap.GetValue("canceledStatus", resourceContext).ValueAsString;
                            break;
                        }
                }
                if (progress >= 100)
                {
                    downloadOperation = null;
                }
        }

        public async void SaveAs(string link)
        {
            Uri downloadUrl = new Uri(link);
            String fileName = Path.GetFileName(downloadUrl.ToString());

            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { Path.GetExtension(fileName) });
            savePicker.SuggestedFileName = "NewFile";

            var request = HttpWebRequest.Create(downloadUrl) as HttpWebRequest;
            StorageFile file = await savePicker.PickSaveFileAsync();
            downloadOperation = backgroundDownloader.CreateDownload(downloadUrl, file);
            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(x => ProgressChanged(downloadOperation));
            cancellationToken = new CancellationTokenSource();

            try
            {
                DownloadFile newFile = new DownloadFile
                {
                    Id = downloadOperation.Guid,
                    Name = fileName
                };
                toastNotification.SendUpdatableToastWithProgress(newFile.Name);

                newFile.FileSize = (downloadOperation.Progress.TotalBytesToReceive / 1024).ToString() + " kb";
                newFile.DateTime = DateTime.Now;
                newFile.Type = FType;
                newFile.Description = Description;
                newFile.Status = Status;
                Files.Add(newFile);

                await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);

                toastNotification.SendCompletedToast(fileName);
                dataStorage.Save(Files);
                UpdateTileAction();
            }
            catch (TaskCanceledException)
            {
                Status = resourceMap.GetValue("canceledStatus", resourceContext).ValueAsString;
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
            }
        }
        #endregion

     

    }
}