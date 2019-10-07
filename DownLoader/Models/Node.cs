using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;

namespace DownLoader.Models
{
    class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public void GetNodes()
        {
            var resourceloader = ResourceLoader.GetForCurrentView();
            Node Picture = new Node() { Name = FileType.Picture.ToString() };
            Node Music = new Node() { Name = FileType.Music.ToString() };
            Node AllDownloads = new Node();
            AllDownloads.Name = resourceloader.GetString("tvAllDownloads");
            AllDownloads.Nodes.Add(Picture);
            AllDownloads.Nodes.Add(Music);
        }
    }
}
