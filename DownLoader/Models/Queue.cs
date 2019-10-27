using System;

namespace DownLoader.Models
{
    public class Queue
    {
        #region Properties
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsStartLoadAt { get; set; }
        public DateTime StartDownload { get; set; }
        public bool IsStopLoadAt { get; set; }
        public DateTime StopDownload { get; set; }
        #endregion
    }
}