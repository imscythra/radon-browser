using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Project_Radon.Contracts.Services;
using Newtonsoft.Json;
using Project_Radon.Models;
using Project_Radon.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using Yttrium_browser;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.VoiceCommands;



namespace Project_Radon.Services
{

    interface IAppSettings
    {

        Uri HomeUrl { get; set; }
        bool SideKick { get; set; }
        int DefaultSearchProvider { get; set; }
        bool IsLocationOnOff { get; set; }
        string UserAgentWebCore { get; set; }
        ElementTheme ThemeDefault { get; set; }
        Visibility VisibilitySideToolBar { get; set; }
        string HomeUrlString { get; set; }
        bool TitleBarPinned { get; set; }
        string AppColorTheme { get; set; }
        bool InlineMode { get; set; }
        bool SystemTitleBar { get; set; }
        string Username { get; set; }
        string QRCodeUrl { get; set; }
        string ProfilePicture { get; set; }
        string OpenUrl { get; set; }
        int AppThemeSetting { get; set; }

    }

    public partial class RadonAppSettings : ObservableObject, IAppSettings
    {
        [JsonIgnore]
        private RadonAppSettings _settings;
        [JsonIgnore]
        private readonly ISettingsService _settingsService;
        public RadonAppSettings(ISettingsService settingsService)
        {
            _settingsService = settingsService; 
        }

        public RadonAppSettings(RadonAppSettings settings)
        {
            _settings = settings;
        }

        public RadonAppSettings() {
            _settingsService = SettingsService.Instance; 
        }   

