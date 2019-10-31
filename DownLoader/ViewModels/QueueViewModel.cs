using DownLoader.Models;
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
        #region Fields
        private readonly ResourceContext resourceContext = ResourceContext.GetForViewIndependentUse();
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        private ICommand closePopUp;
        private ICommand openPopUp;
        private ICommand removeQueue;
        private ICommand updateTable;
        private string queueName;
        readonly DataStorage dataStorage = new DataStorage();
        readonly PopUpControl popUpControl = new PopUpControl();
        TimeSpan newStartTime;
        TimeSpan newStopTime;
        Queue selectedItem;
        #endregion

        #region Properties
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
        public ICommand RemoveQueue
        {
            get
            {
                if (removeQueue == null)
                    removeQueue = new RelayCommand<Queue>(i => RemoveQueueAction(i));
                return removeQueue;
            }
        }
        public ICommand UpdateTable
        {
            get
            {
                if (updateTable == null)
                    updateTable = new RelayCommand<ListView>(i => UpdateTableAction(i));
                return updateTable;
            }
        }
        public ObservableCollection<Queue> Queues { get; set; }
        public RelayCommand AddQueue { get; set; }
        public RelayCommand EditQueue { get; set; }
        public string QueueName
        {
            get { return queueName; }
            set
            {
                if (!string.Equals(queueName, value))
                {
                    queueName = value;
                    RaisePropertyChanged();
                }
            }
        }
        public TimeSpan NewStartTime
        {
            get { return (newStartTime); }
            set
            {
                if (newStartTime != value)
                {
                    newStartTime = value;
                    OnPropertyChanged("NewStartTime");
                }
            }
        }
        public TimeSpan NewStopTime
        {
            get { return (newStopTime); }
            set
            {
                if (newStopTime != value)
                {
                    newStopTime = value;
                    OnPropertyChanged("NewStopTime");
                }
            }
        }
        public Queue SelectedItem
        {
            get { return (selectedItem); }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }
        #endregion

        #region Methods
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
        }
        private void EditQueueAction()
        {
            var queue = Queues.FirstOrDefault(i => i.Id.ToString() == SelectedItem.Id.ToString());
            if (queue != null)
            {
                queue.Name = SelectedItem.Name;
                if (!SelectedItem.IsStartLoadAt)
                {
                    NewStartTime = TimeSpan.Zero;
                }
                if (!SelectedItem.IsStopLoadAt)
                {
                    NewStopTime = TimeSpan.Zero;
                }
                queue.IsStartLoadAt = SelectedItem.IsStartLoadAt;
                queue.StartDownload = NewStartTime.ToString();
                queue.IsStopLoadAt = SelectedItem.IsStopLoadAt;
                queue.StopDownload = NewStopTime.ToString();
            }
            dataStorage.Save(Queues);
        }
        private void UpdateTableAction(ListView queueTable)
        {
            queueTable.ItemsSource = null;
            queueTable.ItemsSource = Queues;
        }
        public QueueViewModel()
        {
            AddQueue = new RelayCommand(AddQueueAction);
            EditQueue = new RelayCommand(EditQueueAction);
            Queues = new ObservableCollection<Queue>();
            dataStorage.Load(Queues);
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
        #endregion
    }
}
