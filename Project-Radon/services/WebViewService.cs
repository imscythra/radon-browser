using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Windows.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Project_Radon.Contracts.Services;
using Project_Radon.Models;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Project_Radon.Helpers;
using Project_Radon.ViewModels.Providers;
using Newtonsoft.Json.Linq;




namespace Project_Radon.Services
{
    #region Online Knowledge 
    /*
        https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environment.createasync?view=webview2-dotnet-1.0.2088.41
        https://learn.microsoft.com/en-us/microsoft-edge/webview2/get-started/winui
        https://learn.microsoft.com/en-us/microsoft-edge/webview2/reference/win32/webview2-idl?view=webview2-1.0.1293.44#createcorewebview2environmentwithoptions
        https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
        https://peter.sh/experiments/chromium-command-line-switches/
    */
    #endregion

    public class WebViewService : ObservableObject, IWebViewService
    {
        #region Variables 

        private WebView2 _webView;

        private Uri homeURL = new("https://google.com/");

        public Uri DefaultUri { get { return homeURL; } set { SetProperty(ref homeURL, value); } }

        private int _usingSearProvider = default;
        public int UsingSearchProvider { get { return _usingSearProvider; } set { _usingSearProvider = value; SettingsService.DefaultSearchProvider = value; } }
        public bool IsLocationOnOff { get; set; }

        public string Address =>
            _webView.Source?.AbsoluteUri?.ToString();
        public bool CanGoBack
            => _webView.CanGoBack;

        public bool CanGoForward
            => _webView.CanGoForward;

        private Uri _AddressUrl = default;
        public Uri AddressUrl { get { return _AddressUrl; } set { _AddressUrl = value; } }

        private BitmapImage _AddressPicture = new BitmapImage();
        public BitmapImage AddressPicture { get { return _AddressPicture; } set { _AddressPicture = value; } }

        private BitmapImage _WebViewPicture = new BitmapImage();
        public BitmapImage WebViewPicture { get { return _WebViewPicture; } set { _WebViewPicture = value; } }

        internal string CorePathWebView { get; set; } = @"RadonCore\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name! + @"\Service_WebviewController\";

        public bool NavigationProgress { get; set; }

        private readonly ISettingsService SettingsService;

        WebView2 IWebViewService.WebView => _webView;

        private ObservableCollection<HistoryModel> historyModels = new ObservableCollection<HistoryModel>();
        public ObservableCollection<HistoryModel> HistoryStore { get { return historyModels; } set { historyModels = value; } }

        #endregion
        #region Events_Handle_Webview2


        public event EventHandler<CoreWebView2WebErrorStatus> NavigationCompleted;
        public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigateStarted;
        public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigateCompleted;
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        public event EventHandler<object> HistoryChanged;
        #endregion

        public WebViewService(ISettingsService settingsService, IMessenger messenger)
        {
            SettingsService = settingsService;
            HistoryStore = SettingsService.HistoryStore;
            SettingsService.HistoryCollectionChanged += SettingsService_HistoryCollectionChanged;
            UsingSearchProvider = settingsService.DefaultSearchProvider;

        }

        private void SettingsService_HistoryCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Active Instance's HistoryStore will directly -> catch acid effects on collection to update binding. 

