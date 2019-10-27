using System;

namespace DownLoader.Models
{
    public class UserAccount
    {
        #region Properties
        public Guid Id {get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        #endregion
    }
}
