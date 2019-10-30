using System;

namespace DownLoader.Models
{
    public class Queue
    {
        #region Properties
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsStartLoadAt { get; set; }
        public string StartDownload { get; set; }
        public bool IsStopLoadAt { get; set; }
        public string StopDownload { get; set; }
        #endregion
    }
}