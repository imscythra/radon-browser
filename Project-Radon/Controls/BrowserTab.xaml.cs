using Fluent.Icons;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Project_Radon;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Connectivity;
using Yttrium;
using Microsoft.Toolkit.Uwp.Notifications;
using Project_Radon.Settings;
using Windows.UI.Xaml.Media.Animation;
using System.Linq;
using Microsoft.Web.WebView2.Core;
using Project_Radon.Contracts.Services;
using Yttrium_browser;
using Microsoft.Extensions.DependencyInjection;

namespace Project_Radon.Controls
{
    public sealed partial class BrowserTab : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler<string> NewTabRequested = delegate { };
        internal void Set<T>(ref T obj, T value, string name = null)
        {
            obj = value;
            InvokePropertyChanged(name);
        }

        public void InvokePropertyChanged(string name = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string OriginalUserAgent;
        string GoogleSignInUserAgent;

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => Set(ref _IsLoading, value);
        }
        public bool CanGoBack => WebBrowser.CanGoBack;
        public bool CanGoFoward => WebBrowser.CanGoForward;
        public string WVBaseUri => IsCoreInitialized ? (WebBrowser.Source.Host.ToString()) : null;
        public string SourceUri => IsCoreInitialized ? (WebBrowser.Source.AbsoluteUri.ToLower().Contains("edge://") ? "radon://" + WebBrowser.Source.AbsoluteUri.Remove(0, 7) : WebBrowser.Source.AbsoluteUri) : "";
        public string Favicon => IsCoreInitialized && !IsLoading ? ("http://www.google.com/s2/favicons?domain=" + WebBrowser.Source.AbsoluteUri) : "https://raw.githubusercontent.com/microsoft/fluentui-system-icons/main/assets/Document/SVG/ic_fluent_document_48_regular.svg";
        public string Title => IsCoreInitialized && !IsLoading ? (WebBrowser.CoreWebView2.DocumentTitle ?? WebBrowser.Source.AbsoluteUri) : "Loading";
        public bool IsCoreInitialized { get; private set; }

        private readonly IWebViewService viewService;

        public BrowserTab()
        {
            InitializeComponent();


            var options = new CoreWebView2EnvironmentOptions();
            options.AdditionalBrowserArguments = "--edge-webview-optional-enable-uwp-regular-downloads";

            viewService = App.Current.Services.GetService<IWebViewService>(); // App.Current.Services.GetService<IWebViewService>();
            viewService.Initialize(WebBrowser);

            WebBrowser.Source = new Uri("edge://radon-ntp");

            //Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
            WebBrowser.CoreWebView2Initialized += delegate
            {
                IsCoreInitialized = true;
                OriginalUserAgent = WebBrowser.CoreWebView2.Settings.UserAgent;
                GoogleSignInUserAgent = OriginalUserAgent.Substring(0, OriginalUserAgent.IndexOf("Edg/"))
                .Replace("Mozilla/5.0", "Mozilla/4.0");
                WebBrowser.CoreWebView2.Settings.UserAgent = GoogleSignInUserAgent;
                WebBrowser.CoreWebView2.DocumentTitleChanged += (_, e) => InvokePropertyChanged();
                WebBrowser.CoreWebView2.SourceChanged += (_, e) => InvokePropertyChanged();
                WebBrowser.CoreWebView2.ContextMenuRequested += async (s, e) =>
                
                
                {
                    IList<CoreWebView2ContextMenuItem> menuList = e.MenuItems;
                    if (e.ContextMenuTarget.HasLinkUri)
                    {
                        CoreWebView2ContextMenuItem newItem = WebBrowser.CoreWebView2.Environment.CreateContextMenuItem("Open link in new tab", null, CoreWebView2ContextMenuItemKind.Command);
                        newItem.CustomItemSelected += (CoreWebView2ContextMenuItem sender, object args) =>
                        {
                            NewTabRequested.Invoke(s, e.ContextMenuTarget.LinkUri);
                        };
                        for (int index = 0; index < menuList.Count; index++)
                        {
                            if (menuList[index].Name == "openLinkInNewWindow")
                            {
                                menuList.RemoveAt(index);
                                break;
                            }
                        }
                    }
                };
            };

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String username = localSettings.Values["username"] as string;
            if (username != null)
            {
                ToolTip toolTip = new ToolTip
                {
                    Content = username
                };
                ToolTipService.SetToolTip(profileCenterToggle, toolTip);
            }


            ntpTimeDisplayService();

        }

        private async void ntpTimeDisplayService()
        {
            bool loop = true;
            while (loop == true)
            {
                ntpHourDisplay.Text = DateTime.Now.ToString("H:mm");
                ntpDateDisplay.Text = DateTime.Today.DayOfWeek.ToString()+", "+DateTime.Today.ToString("MMMM d"); ;
                await Task.Delay(2000);
            }
            

        }






