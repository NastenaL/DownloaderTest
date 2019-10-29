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

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Popup p = (Popup)this.Parent;

            p.IsOpen = false;
        }
    }
}
