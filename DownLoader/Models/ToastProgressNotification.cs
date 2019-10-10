using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace DownLoader.Models
{
    class ToastProgressNotification
    {
        internal static readonly ToastNotifier NOTIFIER = ToastNotificationManager.CreateToastNotifier();
        internal ToastNotification toast;

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

            toast = new ToastNotification(content.GetXml())
            {
                Tag = tag,
                Group = group,
                Data = new NotificationData()
            };
            toast.Data.Values["progress"] = "0";
            toast.Data.Values["RecieveBytes"] = "0 kb";


            toast.Data.SequenceNumber = 0;

            NOTIFIER.Show(toast);
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
            toast.Data = data;
            NOTIFIER.Update(toast.Data, tag, group);
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
                Tag = toast.Tag
            };

            NOTIFIER.Show(notification);
        }
    }
}
