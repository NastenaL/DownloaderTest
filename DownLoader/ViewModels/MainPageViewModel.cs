﻿using DownLoader.Models;
using DownLoader.Servises;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Fields  
        ContentDialog dialog;
        private bool isEnableButtons = false;
        private CancellationTokenSource cancellationToken;
        private DownloadOperation downloadOperation;
        private ICommand closePopUp;
        private ICommand openPopUp;
        private ICommand updateTable;
        private ICommand updateFileDescription;
        private readonly INavigationService navigationService;
        private readonly BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        private readonly ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        readonly ApiPurchase api = new ApiPurchase();
        readonly DataStorage dataStorage = new DataStorage();
        readonly PopUpControl popUpControl = new PopUpControl();
        readonly ToastNotificationViewModel toastNotification = new ToastNotificationViewModel();
        #endregion

        #region Properties
        public bool IsEnableButtons
        {
            get { return isEnableButtons; }
            set
            {
                isEnableButtons = value;
                OnPropertyChanged("IsEnableButtons");
            }
        }
        public FileType FType { get; set; }
        public IEnumerable<FileType> FileTypes
        {
            get
            {
                return Enum.GetValues(typeof(FileType)).Cast<FileType>();
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
        public ICommand OpenPopUp
        {
            get
            {
                if (openPopUp == null)
                    openPopUp = new RelayCommand<Popup>(i => popUpControl.OpenPopupAction(i));
                return openPopUp;
            }
        }
        public ICommand UpdateTable
        {
            get
            {
                if (updateTable == null)
                    updateTable = new RelayCommand<ListView>(i => UpdateTableAction(i));
                return updateTable;
            }
        }
        public ICommand UpdateFileDescription
        {
            get
            {
                if (updateFileDescription == null)
                    updateFileDescription = new RelayCommand<DownloadFile>(i => UpdateDescription(i));
                return updateFileDescription;
            }
        }
        public ObservableCollection<DownloadFile> Files { get; set; }
        public RelayCommand CancelDownload { get; set; }
        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand DownloadCommandAs { get; set; }
        public RelayCommand GoToSettings { get; private set; }
        public RelayCommand ResumeDownload { get; set; }
        public RelayCommand StopDownload { get; set; }
        public RelayCommand UpdateTile { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        #endregion

        #region Methods
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
        internal void StopDownloadAction()
        {
            if (downloadOperation.Progress.Status == BackgroundTransferStatus.Running)
                downloadOperation.Pause();
        }
        internal void ResumeDownloadAction()
        {
            downloadOperation.Resume();
        }
        private void CancelPurchase()
        {
            CancelDownloadAction();
        }
        private void NavigateCommandAction()
        {
            navigationService.NavigateTo("Setting");
        }
        private void Purchase()
        {
            api.PurchaseFullLicense();
            dialog.Hide();
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
        private void UpdateTableAction(ListView dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Files;
        }
        public async void DownloadAction()
        {
            Progress<DownloadOperation> progress = null;
            if (Url == null || Url == "")
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
                Uri downloadUrl = new Uri(Url);
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
                    if (downloadOperation.Progress.Status == BackgroundTransferStatus.Completed)
                    {
                        toastNotification.SendCompletedToast(fileName);
                        dataStorage.Save(Files);
                        UpdateTileAction();
                    }
                    IsEnableButtons = false;
                    Url = Description = "";
                    FType = FileType.None;
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
        public async void DownloadAsAction()
        {
            Uri downloadUrl = new Uri(Url);
            String fileName = Path.GetFileName(downloadUrl.ToString());
            FileSavePicker savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { Path.GetExtension(fileName) });
            savePicker.SuggestedFileName = Path.GetFileName(downloadUrl.ToString());

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
        public async void ProgressChanged(DownloadOperation downloadOperation)
        {
            int oneUse = 0;
            var NewTotalBytesToReceive = (double)downloadOperation.Progress.TotalBytesToReceive;
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)NewTotalBytesToReceive));
          
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
                    Title = resourceMap.GetValue("purchaseDialogTitle", resourceContext).ValueAsString,
                    Content = resourceMap.GetValue("purchaseDialogContent", resourceContext).ValueAsString,
                    PrimaryButtonText = resourceMap.GetValue("purchaseDialogBuyButton", resourceContext).ValueAsString,
                    PrimaryButtonCommand = new RelayCommand(Purchase),
                    CloseButtonText = resourceMap.GetValue("purchaseDialogCloseButton", resourceContext).ValueAsString,
                    CloseButtonCommand = new RelayCommand(CancelPurchase)
                };
                await ContentDialogMaker.CreateContentDialogAsync(dialog, true);
            }
          
                switch (downloadOperation.Progress.Status)
                {
                    case BackgroundTransferStatus.Running:
                        {
                        IsEnableButtons = true;
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
                        IsEnableButtons = false;
                        Status = resourceMap.GetValue("canceledStatus", resourceContext).ValueAsString;
                            break;
                        }
                }
                if (progress >= 100)
                {
                    downloadOperation = null;
                }
        }
        public MainPageViewModel(INavigationService navigationService)
        {
            FrameworkElement root = (FrameworkElement)Window.Current.Content;
            root.RequestedTheme = AppSettings.Theme;

            CancelDownload = new RelayCommand(CancelDownloadAction);
            DownloadCommand = new RelayCommand(DownloadAction);
            DownloadCommandAs = new RelayCommand(DownloadAsAction);
            GoToSettings = new RelayCommand(NavigateCommandAction);
            this.navigationService = navigationService;
            ResumeDownload = new RelayCommand(ResumeDownloadAction);
            StopDownload = new RelayCommand(StopDownloadAction);

            Files = new ObservableCollection<DownloadFile>();
            dataStorage.Load(Files);
        }
        private void UpdateTileAction()
        {
            LiveTile tile = new LiveTile();
            tile.CreateTileAsync();
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