using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Project_Radon.Contracts.Services;
using Project_Radon.Contracts.ViewModels;
using Project_Radon.Models;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using Windows.Graphics;
using Windows.UI.Xaml;
using Project_Radon.Helpers;

namespace Project_Radon.ViewModels
{
    public class HistoryViewModel : ObservableRecipient, INavigationAware
    {
        private ObservableCollection<HistoryModel> _myhistory = new ObservableCollection<HistoryModel>();
        public ObservableCollection<HistoryModel> MyHistory { get { return _myhistory; } set { SetProperty(ref _myhistory, value); } }

        public ObservableRecipient PreviousRecipient { get; set; }

        static public async Task<SizeInt32?> SizeWindow()
        {
            var displayList = await DeviceInformation.FindAllAsync
                              (DisplayMonitor.GetDeviceSelector());

            if (!displayList.Any())
                return null;

            var monitorInfo = await DisplayMonitor.FromInterfaceIdAsync(displayList[0].Id);

            var winSize = new SizeInt32();

            if (monitorInfo == null)
            {
                winSize.Width = 800;
                winSize.Height = 1200;
            }
            else
            {
                winSize.Height = monitorInfo.NativeResolutionInRawPixels.Height;
                winSize.Width = monitorInfo.NativeResolutionInRawPixels.Width;

            }

            return winSize;
        }

        RelayCommand<object> _shareTarget = null;
        public RelayCommand<object> ShareTarget
        {
            get
            {
                if (_shareTarget != null)
                    return _shareTarget;
                _shareTarget = new RelayCommand<object>
                (
                        async o =>
                        {
                            var item = o as HistoryModel;
                            if (item is HistoryModel)
                            {
                                if (item.TheUrl is Uri)
                                    await Windows.System.Launcher.LaunchUriAsync(item.TheUrl);
                            }

                        },
                        o => true
                );
                this.PropertyChanged += (s, e) => _shareTarget.NotifyCanExecuteChanged();
                return _shareTarget;
            }
        }

        RelayCommand<object> _removeItem = null;
        public RelayCommand<object> RemoveItem
        {
            get
            {
                if (_removeItem != null)
                    return _removeItem;
                _removeItem = new RelayCommand<object>
                (
                       async o =>
                       {
                           try
                           {
                               HistoryModel item = o as HistoryModel;
                               if (item is HistoryModel)
                               {
                                   var index = -1;
                                   foreach (HistoryModel x in MyHistory)
                                   {
                                       if (x.TheUrl == item.TheUrl)
                                       {
                                           index = MyHistory.IndexOf(x);
                                           if (index >= 0)
                                           {
                                               MyHistory.RemoveAt(index);
                                               break;
                                           }
                                       }
                                   };
                                   SettingsService.HistoryStore = MyHistory;
                               }

                           }
                           catch (Exception e)
                           {
                               await System.Threading.Tasks.Task.Run(() => new ErrorsToUI(e).SendMessage());

                           }

                       },
                        o => true
                );
                this.PropertyChanged += (s, e) => _removeItem.NotifyCanExecuteChanged();
                return _removeItem;
            }

        }

        RelayCommand<object> _clearHistory = null;
        public RelayCommand<object> ClearHistory
        {
            get
            {
                if (_clearHistory != null)
                    return _clearHistory;
                _clearHistory = new RelayCommand<object>
                (
                        o =>
                        {
                            MyHistory = new ObservableCollection<HistoryModel>();
                            SettingsService.HistoryStore = new ObservableCollection<HistoryModel>();
                            OnPropertyChanged(nameof(MyHistory));
                        },
                        o => true
                );
                this.PropertyChanged += (s, e) => _clearHistory.NotifyCanExecuteChanged();
                return _clearHistory;
            }

        }

        public IRightPaneService RightPaneService { get; }

        public ISettingsService SettingsService { get; }

        public IPageService PageService { get; }

        public HistoryViewModel(ISettingsService settingsService, IPageService pageService)
        {


            SettingsService = settingsService;
            PageService = pageService;
            MyHistory = SettingsService.HistoryStore.ToObservableCollection();
            SettingsService.HistoryCollectionChanged += SettingsService_HistoryCollectionChanged;
        }

        public void RemoveEventListeners()
        {

            SettingsService.HistoryCollectionChanged -= SettingsService_HistoryCollectionChanged;

        }


        private void SettingsService_HistoryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    MyHistory = SettingsService.HistoryStore.ToObservableCollection();
                    MyHistory.All((t) =>
                    {
                        t.RemoveItem = RemoveItem;
                        t.ShareTarget = ShareTarget;    
                        return true;

                    });
                    OnPropertyChanged(nameof(MyHistory));
                    break;
                default:
                    break;
            }

        }

        public void OnNavigatedTo(object parameter)
        {
            PreviousRecipient = (parameter is ObservableRecipient) ? (ObservableRecipient)parameter : null;
            MyHistory = SettingsService.HistoryStore?.ToObservableCollection();
            SettingsService.HistoryCollectionChanged += SettingsService_HistoryCollectionChanged;

        }
        public void OnNavigatedFrom()
        {
            SettingsService.HistoryCollectionChanged -= SettingsService_HistoryCollectionChanged;

        }


    }
}
