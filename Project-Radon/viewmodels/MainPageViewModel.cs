using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Project_Radon.Contracts.Services;
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

        public MainPageViewModel(ISettingsService settingsService, IMessenger messenger) : base(messenger)
        {
            _SettingService = settingsService;
            MyHistory = _SettingService.HistoryStore; // Load the collection from the store

            _SettingService.HistoryCollectionChanged += (sender, e) => {
               MyHistory = _SettingService.HistoryStore;
               OnPropertyChanged(nameof(MyHistory));   
            };
        }

        public void RemoveHistoryItem(HistoryModel historyItem)
        {
            // Remove the item from the collection, but write to the store
            _SettingService.HistoryStore.Remove(historyItem);
            _SettingService.HistoryStore = _SettingService.HistoryStore;
        }
    }
}
