using DownLoader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace DownLoader.ViewModels
{
    internal class DownloadFileViewModel
    {

        private CancellationTokenSource cancellationToken;

        private DownloadOperation downloadOperation;
        private readonly BackgroundDownloader backgroundDownloader = new BackgroundDownloader();
        readonly DataStorageViewModel dataStorage = new DataStorageViewModel();
        readonly PopUpControlViewModel popUpControl = new PopUpControlViewModel();
        public ObservableCollection<DownloadFile> Files { get; set; }


        readonly ToastNotificationViewModel toastNotification = new ToastNotificationViewModel();
        public FileType FType { get; set; }
       
        public string Description { get; set; }
        public string Status { get; set; } = "initialization...";

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
    }
}
