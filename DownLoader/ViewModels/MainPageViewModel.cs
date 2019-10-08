using GalaSoft.MvvmLight;
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
using System.Xml.Serialization;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;

using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DownLoader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Fields
        private CancellationTokenSource cancellationToken;
        public DownloadFile newFile = new DownloadFile();
        private DownloadOperation downloadOperation;
        private ICommand closePopUp;
        private ICommand downloadCommand;
        private ICommand downloadCommandAs;
        private ICommand filterFilesByType;
        private ICommand openPopUp;
        private ICommand refreshDataGrid;
        private ICommand updateFileDescription;
        private string status = "initialization...";
        private readonly INavigationService navigationService;
        private Windows.Networking.BackgroundTransfer.BackgroundDownloader backgroundDownloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();



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
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
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

        public ICommand RefreshDataGrid
        {
            get
            {
                if (refreshDataGrid == null)
                    refreshDataGrid = new RelayCommand<DataGrid>(i => RefreshGrid(i));
                return refreshDataGrid;
            }
        }
        public ICommand OpenPopUp
        {
            get
            {
                if (openPopUp == null)
                    openPopUp = new RelayCommand<Popup>(i => OpenPopupAction(i));
                return openPopUp;
            }
        }

        public ICommand ClosePopUp
        {
            get
            {
                if (closePopUp == null)
                    closePopUp = new RelayCommand<Popup>(i => ClosePopupAction(i));
                return closePopUp;
            }
        }

        public ICommand FilterFilesByType
        {
            get
            {
                if (filterFilesByType == null)
                    filterFilesByType = new RelayCommand<ComboBox>(i => FilterByType(i));
                return filterFilesByType;
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

            Files = new ObservableCollection<DownloadFile>();
            Load();
        }

        #region Methods


        private void OpenPopupAction(Popup popupName)
        {
            popupName.IsOpen = true;
        }
        private void RefreshGrid(DataGrid dataGrid)
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
            Save();
        }

        private void NavigateCommandAction()
        {
            navigationService.NavigateTo("Setting");
        }

        private void ClosePopupAction(Popup popupName)
        {
            popupName.IsOpen = false;
        }
        private void FilterByType(ComboBox tvTypes)
        {
            var search = Files.Where(i => i.Type.ToString() == tvTypes.SelectedValue.ToString());
            foreach (DownloadFile file in search)
            {
                SearchResult.Add(file);
            }

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
            
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.Thumbnail;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Uri downloadUrl = new Uri(link);
                String fileName = Path.GetFileName(downloadUrl.ToString());
                var request = HttpWebRequest.Create(downloadUrl) as HttpWebRequest;
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                downloadOperation = backgroundDownloader.CreateDownload(downloadUrl, file);
                Progress<DownloadOperation> progress = new Progress<DownloadOperation>(x => ProgressChanged(downloadOperation));
                cancellationToken = new CancellationTokenSource();
                try
                {
                    newFile.Id = downloadOperation.Guid;
                    newFile.Name = fileName;
                    newFile.FileSize = (downloadOperation.Progress.TotalBytesToReceive / 1024).ToString() + " kb";
                    newFile.DateTime = DateTime.Now;
                    newFile.Type = FType;
                    newFile.Description = Description;
                    newFile.Status = Status;
                    Files.Add(newFile);
                    await downloadOperation.StartAsync().AsTask(cancellationToken.Token, progress);

                    Save();

                }
                catch (TaskCanceledException)
                {
                    status = "Download canceled.";
                    await downloadOperation.ResultFile.DeleteAsync();
                    downloadOperation = null;
                }
                catch (Exception)
                {
                    status = "File not found";
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
                         
                            {
                                item.FileSize = (Convert.ToInt32(NewTotalBytesToReceive) / 1024).ToString() + " kb";
                                progress = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / Convert.ToInt32(NewTotalBytesToReceive)));
                                item.State = progress;
                                item.Status = string.Format("{0} of {1} kb. downloaded", downloadOperation.Progress.BytesReceived / 1024, Convert.ToInt32(NewTotalBytesToReceive) / 1024);
                            }
                        //    SendUpdatableToastWithProgress(item.Name, progress, item.Status);

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
                        status = "Download paused because of metered connection.";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        status = "No network detected. Please check your internet connection.";
                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        status = "An error occured while downloading.";
                        break;
                    }
                case BackgroundTransferStatus.Canceled:
                    {
                        status = "Download canceled.";
                        break;
                    }
            }

            if (progress >= 100)
            {
                downloadOperation = null;
            }
            return status = "";
        }
        public async void SaveAs(string link)
        {
            Uri downloadUrl = new Uri(link);
            String fileName = Path.GetFileName(downloadUrl.ToString());

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { Path.GetExtension(fileName) });
            savePicker.SuggestedFileName = "NewFile";

            var request = HttpWebRequest.Create(downloadUrl) as HttpWebRequest;
            StorageFile file = await savePicker.PickSaveFileAsync();
            downloadOperation = backgroundDownloader.CreateDownload(downloadUrl, file);
            Progress<DownloadOperation> progress = new Progress<DownloadOperation>(x => ProgressChanged(downloadOperation));
            cancellationToken = new CancellationTokenSource();

            try
            {
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

                Save();
            }
            catch (TaskCanceledException)
            {
                status = "Download canceled.";
                await downloadOperation.ResultFile.DeleteAsync();
                downloadOperation = null;
            }
        }

        public async void Save()
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("downloads.xml", CreationCollisionOption.ReplaceExisting);
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<DownloadFile>));
            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, this.Files);
            }
        }

        public async void Load()
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("downloads.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<DownloadFile>));
            ObservableCollection<DownloadFile> customerList = null;
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                customerList = serializer.Deserialize(stream) as ObservableCollection<DownloadFile>;
                foreach (var c in customerList)
                {
                    Files.Add(c);
                }
            }
        }
        #endregion


        public void SendUpdatableToastWithProgress(string FileName, double progress, string RecieveBytes)
        {
            // Define a tag (and optionally a group) to uniquely identify the notification, in order update the notification data later;
            string tag = "weekly-playlist";
            string group = "downloads";

            // Construct the toast content with data bound fields
            var content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Downloading ..."
                    },

                    new AdaptiveProgressBar()
                    {
                        Title = FileName,
                        Value = new BindableProgressBarValue("progress"),
                        ValueStringOverride = new BindableString("RecieveBytes"),
                        Status = new BindableString("RecieveBytes")
                    }
                }
                    }
                }
            };

            // Generate the toast notification
            var toast = new ToastNotification(content.GetXml());

            // Assign the tag and group
            toast.Tag = tag;
            toast.Group = group;

            // Assign initial NotificationData values
            // Values must be of type string
            toast.Data = new NotificationData();
            toast.Data.Values["progressValue"] = "0.6";
            toast.Data.Values["progressValueString"] = "15/26 songs";
            toast.Data.Values["progressStatus"] = "Downloading...";

            // Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
            toast.Data.SequenceNumber = 1;

            // Show the toast notification to the user
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        public void UpdateProgress()
        {
            // Construct a NotificationData object;
            string tag = "weekly-playlist";
            string group = "downloads";

            // Create NotificationData and make sure the sequence number is incremented
            // since last update, or assign 0 for updating regardless of order
            var data = new NotificationData
            {
                SequenceNumber = 2
            };

            // Assign new values
            // Note that you only need to assign values that changed. In this example
            // we don't assign progressStatus since we don't need to change it
            data.Values["progressValue"] = "0.7";
            data.Values["progressValueString"] = "18/26 songs";

            // Update the existing notification's data by using tag/group
            ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
        }
    }
}