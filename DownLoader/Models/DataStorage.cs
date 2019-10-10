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

        public async void Load(ObservableCollection<DownloadFile> downloadFiles)
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("downloads.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<DownloadFile>));
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                try
                {
                    ObservableCollection<DownloadFile> customerList = serializer.Deserialize(stream) as ObservableCollection<DownloadFile>;

                    foreach (var c in customerList)
                    {
                        downloadFiles.Add(c);
                    }

                }
                catch (Exception)
                {

                }
            }
        }
    }
}