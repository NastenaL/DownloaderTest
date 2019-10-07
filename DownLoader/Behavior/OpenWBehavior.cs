using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Xaml.Interactivity;
using System.ComponentModel;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.Behavior
{
    class OpenWBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

          //  AssociatedObject.SelectedItem += AssociatedObjectOnClosing;
        }

        private void AssociatedObjectOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            //Логика закрытия
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

           // AssociatedObject.Opened -= AssociatedObjectOnClosing;
        }
    }
}
