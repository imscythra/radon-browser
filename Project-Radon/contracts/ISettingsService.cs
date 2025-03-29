using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Project_Radon.Models;
using Project_Radon.Services;

namespace Project_Radon.Contracts.Services
{
    public interface ISettingsService
    {
        event EventHandler<NotifyCollectionChangedEventArgs> FavoritesCollectionChanged;
        event EventHandler<NotifyCollectionChangedEventArgs> HistoryCollectionChanged;
        bool IsLocationOnOff { get; set; }
        void InitializeAsync();
        void Remove(string key);
        string HomeUrlString { get; set; }
        ObservableCollection<Bookmarks> FavoritesStore { get; set; }
        int DefaultSearchProvider { get; set; }
        public bool TitleBarPinned { get; set; }
        public RadonAppSettings AppSettings { get; set; }
        bool SideKick { get; set; }
        ObservableCollection<HistoryModel> HistoryStore { get; set; }
        Uri HomeUrl { get; set; }
        string UserAgentWebCore { get; set; }
        ElementTheme ElementTheme { get; set; }
        Visibility VisibilitySideToolBar { get; set; }
    }
}
