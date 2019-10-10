using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace DownLoader.Models
{
    class ToastProgressNotification
    {
        #region Fields

        private static readonly ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
        private ToastNotification toastNotification;

        #endregion

        #region Methods


        internal void SendUpdatableToastWithProgress(string FileName)
        {
            string tag = "downloads";
            string group = "downloads";

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
                        Text = "Dowloading.."
                    },

                    new AdaptiveProgressBar()
                    {
                        Title = FileName,
                        Value = new BindableProgressBarValue("progress"),
                        ValueStringOverride = new BindableString("RecieveBytes"),
                        Status = "Downloading"

                    }
                }
                    }
                }
            };

            toastNotification = new ToastNotification(content.GetXml())
            {
                Tag = tag,
                Group = group,
                Data = new NotificationData()
            };
            toastNotification.Data.Values["progress"] = "0";
            toastNotification.Data.Values["RecieveBytes"] = "0 kb";


            toastNotification.Data.SequenceNumber = 0;

            toastNotifier.Show(toastNotification);
        }
        internal void UpdateProgress(double TotalBytes, double RecieveBytes, string Status)
        {
            string tag = "downloads";
            string group = "downloads";

            var data = new NotificationData
            {
                SequenceNumber = 2
            };

            data.Values["progress"] = (RecieveBytes / TotalBytes).ToString();
            data.Values["RecieveBytes"] = Status;
            toastNotification.Data = data;
            toastNotifier.Update(toastNotification.Data, tag, group);
        }

        internal void SendCompletedToast(string FileName)
        {
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Download completed!"
                                },

                                new AdaptiveText()
                                {
                                    Text = FileName
                                }
                            }
                    }
                }
            };

            ToastNotification notification = new ToastNotification(toastContent.GetXml())
            {
                Tag = toastNotification.Tag
            };

            toastNotifier.Hide(toastNotification);
            toastNotifier.Show(notification);
        }
        #endregion
    }
}