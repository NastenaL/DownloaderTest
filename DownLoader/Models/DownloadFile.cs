using System;
using System.ComponentModel;

namespace DownLoader.Models
{
    #region Type enum
    public enum FileType
    {
        None = 0,
        Picture = 1,
        Music = 2,
        Program = 3,
        Video = 4,
        Zip = 5
    }
    #endregion

    public class DownloadFile : INotifyPropertyChanged
    {
        #region Fields
        private int state;
        private string fileSize;
        private string status;
        #endregion

        #region Properties
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }
        public DateTime DateTime { get; set; }
        public FileType Type { get; set; }
        public int State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }
        public string Description { get; set; }
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