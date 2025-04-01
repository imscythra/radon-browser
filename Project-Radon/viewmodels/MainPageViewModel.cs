using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Project_Radon.Contracts.Services;
using Project_Radon.Helpers;
using Project_Radon.Models;
using Project_Radon.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Radon.ViewModels
{
    public partial class MainPageViewModel : ObservableRecipient
    {

        public ISettingsService _SettingService;
        [ObservableProperty]
        private ObservableCollection<HistoryModel> _MyHistory;

        [ObservableProperty]
        private bool _CanClearHistory; 
        public MainPageViewModel(ISettingsService settingsService, IMessenger messenger) : base(messenger)
        {
            _SettingService = settingsService;
            // Load the collection from the store
            MyHistory = _SettingService.HistoryStore;
            CanClearHistory = MyHistory.Count > 0;
            MyHistory.All((t) =>
            {
                t.RemoveItem = new RelayCommand<object>(async (o) => await RemoveItem(o));
                return true;
            });
            // handle realtime change from the store to the UI.
            _SettingService.HistoryCollectionChanged += (sender, e) =>
            {
                MyHistory = _SettingService.HistoryStore;
                CanClearHistory = MyHistory.Count > 0;
                MyHistory.All((t) =>
                {
                    t.RemoveItem = new RelayCommand<object>(async (o) => await RemoveItem(o));
                    return true;
                });
                OnPropertyChanged(nameof(CanClearHistory)); 
                OnPropertyChanged(nameof(MyHistory));
                OnPropertyChanged(nameof(ClearAllHistory)); 
            };
        }
         
        [RelayCommand(CanExecute=(nameof(CanClearHistory)))]
        public void ClearAllHistory()
        {

            _SettingService.HistoryStore = new();
            OnPropertyChanged(nameof(MyHistory));
        }

        [RelayCommand]
        public async Task RemoveItem(object o)
        {
            try
            {
                HistoryModel item = o as HistoryModel;
                if (item is HistoryModel)
                {
                    _SettingService.RemoveFromHistory(item);
                    await System.Threading.Tasks.Task.Run(() => new ErrorsToUI($"{item.TheDocumentTitle} has been removed").SendMessage());
                }

            }
            catch (Exception e)
            {
                await System.Threading.Tasks.Task.Run(() => new ErrorsToUI(e).SendMessage());

            }
        }

        public void RemoveHistoryItem(HistoryModel historyItem)
        {
            // Remove the item from the collection, but write to the store
            _SettingService.HistoryStore.Remove(historyItem);
            _SettingService.HistoryStore = _SettingService.HistoryStore;
        }
    }
}
