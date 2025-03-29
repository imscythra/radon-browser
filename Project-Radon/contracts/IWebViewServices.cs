
using Microsoft.Web.WebView2.Core;
using Project_Radon.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Controls;

namespace Project_Radon.Contracts.Services
{
    public interface IWebViewService
    {
        event EventHandler<CoreWebView2WebErrorStatus> NavigationCompleted;
        // use these to invoke, and then bubble up the chain ...
        event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigateStarted;
        event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigateCompleted;
        event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        event EventHandler<object> HistoryChanged;
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        public void ChangeServiceProvider(int SearchProvider);
        bool NavigationProgress { get; set; }

        bool IsLocationOnOff { get; set; }
        void Initialize(WebView2 webView);

        void UnregisterEvents();


        void GoBack();

        void GoForward();

        Task GoToSearchText(string QueryText);
        void Reload();

        //Task<IEnumerable<FrameworkElement>> GetPages(); 
        void GoHome();
        Uri DefaultUri { get; set; }

        string Address { get; }

        //Task<Uri> GetValidateUrl(string QueryText);
        Task<BitmapImage> GetPictureOfViewAsync();
        Task ClearCookiesCoreWebView2();
        Task ClearCacheCoreWebView2();
        Task ClearCoreHistory();
        Task<string> StopPageLoad();
        IAsyncOperation<string> ExecuteScriptAsync(string javascriptCode);

        WebView2 WebView { get; }

        ObservableCollection<HistoryModel> HistoryStore { get; set; }

        Task<HistoryModel> GatherWebViewInfoAsync();

        Uri AddressUrl { get; }
        BitmapImage AddressPicture { get; }
        BitmapImage WebViewPicture { get; }
    }
}
