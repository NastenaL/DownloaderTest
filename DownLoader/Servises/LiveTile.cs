using CommonServiceLocator;
using System;
using System.Linq;
using Windows.UI.StartScreen;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using DownLoader.ViewModels;
using System.Collections.ObjectModel;
using DownLoader.Models;

namespace DownLoader.Servises
{
    class LiveTile
    {
        TileNotification tileNotification;
        internal SecondaryTile secondaryTile;
        TileBindingContentAdaptive tileBindingContentAdaptive;
        MainPageViewModel main = ServiceLocator.Current.GetInstance<MainPageViewModel>();
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
                tileBindingContentAdaptive = new TileBindingContentAdaptive
                {
                    Children =
                    {
                       new AdaptiveText()
                        {
                            Text = main.Files.Last().Name,
                            HintStyle = AdaptiveTextStyle.Base
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
                    DisplayName = main.Files.Last().DateTime.ToShortTimeString()
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
