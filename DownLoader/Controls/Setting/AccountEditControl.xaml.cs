using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace DownLoader.Controls.Setting
{
    public sealed partial class AccountEditControl : UserControl
    {
        public AccountEditControl()
        {
            this.InitializeComponent();
        }

        private void ClosePopUp(object sender, RoutedEventArgs e)
        {
            Popup p = (Popup)this.Parent;

            p.IsOpen = false;
        }
    }
}
