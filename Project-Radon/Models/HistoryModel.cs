using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Project_Radon.Models.Interfaces;
using Project_Radon.ViewModels;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using System.Net.Http;
using Microsoft.UI.Xaml.Controls;

namespace Project_Radon.Models
{
    public class HistoryModel : ObservableObject, IHistoryModel
    {
        [JsonIgnore]
        public HistoryViewModel HistoryView { get; internal set; }

        [JsonIgnore]
        public RelayCommand<object> RemoveItem { get; internal set; }

        [JsonIgnore]
        public RelayCommand<object> ShareTarget { get; internal set; }

        private Uri _TheUrl = default;
        public Uri TheUrl { get { return _TheUrl; } set { SetProperty(ref _TheUrl, value); } }

        private string _FavIconUrl = default;
        public string FavIconUrl { get { return _FavIconUrl; } set { SetProperty(ref _FavIconUrl, value); } }


        private BitmapImage _TheContents = default;
        [JsonIgnore]
        public BitmapImage TheContents { get { return _TheContents; } set { SetProperty(ref _TheContents, value); } }

        private BitmapImage _WebViewPicture = default;
        [JsonIgnore]
        public BitmapImage WebViewPicture { get { return _WebViewPicture; } set { SetProperty(ref _WebViewPicture, value); } }

        private Page _ThePage = default;
        public Page ThePage { get { return _ThePage; } set { SetProperty(ref _ThePage, value); } }

        private UIElement _TheElement = default;
        public UIElement TheElement { get { return _TheElement; } set { SetProperty(ref _TheElement, value); } }

        private string _TheDocumentTitle = default;
        public string TheDocumentTitle { get { return _TheDocumentTitle; } set { SetProperty(ref _TheDocumentTitle, value); } }

        private int _Position = default;
        public int Position { get { return _Position; } set { SetProperty(ref _Position, value); } }

        private RenderTargetBitmap _Bitmap = default;
        [JsonIgnore]
        public RenderTargetBitmap Bitmap { get { return _Bitmap; } set { SetProperty(ref _Bitmap, value); } }

        private ObservableRecipient _ViewModel = default;
        public ObservableRecipient ViewModel { get { return _ViewModel; } set { SetProperty(ref _ViewModel, value); } }

        private DateTimeOffset _DateTimeOffset = default;
        public DateTimeOffset DateTime { get { return _DateTimeOffset; } set { SetProperty(ref _DateTimeOffset, value); } }

        public HistoryModel() { }

        public HistoryModel(string title, string _url, BitmapImage _contents, Page _page, int _position, BitmapImage _webviewPictre, ObservableRecipient _viewmodel = null)
        {
            TheDocumentTitle = title;
            TheUrl = new Uri(_url);
            TheContents = _contents;
            ThePage = _page;
            Position = _position;
            ViewModel = _viewmodel;
            DateTime = DateTimeOffset.Now;
            WebViewPicture = _webviewPictre;
        }

        public HistoryModel(string title, string _url, BitmapImage _contents, UIElement _uIElement, int _position, ObservableRecipient _viewmodel = null)
        {
            TheDocumentTitle = title;
            TheUrl = new Uri(_url);
            TheContents = _contents;
            TheElement = _uIElement;
            Position = _position;
            ViewModel = _viewmodel;
            DateTime = DateTimeOffset.Now;
        }

        public HistoryModel(string title, string _url)
        {

            TheDocumentTitle = title; 
            TheUrl = new Uri(_url); 
            DateTime = DateTimeOffset.Now; 
            FavIconUrl = $"https://www.google.com/s2/favicons?domain_url={_url}";

        }

        public bool Equals(HistoryModel other)
        {
            return TheUrl.AbsoluteUri == other.TheUrl.AbsoluteUri;
        }

        public override string ToString()
        {
            return TheUrl?.AbsoluteUri?.ToString();
        }
    }
}