        private void WebBrowser_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            ntpSearchBar.Text = string.Empty;
            IsLoading = false;
            if (WebBrowser.Source.AbsoluteUri.Contains("edge://"))
            {
                WebBrowser.Visibility = Visibility.Collapsed;
                ntpGrid.Visibility = Visibility.Visible;
                ntpbackgroundbrush.ImageSource = new BitmapImage(new Uri("https://bing.biturl.top/?resolution=1366&format=image&index=random&mkt=en-US"));
                HideOfflinePage();
            }
            else
            {
                if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable == false & !WebBrowser.Source.AbsoluteUri.Contains("edge://"))
                {
                    WebBrowser.Visibility = Visibility.Collapsed;
                    ntpGrid.Visibility = Visibility.Collapsed;
                    ShowOfflinePage();
                }
                else
                {
                    WebBrowser.Visibility = Visibility.Visible;
                    ntpGrid.Visibility = Visibility.Collapsed;
                    offlinePage.Visibility = Visibility.Collapsed;
                }
            }
        }
        public void Close() => WebBrowser.Close();

        private void WebBrowser_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            
            IsLoading = true;
            WebBrowser.Focus(FocusState.Pointer);
            WebBrowser.Focus(FocusState.Keyboard);
            //if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable == false)
            //{
            //    OfflineHandler();
            //}
        }

        private async void OfflineHandler()
        {
            bool isOffline = true;
            if (WebBrowser.Source.AbsoluteUri.Contains("edge://"))
            {
                    HideOfflinePage();
                    isOffline = true;
            }
            else if (!WebBrowser.Source.AbsoluteUri.Contains("edge://"))
            {
                ShowOfflinePage();
                netIconLoadIndicator.Opacity = 0.3;
                netIcon.Opacity = 0;
                while (isOffline == true)
                {
                    
                    await Task.Delay(1000);
                    // checks current connection status with glyphs to represent
                    if (NetworkHelper.Instance.ConnectionInformation.ConnectionType.ToString() == "Unknown")
                    {
                        pcToNet.Value = 0;
                        netIconLoadIndicator.Opacity = 0;
                        netIcon.Glyph = "\uEA39";
                        netIcon.Opacity = 0.3;
                    }

                    // check if Internet is back
                    if (NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable == true)
                    {
                        WebBrowser.Reload();

                        isOffline = false;
                        new ToastContentBuilder()
                           .AddArgument("action", "viewConversation")
                           .AddArgument("conversationId", 9813)
                               .AddButton(new ToastButton()
                               .SetContent("Dismiss"))
                           .AddText("You're now online!")
                           .AddText("The Internet connection has been restored. You can now continue browsing.")
                           .AddAttributionText("via Radon Browser")
                           .AddAppLogoOverride(new Uri("ms-appx:///Assets/StoreLogo.scale-100.png"), ToastGenericAppLogoCrop.Circle)
                           .Show();
                        await Task.Delay(500);

                    }
                }

            }
        }
        private async void HideOfflinePage()
        {
            offlinePage.Opacity = 0;
            await Task.Delay(500);
            offlinePage.Visibility = Visibility.Collapsed;
        }
        private void ShowOfflinePage()
        {
            netIconLoadIndicator.Opacity = 0.3;
            netIcon.Opacity = 0;
            offlinePage.Opacity = 1;
            offlinePage.Visibility = Visibility.Visible;
        }
        public async Task GoTo(string url)
        {
            Regex UrlMatch = new Regex("^(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$", RegexOptions.Singleline);
            await WebBrowser.EnsureCoreWebView2Async();
            if (url.ToLower().StartsWith("radon://"))
            {
                WebBrowser.Source = new Uri("edge://" + url.Remove(0, 7));
            }
            else if (UrlMatch.IsMatch(url) || url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("edge://"))
            {
                WebBrowser.Source = new Uri(!Uri.TryCreate(url, UriKind.Absolute, out var r) || !r.IsAbsoluteUri ? "https://" + url : url);
            }
        }
        public async Task SearchOrGoto(string SearchBarText)
        {
            Regex UrlMatch = new Regex("^(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$", RegexOptions.Singleline);
            await WebBrowser.EnsureCoreWebView2Async();
            if (SearchBarText.ToLower().StartsWith("radon://"))
            {
                WebBrowser.Source = new Uri("edge://" + SearchBarText.Remove(0, 7));
            }
            else if (UrlMatch.IsMatch(SearchBarText) || SearchBarText.StartsWith("https://") || SearchBarText.StartsWith("http://") || SearchBarText.StartsWith("edge://"))
            {
                WebBrowser.Source = new Uri(!Uri.TryCreate(SearchBarText, UriKind.Absolute, out var r) || !r.IsAbsoluteUri ? "https://" + SearchBarText : SearchBarText);
            }
            else
            {
                WebBrowser.Source = new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(SearchBarText));
            }
        }

        public async Task OpenDownloadsDialog()
        {
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.CoreWebView2.DefaultDownloadDialogCornerAlignment = Microsoft.Web.WebView2.Core.CoreWebView2DefaultDownloadDialogCornerAlignment.TopRight;
            WebBrowser.CoreWebView2.OpenDefaultDownloadDialog();
        }
        public async Task OpenDevTools()
        {
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.CoreWebView2.OpenDevToolsWindow();
        }
        public void BackButtonSender()
        {
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.GoBack();
            }
        }
        public void FowardButtonSender()
        {
            if (WebBrowser.CanGoForward)
            {
                WebBrowser.GoForward();
            }
        }

        public async void Task1()
        {

        }
        public void Reload()
        {
            WebBrowser.Reload();
        }
        public void Stop()
        {
            WebBrowser.CoreWebView2.Stop();
        }
        public async Task ExecuteScriptAsyc(string script)
        {
            await WebBrowser.EnsureCoreWebView2Async();
            await WebBrowser.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void ntpSearchBar_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(ntpSearchBar.Text))
            {
                WebBrowser.Source = new Uri("about:blank");
                ntpGrid.Visibility = Visibility.Collapsed;
                WebBrowser.Source = new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(ntpSearchBar.Text));
            }
        }

        private void ntpSearchBar_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private async void profileCenterToggle_Click(object sender, RoutedEventArgs e)
        {
            await new UserProfileDialog().ShowAsync();
        }
    }
}
