using CommonServiceLocator;
using System;
using System.Linq;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using DownLoader.ViewModels;

namespace DownLoader.Servises
{
    class LiveTile
    {
        TileNotification tileNotification;
        readonly MainPageViewModel main = ServiceLocator.Current.GetInstance<MainPageViewModel>();
        internal async void CreateTileAsync()
        {
            var secondaryTile = new SecondaryTile(
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
    }
}