            lock (HistoryStore) {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        //HistoryStore = SettingsService.HistoryStore;
                        OnPropertyChanged(nameof(HistoryStore));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        OnPropertyChanged(nameof(HistoryStore));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        HistoryStore = SettingsService.HistoryStore;
                        OnPropertyChanged(nameof(HistoryStore));
                        break;
                    default:
                        break;
                }
            }
            
        }

        public void ChangeServiceProvider(int SearchProvider)
        {
            UsingSearchProvider = SearchProvider;
        }

        private async void SaveViewToDisk(byte[] drawBytes)
        {

            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1))
            {

                await semaphoreSlim.WaitAsync();

                string path = Path.Combine(Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER"), CorePathWebView);
                string fileFavs = Path.Combine(path, "view.png");

                try
                {
                    if (Directory.Exists(path))
                    {
                        File.WriteAllBytes(fileFavs, drawBytes);
                    }
                    else
                    {
                        var dir = Directory.CreateDirectory(path);
                        File.WriteAllBytes(fileFavs, drawBytes);
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    semaphoreSlim.Release();
                }

            }
            ;

        }

        public async Task<BitmapImage> GetPictureOfViewAsync()
        {

            try
            {
                BitmapImage bitmap = new BitmapImage();


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    try
                    {
                        await _webView?.CoreWebView2?.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Jpeg, memoryStream.AsRandomAccessStream());
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        bitmap.SetSource(memoryStream.AsRandomAccessStream());

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        SaveViewToDisk(memoryStream.GetBuffer());

                        memoryStream.Seek(0, SeekOrigin.Begin);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                    return await Task.FromResult(bitmap);

                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<IRandomAccessStream> GetFavoriteIcon()
        {

            return await _webView?.CoreWebView2.GetFaviconAsync(CoreWebView2FaviconImageFormat.Jpeg);

        }
        public async Task<string> GetFavIcon()
        {

            try
            {
                var answer = await ExecuteScriptAsync(@"(() => {
                        let faviconURI = '';
                        let links = document.getElementsByTagName('link');
                        Array.from(links).map(element => {
                            let rel = element.rel;
                            if (rel && (rel == 'shortcut icon' || rel == 'icon')) {
                                if (!element.href) {
                                    return;
                                }
                                try {
                                    let urlParser = new URL(element.href);
                                    faviconURI = urlParser.href;
                                } catch(e) {
                                    let origin = window.location.origin;
                                    let faviconLocation = `${origin}/${element.href}`;
                                    try {
                                        urlParser = new URL(faviconLocation);
                                        faviconURI = urlParser.href;
                                    } catch (e2) {
                                        return;
                                    }
                                }
                            }
                        });
                        return faviconURI;
                    })();");

                return answer;

            }
            catch (Exception)
            {
                return String.Empty;
            }



        }
        public async Task<HistoryModel> GatherWebViewInfoAsync()
        {
            string title = default;
            await _webView.EnsureCoreWebView2Async();

            try
            {
                try
                {
                    // use the default from the api, 11-1-23 testing; 
                    BitmapImage image = new BitmapImage(new(_webView.CoreWebView2?.FaviconUri));
                    title = _webView.CoreWebView2?.DocumentTitle;
                    return new HistoryModel(title, _webView.Source.AbsoluteUri);

                }
                catch (Exception)
                {

                    string favLocation = await GetFavIcon();
                    if (!string.IsNullOrEmpty(favLocation))
                    {
                        var json = JsonConvert.DeserializeObject(favLocation)?.ToString();

                        BitmapImage favStream;
                        if (string.IsNullOrEmpty(json))
                        {
                            favStream = await GetFavoriteBitmap($"https://www.google.com/s2/favicons?domain_url={_webView.Source.AbsoluteUri}");
                        }
                        else
                        {
                            favStream = await GetFavoriteBitmap(json);
                        }

                        title = _webView.CoreWebView2?.DocumentTitle;
                        //BitmapImage view = await GetPictureOfViewAsync();
                        return new HistoryModel(title, _webView.Source.AbsoluteUri);
                    }
                    else
                    {
                        return new HistoryModel(title, _webView.Source.AbsoluteUri);
                    }
                }

            }
            catch (Exception)
            {
                return new HistoryModel(title, _webView.Source.AbsoluteUri.ToString(), null, null, 0);
            }


        }
        public BitmapImage Base64StringToBitmap()
        {
            string base64String = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAQklEQVQ4jWP87if3nwEH4Nz0iLF+FwNO+UY3BkYmXJLEglEDqGAAxYCRYcl7nPHMECPI+H8n7nTA6D6aDgaHARQDAKgRDRsLiHU6AAAAAElFTkSuQmCC";
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bitmapImage;
        }
        async Task<BitmapImage> GetFavoriteBitmap(string imageUrl)
        {
            try
            {
                HttpClient client = new System.Net.Http.HttpClient();
                HttpResponseMessage imageResponse = await client.GetAsync(imageUrl);
                InMemoryRandomAccessStream randomAccess = new InMemoryRandomAccessStream();

                Windows.Storage.Streams.DataWriter writer =
                new Windows.Storage.Streams.DataWriter(randomAccess.GetOutputStreamAt(0));

                writer.WriteBytes(await imageResponse.Content.ReadAsByteArrayAsync());
                await writer.StoreAsync();
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(randomAccess);
                return bitmap;
            }
            catch (Exception)
            {
                return Base64StringToBitmap();
            }
        }

        private async void CoreWebView2_HistoryChanged(CoreWebView2 sender, object args)
        {


            try
            {
                if (_webView.Source is not null)
                    if (_webView.Source.OriginalString! == "about:blank" || _webView.Source.OriginalString.StartsWith("edge://"))
                        return;
                    else
                    {
                        await SaveHistoryToLocalStorageAsync(new HistoryModel(_webView.CoreWebView2?.DocumentTitle, _webView.Source?.AbsoluteUri));
                    }
            }
            catch (Exception)
            {
                return;
            }
        }


        private async void OnWebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            // wait for loading of page.. 
            try
            {
                if (_webView.Source is not null)
                    if (_webView.Source.OriginalString! == "about:blank" || _webView.Source.OriginalString.StartsWith("edge://"))
                        return;

                // not used capture picture -
                // make so not duplicates.  youtube iframes are not captured here - capture in the starting event. 

                await Task.Factory.StartNew(async () =>
                {
                    //await Task.Delay(640); //capture picture of page.
                    try
                    {
                        _webView.Dispatcher?.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async ()
                        =>
                        {

                            HistoryModel item = await GatherWebViewInfoAsync();
                            AddressPicture = item.TheContents;
                            AddressUrl = string.IsNullOrEmpty(_webView.CoreWebView2?.FaviconUri) ? new Uri(string.Format("https://www.google.com/s2/favicons?domain_url={0}", _webView.Source.Host.ToString())) : new Uri(_webView.CoreWebView2?.FaviconUri);

                            SettingsService.AddToHistory(item);
                            // internal listener will fire Handlers =>4 subscribers 
                            // listener will also handle property changes locally. 
                            OnPropertyChanged(nameof(CanGoBack));
                            OnPropertyChanged(nameof(CanGoForward));
                            NavigationCompleted?.Invoke(this, args.WebErrorStatus);
                            NavigationProgress = false;
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message!);
                    }

                    return Task.CompletedTask;

                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message!);
            }

        }
        public async Task SaveHistoryToLocalStorageAsync(HistoryModel item)
        {
            try
            {
                string jsonItem = JsonConvert.SerializeObject(item);
                string script = $@"
                                    (function() {{
                                        console.log('Saving history to local storage');
                                        localStorage.setItem('radon_edge_history', '{jsonItem}');
                                        console.log('History saved');
                                    }})();
                                ";

                await _webView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history to local storage: {ex.Message}");
            }
        }



        public void Initialize(WebView2 webView)
        {
            _webView = webView;
            _webView.Source = DefaultUri;
            _webView.NavigationStarting += OnWebViewProgressStartingAsync;
            _webView.NavigationCompleted += OnWebViewProgressCompleted;
            _webView.NavigationCompleted += OnWebViewNavigationCompleted;
            _webView.CoreWebView2Initialized += CoreWebView2Initialized;
            HistoryStore = SettingsService.HistoryStore ?? new ObservableCollection<HistoryModel>();


        }

        // scribe for progress rings.
        private void OnWebViewProgressStartingAsync(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            NavigateStarted?.Invoke(this, args);
        }
        private void OnWebViewProgressCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            NavigateCompleted?.Invoke(this, args);
        }

        private async void CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {

            //sender.Source = homeURL;
            await sender.EnsureCoreWebView2Async();

            if (!(args.Exception is Exception))
            {
                if (_webView.CoreWebView2 != null)
                {
                    _webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
                    _webView.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
                    _webView.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
                    _webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
                    _webView.CoreWebView2.Profile.PreferredTrackingPreventionLevel = CoreWebView2TrackingPreventionLevel.Balanced;
                    _webView.CoreWebView2.Settings.IsPinchZoomEnabled = true;
                    _webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = true;

                }
            }
        }
        private async void CoreWebView2_DOMContentLoaded(CoreWebView2 sender, CoreWebView2DOMContentLoadedEventArgs args)
        {

            if (sender is CoreWebView2 wb)
            {
                await wb.ExecuteScriptAsync(@"(function () {
                        try{
                            [].forEach.call(document.querySelectorAll('a[target=_blank]'), function (link) { link.setAttribute('target', '_self');});
                        }catch(e){
                            console.error(e); 
                        }
                })()"
                );
            }

        }
        private async void CoreWebView2_PermissionRequested(CoreWebView2 sender, CoreWebView2PermissionRequestedEventArgs args)
        {

            switch (args.PermissionKind)
            {
                case CoreWebView2PermissionKind.UnknownPermission:
                    break;
                case CoreWebView2PermissionKind.Microphone:
                    break;
                case CoreWebView2PermissionKind.Camera:
                    break;
                case CoreWebView2PermissionKind.Geolocation:

                    var def = args.GetDeferral();

                    try
                    {
                        args.State = SettingsService.IsLocationOnOff ? CoreWebView2PermissionState.Allow : CoreWebView2PermissionState.Deny;
                        args.Handled = true;
                        def.Complete();
                    }
                    catch (Exception e)
                    {

                        await new ErrorsToUI(e).SendMessage();

                    }

                    break;
                case CoreWebView2PermissionKind.Notifications:
                    break;
                case CoreWebView2PermissionKind.OtherSensors:
                    break;
                case CoreWebView2PermissionKind.ClipboardRead:
                    break;
                default:
                    break;
            }

        }
        private void CoreWebView2_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            // generic handle on subscriber let them handle -> abstract in line
            NewWindowRequested?.Invoke(this, args);

        }
        public void UnregisterEvents()
        {
            _webView.CoreWebView2?.Stop();
            _webView.NavigationCompleted -= OnWebViewNavigationCompleted;
            _webView.NavigationStarting -= OnWebViewProgressStartingAsync;
            _webView.NavigationCompleted -= OnWebViewProgressCompleted;
            if (_webView.CoreWebView2 != null)
            {
                _webView.CoreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
                _webView.CoreWebView2.PermissionRequested -= CoreWebView2_PermissionRequested;
                _webView.CoreWebView2.HistoryChanged -= CoreWebView2_HistoryChanged;
                _webView.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
            }
            _webView.CoreWebView2Initialized -= CoreWebView2Initialized;

        }

        public void GoHome()
        {
            _webView.Source = DefaultUri;
        }
        public void GoBack()
            => _webView.GoBack();

        public void GoForward()
            => _webView.GoForward();

        public void Reload()
            => _webView.Reload();

        public async Task<string> StopPageLoad()
        {
            return await _webView.CoreWebView2?.CallDevToolsProtocolMethodAsync("Page.stopLoading", "{}");
        }

        //public async Task<string> BlockUrlsCoreWebView2()
        //{

        //    dynamic obj = new JObject();
        //    obj.urls = new JArray("https://ebay.com", "https://yahoo.com");

        //    string blocking = JsonConvert.SerializeObject(obj);

        //    return await _webView.CoreWebView2?.CallDevToolsProtocolMethodAsync("Network.setBlockedURLs", blocking);
        //}

        //public async Task<object> GetPostionWebViewView()
        //{

        //    var postion = await _webView.ExecuteScriptAsync(@"(() => { 

        //                    const getScrollPosition = {
        //                      x: window.pageXOffset !== undefined ? window.pageXOffset : window.scrollLeft,
        //                      y: window.pageYOffset !== undefined ? window.pageYOffset : window.scrollTop
        //                    };

        //                    return getScrollPosition; 
        //     })()");

        //    var answer = JsonConvert.DeserializeObject(postion).ToString();
        //    return answer;

        //}
        public async Task<string> ZoomingCoreWebView2()
        {
            dynamic obj = new JObject();
            obj.width = 300;
            obj.height = 400;
            obj.deviceScaleFactor = false;
            obj.mobile = false;
            var factors = JsonConvert.SerializeObject(obj);
            return await _webView.CoreWebView2?.CallDevToolsProtocolMethodAsync("Emulation.clearDeviceMetricsOverride", factors);

        }

        //  for subscribers control to show whatever message to the UI.
        public async Task ShowMessageShellPage(string contentMessage)
        {

            await new MessageToUI(contentMessage).ShowMessage();


        }

        public async Task ClearCookiesCoreWebView2()
        {
            await _webView.CoreWebView2?.CallDevToolsProtocolMethodAsync("Network.clearBrowserCookies", "{}");
            await ShowMessageShellPage("Cookie Manager cleared all Cookies !");

        }

        public async Task ClearCoreHistory()
        {
            await _webView.CoreWebView2?.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.BrowsingHistory);
            await ShowMessageShellPage("Browsing History has been deleted!");

        }
        public async Task ClearCacheCoreWebView2()
        {
            await _webView.CoreWebView2?.CallDevToolsProtocolMethodAsync("Network.clearBrowserCache", "{}");
            await ShowMessageShellPage("All Cache has been cleared!");
        }

        public async Task<Uri> GetValidateUrl(string QueryText)
        {

            Uri uriOut = default;

            System.Uri sendUri = null;
            Regex regex = new Regex(@"^(http|https|ms-appx|ms-appx-web|ftp)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", RegexOptions.IgnoreCase);
            var exists = regex.Match(QueryText);
            if (exists.Length > 0)
            {

                if (!System.Uri.TryCreate(QueryText, UriKind.RelativeOrAbsolute, out sendUri))
                {
                    exists = regex.Match(sendUri.ToString());

                    if (exists.Length <= 0)
                    {
                        var provider = SearchProviders.ProvidersList.Where((x) => x.Index == UsingSearchProvider).FirstOrDefault();
                        var gsearch = provider.ProviderUrl;
                        var test = System.Net.WebUtility.UrlEncode(QueryText);
                        string gsort = gsearch + test;
                        exists = regex.Match(gsort);
                        if (exists.Length <= 0)
                        { await new ErrorsToUI(new Exception("Invalid url format.\n" + QueryText)).SendMessage(); return null; }
                        QueryText = gsort;
                        uriOut = new Uri(QueryText);

                    }

                }
                else
                {
                    uriOut = new UriBuilder(sendUri.ToString()).Uri;
                }
            }
            else
            {

                var provider = SearchProviders.ProvidersList.Where((x) => x.Index == UsingSearchProvider).FirstOrDefault();

                var gsearch = provider.ProviderUrl;
                var test = System.Net.WebUtility.UrlEncode(QueryText);
                string gsort = gsearch + test;
                exists = regex.Match(gsort);
                if (exists.Length <= 0)
                { await new ErrorsToUI("Invalid url format.\n" + QueryText).SendMessage(); return null; }
                QueryText = gsort;
                uriOut = new Uri(QueryText);
            }

            return uriOut;

        }

        // anyone IOC subscribes can call this to search under the provider that is set in AppSettings
        async Task IWebViewService.GoToSearchText(string QueryText)
        {
            Uri uriOut = default;

            try
            {
                uriOut = await GetValidateUrl(QueryText);

                if (uriOut is null) throw new ArgumentNullException(nameof(uriOut));

                switch (SettingsService.SideKick)
                {
                    case true:
                        await SendToSearchWindowAsync(uriOut.ToString());
                        break;
                    default:
                        _webView.Source = uriOut;
                        break;
                }




            }
            catch (Exception ex)
            {
                await new ErrorsToUI(new Exception("Web Searching input was invalid\n " + ex?.Message)).SendMessage();

                return;
            }
            await Task.CompletedTask;
        }

        // used to subscribe to hide window that control whatever
        internal async Task SendToSearchWindowAsync(string uri)
        {
            //var commander = Ioc.Default.GetService<CommanderWebView>();
            //await commander.OpenSearchInNewWindowAsync(uri);
            await Task.Delay(0); 
        }

        // internal thread safe java => the subscribers webview (Wrap)
        //public Windows.Foundation.IAsyncOperation<string> ExecuteScriptAsync(string javascriptCode)
        //{
        //    return _webView.ExecuteScriptAsync(javascriptCode);
        //}

        public async Task<string> ExecuteScriptAsync(string javascriptCode)
        {
            string result = string.Empty;

            await _webView.Dispatcher.TryRunAsync( Windows.UI.Core.CoreDispatcherPriority.High , async () =>
            {
                result = await _webView.ExecuteScriptAsync(javascriptCode);
            });

            return result;
        }
    }
};