        private Uri _homeUrl;
        public Uri HomeUrl
        {
            get => _homeUrl;    
            set
            {
                SetProperty(ref _homeUrl, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private bool _sideKick;
        public bool SideKick
        {
            get => _sideKick;
            set
            {
                SetProperty(ref _sideKick, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private int _defaultSearchProvider;
        public int DefaultSearchProvider
        {
            get => _defaultSearchProvider;
            set
            {
                SetProperty(ref _defaultSearchProvider, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private bool _isLocationOnOff;
        public bool IsLocationOnOff
        {
            get => _isLocationOnOff;
            set
            {
                SetProperty(ref _isLocationOnOff, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _userAgentWebCore;
        public string UserAgentWebCore
        {
            get => _userAgentWebCore;
            set
            {
                SetProperty(ref _userAgentWebCore, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private ElementTheme _themeDefault;
        public ElementTheme ThemeDefault
        {
            get => _themeDefault;
            set
            {
                SetProperty(ref _themeDefault, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private Visibility _visibilitySideToolBar;
        public Visibility VisibilitySideToolBar
        {
            get => _visibilitySideToolBar;
            set
            {
                SetProperty(ref _visibilitySideToolBar, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private bool _titleBarPinned;
        public bool TitleBarPinned
        {
            get => _titleBarPinned;
            set
            {
                SetProperty(ref _titleBarPinned, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _homeUrlString;
        public string HomeUrlString
        {
            get => _homeUrlString;
            set
            {
                SetProperty(ref _homeUrlString, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _appColorTheme;
        public string AppColorTheme
        {
            get => _appColorTheme;
            set
            {
                SetProperty(ref _appColorTheme, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private bool _inlineMode;
        public bool InlineMode
        {
            get => _inlineMode;
            set
            {
                SetProperty(ref _inlineMode, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private bool _systemTitleBar;
        public bool SystemTitleBar
        {
            get => _systemTitleBar;
            set
            {
                SetProperty(ref _systemTitleBar, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _qrCodeUrl;
        public string QRCodeUrl
        {
            get => _qrCodeUrl;
            set
            {
                SetProperty(ref _qrCodeUrl, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _profilePicture;
        public string ProfilePicture
        {
            get => _profilePicture;
            set
            {
                SetProperty(ref _profilePicture, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private string _openUrl;
        public string OpenUrl
        {
            get => _openUrl;
            set
            {
                SetProperty(ref _openUrl, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }

        private int _appThemeSetting;
        public int AppThemeSetting
        {
            get => _appThemeSetting;
            set
            {
                SetProperty(ref _appThemeSetting, value);
                _settingsService.WriteSettings(this).ConfigureAwait(false);
            }
        }
    }

    class SettingsService : ISettingsService
    {
        private static readonly SemaphoreSlim __Locker_acids_events = new SemaphoreSlim(1, 1);

        public static SettingsService Instance { get; set; }
        public RadonAppSettings AppSettings { get; set; }
        public event EventHandler<NotifyCollectionChangedEventArgs> FavoritesCollectionChanged;
        public event EventHandler<NotifyCollectionChangedEventArgs> HistoryCollectionChanged;
        internal string CorePathFavorites { get; set; } = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER") + @"Favorites\";
        internal string CorePathSettings { get; set; } = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER") + @"Settings\";
        internal string CorePathHistory { get; set; } = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER") + @"History\";

        public SettingsService()
        {

            Instance = this;

            InitializeAsync();
            
            var path = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER");

            if (File.Exists(Path.Combine(path, "Settings", "settings.json")))
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(path, "settings", "settings.json"));
                if (fileInfo.Length <= 0)
                {
                    AppSettings = new() { DefaultSearchProvider = 0, HomeUrl = new Uri("https://google.com"), IsLocationOnOff = false, SideKick = false };
                    File.WriteAllText(Path.Combine(path, "settings", "settings.json"), JsonConvert.SerializeObject(AppSettings));
                }
            }
            else
            {

                using (var fs = File.Create(Path.Combine(path, "Settings", "settings.json"))) { } ;

            }
            // load from file. 
            
            
            FavoritesStore.CollectionChanged += FavoritesStore_CollectionChanged;
            HistoryStore.CollectionChanged += HistoryStore_CollectionChanged;

        }

        public async void InitializeAsync()
        {
            var settings = await ReadSettings<RadonAppSettings>();
            if (settings is not null)
            {
                AppSettings = settings;
            }
            else {
                AppSettings = new RadonAppSettings(this);
            }
             
            historyModels =  await ReadHistory<ObservableCollection<HistoryModel>>();
        }

        private void HistoryStore_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            lock (historyModels) {
                WriteHistory(historyModels.Distinct(new HistoryModelEqualityComparer())).ConfigureAwait(false);
                // this is bubbling up to historyviewmodel and mainpageviewmodel for myhistory collection. need to add to other pages sortly 
                HistoryCollectionChanged?.Invoke(this, e);
            }

        }

        private void FavoritesStore_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
            __Locker_acids_events.WaitAsync();

            FavoritesCollectionChanged?.Invoke(this, e);

            __Locker_acids_events.Release();
        }

        
        public void AddToHistory(HistoryModel historyItem)
        {
            lock (historyModels)
            {

                if (!historyModels.Any(h => h.TheUrl == historyItem.TheUrl))
                {
                    historyModels.Add(historyItem);
                    WriteHistory(historyModels.Distinct(new HistoryModelEqualityComparer())).ConfigureAwait(false);
                    HistoryCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, historyItem));
                }
                else {
                    
                    var existingItem = historyModels.First(h => h.TheUrl == historyItem.TheUrl);
                    historyModels.Remove(existingItem);
                    historyModels.Add(historyItem);
                    WriteHistory(historyModels.Distinct(new HistoryModelEqualityComparer())).ConfigureAwait(false);
                    HistoryCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, historyItem));
                }
            }
        }

        public void RemoveFromHistory(string key, string value)
        {
            //   throw new NotImplementedException();
        }

        public void RemoveFromHistory(HistoryModel historyItem)
        {
            lock (historyModels)
            {
                historyModels.Remove(historyItem);
                WriteHistory(historyModels.Distinct(new HistoryModelEqualityComparer())).ConfigureAwait(false);
                HistoryCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, historyItem));
            }
        }

        private class HistoryModelEqualityComparer : IEqualityComparer<HistoryModel>
        {
            public bool Equals(HistoryModel x, HistoryModel y)
            {
                return x.TheUrl == y.TheUrl;
            }

            public int GetHashCode(HistoryModel obj)
            {
                return obj.TheUrl.GetHashCode();
            }
        }
        public void Remove(string key)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder; 
                var file = await storageFolder.GetFileAsync(fileName);
                return file != null;
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }
        #region FileReadWriteAccessIO
        public async Task<T> ReadFavorites<T>()
        {
            string json = "{}";
            string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathFavorites);
            string fileFavs = Path.Combine(path, "favorites.json");

            try
            {

                if (File.Exists(fileFavs))
                {
                    json = File.ReadAllText(fileFavs);
                    var settings = JsonConvert.DeserializeObject<T>(json);
                    return await Task.FromResult<T>(settings);

                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    using (var fs = File.Create(fileFavs)) ;
                    return await Task.FromResult(JsonConvert.DeserializeObject<T>("[]"));

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<T> ReadSettings<T>()
        {
            string json = "{}";
            string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathSettings);
            string fileFavs = Path.Combine(path, "settings.json");

            try
            {

                if (File.Exists(fileFavs))
                {
                    json = File.ReadAllText(fileFavs);
                    var settings = JsonConvert.DeserializeObject<T>(json);
                    return await Task.FromResult<T>(settings);

                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    using (var fs = File.Create(fileFavs)) ;
                    return await Task.FromResult(JsonConvert.DeserializeObject<T>("{}"));

                }

            }
            catch (Exception)
            {

                throw;
            }



        }
        public async Task<T> ReadHistory<T>()
        {
            string json = "[]";
            string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathHistory);
            string fileHistory = Path.Combine(path, "history.json");

            try
            {
                if (File.Exists(fileHistory))
                {
                    json = File.ReadAllText(fileHistory);
                    var history = JsonConvert.DeserializeObject<List<HistoryModel>>(json);

                    if (history is null)
                    {
                        return await Task.FromResult(JsonConvert.DeserializeObject<T>("[]"));
                    }   

                    // Load BitmapImage for each HistoryModel

                    foreach (var item in history.ToList())
                    {
                        var bitmap = new BitmapImage(new Uri($"https://www.google.com/s2/favicons?domain_url={item.TheUrl.AbsoluteUri}"));
                        item.TheContents = bitmap ?? new BitmapImage(new("ms-appx://Assets/StoreLogo.png"));

                    }
                    return await Task.FromResult(JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(history)));
                }
                else
                {
                    var dir = Directory.CreateDirectory(path);
                    using (var fs = File.Create(fileHistory)) ;
                    return await Task.FromResult(JsonConvert.DeserializeObject<T>("[]"));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task WriteHistory<T>(T history)
        {
            try
            {
                var file = CorePathHistory + "history.json";

                if (File.Exists(file))
                {
                    var json = JsonConvert.SerializeObject(history);
                    await File.WriteAllTextAsync(file, json);
                }
                else
                {
                    Directory.CreateDirectory(CorePathSettings);

                    if (Directory.Exists(CorePathSettings))
                    {
                        using (var fs = File.Create(file)) { }
                        var json = JsonConvert.SerializeObject(history);
                        await File.WriteAllTextAsync(file, json);
                    }
                }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task WriteFavorites<T>(T favorites)
        {
            try
            {
                var file = CorePathFavorites + "favorites.json";

                if (File.Exists(file))
                {
                    var json = JsonConvert.SerializeObject(favorites);
                    await File.WriteAllTextAsync(file, json);
                }
                else
                {
                    Directory.CreateDirectory(CorePathSettings);

                    if (Directory.Exists(CorePathSettings))
                    {
                        using (var fs = File.Create(file)) { };
                        var json = JsonConvert.SerializeObject(favorites);
                        await File.WriteAllTextAsync(file, json);
                    }
                }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task WriteSettings<T>(T settings)
        {


            try
            {
                var file = CorePathSettings + "settings.json";

                if (File.Exists(file))
                {
                    var json = JsonConvert.SerializeObject(settings);
                    await File.WriteAllTextAsync(file, json);
                }
                else
                {
                    Directory.CreateDirectory(CorePathSettings);

                    if (Directory.Exists(CorePathSettings))
                    {
                        using (var fs = File.Create(file)) { };
                        var json = JsonConvert.SerializeObject(settings);
                        await File.WriteAllTextAsync(file, json);
                    }
                }
                return;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public void AddToHistory(string key, string value)
        {
            //throw new NotImplementedException();
        }

        #endregion

        private ObservableCollection<HistoryModel> historyModels = new ObservableCollection<HistoryModel>();
        public ObservableCollection<HistoryModel> HistoryStore
        {
            get
            {
                return historyModels?.OrderByDescending(t=> t.DateTime).ToObservableCollection() ?? new();   

                //var result = ReadHistory<ObservableCollection<HistoryModel>>().GetAwaiter().GetResult();
                //return result ?? new ObservableCollection<HistoryModel>();
            }
            set
            {
                lock (historyModels)
                {
                    historyModels.Clear(); 
                    foreach(var item in value)
                    {
                        historyModels.Add(item); 
                    }
                    WriteHistory(historyModels.Distinct(new HistoryModelEqualityComparer())).ConfigureAwait(false);
                    HistoryCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        
        public ObservableCollection<Bookmarks> FavoritesStore
        {
            get
            {
                var result = ReadFavorites<ObservableCollection<Bookmarks>>().GetAwaiter().GetResult();
                return result ?? new ObservableCollection<Bookmarks>();
            }
            set
            {
                var obj = new object();
                lock (obj)
                {
                    WriteFavorites(value).ConfigureAwait(false);

                    //Write(nameof(FavoritesStore), value);
                    FavoritesCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }

            }
        }
    }
}
