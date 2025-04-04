using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Project_Radon.Models;
using Project_Radon.Services;
using System.Threading.Tasks;

namespace Project_Radon.Contracts.Services
{
    public interface ISettingsService
    {
        event EventHandler<NotifyCollectionChangedEventArgs> FavoritesCollectionChanged;
        event EventHandler<NotifyCollectionChangedEventArgs> HistoryCollectionChanged;
        void InitializeAsync();
        void Remove(string key);
        void AddToHistory(string key, string value);
        void RemoveFromHistory(string key, string value);
        void AddToHistory(HistoryModel history);
        void RemoveFromHistory(HistoryModel history);
        Task<T> ReadFavorites<T>();
        Task<T> ReadSettings<T>();
        Task<T> ReadHistory<T>();
        Task WriteHistory<T>(T history);
        Task WriteFavorites<T>(T favorites);
        Task WriteSettings<T>(T settings);

        ObservableCollection<Bookmarks> FavoritesStore { get; set; }
        public RadonAppSettings AppSettings { get; set; }
        ObservableCollection<HistoryModel> HistoryStore { get; set; }
        
        
    }
}
