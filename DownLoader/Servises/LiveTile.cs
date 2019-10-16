using CommonServiceLocator;
using DownLoader.ViewModels;
using System;
using System.Linq;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace DownLoader.Servises
{
    class LiveTile
    {
        SecondaryTile secondaryTile;
        TileNotification tileNotification;
        readonly MainPageViewModel main = ServiceLocator.Current.GetInstance<MainPageViewModel>();
        internal async void CreateTileAsync()
        {
              secondaryTile = new SecondaryTile(
               "tilePage",
               main.Files.Last().Name,
               "displayname",
               new Uri("ms-appx:///Assets/StoreLogo.png", UriKind.Absolute),
               TileSize.Default);


            var success = await secondaryTile.RequestCreateAsync();
            if (success)
            {
                var tileBindingContentAdaptive = new TileBindingContentAdaptive
                {
                    Children =
                    {
                       new AdaptiveText()
                        {
                            Text = main.Files.Last().Name,
                            HintStyle = AdaptiveTextStyle.Base,
                            HintWrap = true
                        },

                        new AdaptiveText()
                        {
                            Text = main.Files.Last().FileSize,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }
                    }
                };
                var tileBinding = new TileBinding
                {
                    Branding = TileBranding.Name,
                    Content = tileBindingContentAdaptive,
                    DisplayName = main.Files.Last().FileSize
                };

                var tileContent = new TileContent
                {
                    Visual = new TileVisual
                    {
                        TileMedium = tileBinding,
                        TileWide = tileBinding,
                        TileLarge = tileBinding
                    }
                };

                var xmlDoc = tileContent.GetXml();

                tileNotification = new TileNotification(xmlDoc);
               
                var tileUpdaterForSecondaryTile = TileUpdateManager.CreateTileUpdaterForSecondaryTile("tilePage");
                tileUpdaterForSecondaryTile.Update(tileNotification);
            }
        }
        private Color stringToColour(string value)
        {
            return Color.FromArgb(
            Byte.Parse(value.Substring(0, 2), NumberStyles.HexNumber),
            Byte.Parse(value.Substring(2, 2), NumberStyles.HexNumber),
            Byte.Parse(value.Substring(4, 2), NumberStyles.HexNumber),
            Byte.Parse(value.Substring(6, 2), NumberStyles.HexNumber));
        }

        public void AddColor(ComboBox colour)
        {
            Color background = stringToColour(((ComboBoxItem)colour.SelectedItem).Tag.ToString());
            secondaryTile.VisualElements.BackgroundColor = background;
        }

    }
}
