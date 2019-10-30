﻿using DownLoader.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace DownLoader.ViewModels
{
    public class QueueViewModel: ViewModelBase, INotifyPropertyChanged
    {
        Queue selectedItem;
        public Queue SelectedItem
        {
            get
            {
                return (this.selectedItem);
            }
            set
            {
                if (this.selectedItem != value)
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged("SelectedItem");
                }
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        #endregion

        private readonly ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        readonly DataStorage dataStorage = new DataStorage();
        readonly PopUpControl popUpControl = new PopUpControl();
        private ICommand closePopUp;
        private ICommand openPopUp;
        public ICommand ClosePopUp
        {
            get
            {
                if (closePopUp == null)
                    closePopUp = new RelayCommand<Popup>(i => popUpControl.ClosePopupAction(i));
                return closePopUp;
            }
        }

        public ICommand OpenPopUp
        {
            get
            {
                if (openPopUp == null)
                    openPopUp = new RelayCommand<Popup>(i => popUpControl.OpenPopupAction(i));
                return openPopUp;
            }
        }

        public QueueViewModel()
        {
            AddQueue = new RelayCommand(AddQueueAction);
            EditQueue = new RelayCommand(EditQueueAction);
            Queues = new ObservableCollection<Queue>();
            dataStorage.Load(Queues);
        }

        private string queueName;
        public string QueueName
        {
            get { return this.queueName; }
            set
            {
                if (!string.Equals(this.queueName, value))
                {
                    this.queueName = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<Queue> Queues { get; set; }
        public RelayCommand AddQueue { get; set; }
        private void AddQueueAction()
        {
            Queue newAccount = new Queue
            {
                Id = Guid.NewGuid(),
                Name = QueueName,
                IsStartLoadAt = false,
                IsStopLoadAt = false,

            };
            Queues.Add(newAccount);
            dataStorage.Save(Queues);

            QueueName = "";
        }

        private void EditQueueAction()
        {
            var queue = Queues.FirstOrDefault(i => i.Id.ToString() == SelectedItem.Id.ToString());
            if (queue != null)
            {
                queue.Name = SelectedItem.Name;
            }
            dataStorage.Save(Queues);
        }

        private ICommand updateTable;
        public ICommand UpdateTable
        {
            get
            {
                if (updateTable == null)
                    updateTable = new RelayCommand<ListView>(i => UpdateTableAction(i));
                return updateTable;
            }
        }
        private void UpdateTableAction(ListView queueTable)
        {
            queueTable.ItemsSource = null;
            queueTable.ItemsSource = Queues;
        }

        private ICommand removeQueue;
        public ICommand RemoveQueue
        {
            get
            {
                if (removeQueue == null)
                    removeQueue = new RelayCommand<Queue>(i => RemoveQueueAction(i));
                return removeQueue;
            }
        }

        private async void RemoveQueueAction(Queue file)
        {
            if (file == null)
            {
                ContentDialog notSelectQueueDialog = new ContentDialog()
                {
                    Title = resourceMap.GetValue("titleErrorDeleteQueueDialog", resourceContext).ValueAsString,
                    Content = resourceMap.GetValue("contentErrorRemoveQueueDialog", resourceContext).ValueAsString,
                    PrimaryButtonText = "ОК"
                };
                ContentDialogResult result = await notSelectQueueDialog.ShowAsync();
                return;
            }
            var item = Queues.FirstOrDefault(i => i.Id.ToString() == file.Id.ToString());
            if (item != null)
            {
                Queues.Remove(item);
            }
            dataStorage.Save(Queues);
        }
      
        public RelayCommand EditQueue { get; set; }



    }
}