﻿using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.Models
{
    class PopUpControl
    {
        internal void ClosePopupAction(Popup popupName)
        {
            popupName.IsOpen = false;
        }
        internal void OpenPopupAction(Popup popupName)
        {
            popupName.IsOpen = true;
        }
    }
}