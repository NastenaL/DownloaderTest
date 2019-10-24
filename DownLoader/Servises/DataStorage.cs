using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;

namespace DownLoader.Models
{
    class DataStorage
    {
        public async void Save(ObservableCollection<DownloadFile> downloadFiles)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("downloads.xml", CreationCollisionOption.ReplaceExisting);
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<DownloadFile>));

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, downloadFiles);
            }
        }

        public async void Save(ObservableCollection<UserAccount> userAccounts)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("accounts.xml", CreationCollisionOption.ReplaceExisting);
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<UserAccount>));

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                serializer.Serialize(stream, userAccounts);
            }
        }

        public async void Load(ObservableCollection<DownloadFile> downloadFiles)
        {
           StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("downloads.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<DownloadFile>));

            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                try
                {
                    ObservableCollection<DownloadFile> downloads = serializer.Deserialize(stream) as ObservableCollection<DownloadFile>;

                    foreach (var c in downloads)
                    {
                        downloadFiles.Add(c);
                    }
                }
                catch (Exception) {}
            }
        }

        public async void Load(ObservableCollection<UserAccount> userAccounts)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("accounts.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<UserAccount>));

            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                try
                {
                    ObservableCollection<UserAccount> accounts = serializer.Deserialize(stream) as ObservableCollection<UserAccount>;

                    foreach (var c in accounts)
                    {
                        userAccounts.Add(c);
                    }
                }
                catch (Exception) { }
            }
        }

    }
}