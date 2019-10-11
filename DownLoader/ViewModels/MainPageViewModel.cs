﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;
using System.Threading;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.Storage;
using System.Net;
using System.IO;
using System.Collections.Generic;
using DownLoader.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields

        private CancellationTokenSource cancellationToken;

        private DownloadOperation downloadOperation;
        private ICommand closePopUp;
        private ICommand downloadCommand;
        private ICommand downloadCommandAs;
        private ICommand openPopUp;
        private ICommand refreshDataList;
        private ICommand updateFileDescription;
        private readonly INavigationService navigationService;
        private readonly BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        readonly DataStorageViewModel dataStorage = new DataStorageViewModel();
        readonly PopUpControlViewModel popUpControl = new PopUpControlViewModel();
        readonly ToastNotificationViewModel toastNotification = new ToastNotificationViewModel();
     //   public DownloadFileViewModel downloadVM = new DownloadFileViewModel();

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
        public string Status { get; set; } = "initialization...";
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
        public RelayCommand StopDownload { get; set; }

        public ObservableCollection<DownloadFile> Files { get; set; }

        public ObservableCollection<DownloadFile> SearchResult
        {
            get { return Files; }
            set
            {
                Files = value;
            }
        }

        #endregion

        public MainPageViewModel(INavigationService NavigationService)
        {
            navigationService = NavigationService;
            GoToSettings = new RelayCommand(NavigateCommandAction);
            StopDownload = new RelayCommand(StopDownloadAction);
            CancelDownload = new RelayCommand(CancelDownloadAction);

            Files = new ObservableCollection<DownloadFile>();
            dataStorage.Load(Files);
        }

        #region Methods

        private void CancelDownloadAction()
        {
            cancellationToken.Cancel();
            cancellationToken.Dispose();

            cancellationToken = new CancellationTokenSource();
            var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
            Files.Remove(item);
            dataStorage.Save(Files);
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

        private void StopDownloadAction()
        {
            if (downloadOperation.Progress.Status == BackgroundTransferStatus.Running)
            {
                downloadOperation.Pause();
            }
            else if (downloadOperation.Progress.Status == BackgroundTransferStatus.PausedByApplication)
            {
                downloadOperation.Resume();
            }
        }

        public async void Download(string link)
        {
            Progress<DownloadOperation> progress = null;
            if (link == null || link == "")
            {
                ContentDialog notFoundLinkFileDialog = new ContentDialog()
                {
                    Title = "Подтверждение действия",
                    Content = "Вы не ввели ссылку, попробуем еще раз?",
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
                    DownloadFile newFile = new DownloadFile();
                    newFile.Id = downloadOperation.Guid;
                    newFile.Name = fileName;
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

                    cancellationToken.Dispose();
                    downloadUrl = null;

                }
                catch (TaskCanceledException)
                {
                    Status = "Download canceled.";
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

        public string ProgressChanged(DownloadOperation downloadOperation)
        {
            int progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
            var NewTotalBytesToReceive = (double)downloadOperation.Progress.TotalBytesToReceive;

            if (downloadOperation.GetResponseInformation().Headers.ContainsKey("Content-Length"))
            {
                NewTotalBytesToReceive = Convert.ToDouble(downloadOperation.GetResponseInformation().Headers["Content-Length"]);
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
                            item.Status = string.Format("{0} of {1} kb. downloaded", downloadOperation.Progress.BytesReceived / 1024, Convert.ToInt32(NewTotalBytesToReceive) / 1024);
                            toastNotification.UpdateProgress(NewTotalBytesToReceive, (double)downloadOperation.Progress.BytesReceived, item.Status);
                        }

                        break;
                    }
                case BackgroundTransferStatus.Completed:
                    {
                        var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
                        if (item != null)
                        {
                            item.Status = string.Format("{0} of {1} kb. downloaded - {2}% complete.", downloadOperation.Progress.BytesReceived / 1024, downloadOperation.Progress.TotalBytesToReceive / 1024, progress);
                        }

                        break;
                    }
                case BackgroundTransferStatus.PausedByApplication:
                    {
                        var item = Files.FirstOrDefault(i => i.Id.ToString() == downloadOperation.Guid.ToString());
                        if (item != null)
                        {
                            item.Status = "Download paused.";
                        }
                        break;
                    }
                case BackgroundTransferStatus.PausedCostedNetwork:
                    {
                        Status = "Download paused because of metered connection.";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        Status = "No network detected. Please check your internet connection.";
                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        Status = "An error occured while downloading.";
                        break;
                    }
                case BackgroundTransferStatus.Canceled:
                    {
                        Status = "Download canceled.";
                        break;
                    }
            }

            if (progress >= 100)
            {
                downloadOperation = null;
            }
            return Status = "";
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
                DownloadFile newFile = new DownloadFile();
                await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);
                newFile.Id = downloadOperation.Guid;
                newFile.Name = file.Name;
                newFile.DateTime = DateTime.Now;
                newFile.Type = FType;
                newFile.Description = Description;

                newFile.FileSize = (downloadOperation.Progress.TotalBytesToReceive / 1024).ToString() + " kb";
                newFile.State = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
                newFile.Status = Status;
                Files.Add(newFile);

                dataStorage.Save(Files);
            }
            catch (TaskCanceledException)
            {
                Status = "Download canceled.";
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
            }
        }
        #endregion
    }
}