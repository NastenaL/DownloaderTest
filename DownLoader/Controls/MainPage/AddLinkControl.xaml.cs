using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.Controls
{
    public sealed partial class AddLinkControl : UserControl
    {
      
        public AddLinkControl()
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
