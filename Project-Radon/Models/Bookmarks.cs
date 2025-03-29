using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Project_Radon.Helpers;


namespace Project_Radon.Models
{
    public class Bookmarks : ObservableRecipient
    {
        private string _BoookmarkTitle = default;
        public string BookmarkTitle { get { return _BoookmarkTitle; } set { SetProperty(ref _BoookmarkTitle, value); } }

        private Uri _BookmarkName = default;
        public Uri BookmarkName { get { return _BookmarkName; } set { SetProperty(ref _BookmarkName, value); } }

        private Uri _BookmarkIconUrl = default;
        public Uri BookmarkIconUrl { get { return _BookmarkIconUrl; } set { SetProperty(ref _BookmarkIconUrl, value); } }
        public event EventHandler<NotifyCollectionChangedEventArgs> FavoritesCollectionChanged;
        public Bookmarks(string bookmarktitle, Uri bookmarkname, Uri UrlBookMarkBitmap)
        {
            BookmarkName = bookmarkname;
            BookmarkTitle = bookmarktitle;
            BookmarkIconUrl = UrlBookMarkBitmap;
        }
        private void Instance_FavoritesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            FavoritesCollectionChanged?.Invoke(this, e);
        }

        private IDisposable tokenMessageStatus { get; set; }
        public Bookmarks()
        {

        }
        public Bookmarks(IMessenger messenger) : base(messenger) { }

        RelayCommand<Bookmarks> _RemoveBookmark = null;
        public RelayCommand<Bookmarks> RemoveBookmark
        {
            get
            {
                if (_RemoveBookmark != null)
                    return _RemoveBookmark;
                _RemoveBookmark = new RelayCommand<Bookmarks>
                (
                        async o =>
                        {
                            try
                            {
                                var favorites = Services.SettingsService.Instance.FavoritesStore;

                                Bookmarks book = o as Bookmarks;
                                foreach (Bookmarks x in favorites)
                                {
                                    if (x.BookmarkName == book.BookmarkName)
                                    {
                                        var index = favorites.IndexOf(x);
                                        var item = favorites[index];
                                        if (index >= 0)
                                        {
                                            favorites.RemoveAt(index);
                                            break;
                                        }
                                    }
                                };

                                Services.SettingsService.Instance.FavoritesStore = favorites;

                            }
                            catch (Exception e)
                            {
                                await Task.Run(() => new ErrorsToUI(e).SendMessage());

                            }

                        },
                        o => true
                );
                this.PropertyChanged += (s, e) => _RemoveBookmark.NotifyCanExecuteChanged();
                return _RemoveBookmark;
            }

        }

    }
}