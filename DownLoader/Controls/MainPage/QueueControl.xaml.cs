using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.Controls.MainPage
{
    public sealed partial class QueueControl : UserControl
    {
        public QueueControl()
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
