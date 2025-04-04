using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using static System.Net.Mime.MediaTypeNames;
using Project_Radon.Contracts.Services;
using Yttrium_browser;
using Microsoft.Extensions.DependencyInjection;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWindowed : Page
    {
        private readonly ISettingsService settingsService;
        public BrowserWindowed()
        {
            settingsService = App.Current.Services.GetService<ISettingsService>();
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;

            webLoader();
        }
        private async void webLoader()
        {
            await BrowserView.EnsureCoreWebView2Async();
            string openUrl = settingsService.AppSettings.OpenUrl ?? "about:blank";
            BrowserView.Source = new Uri(openUrl);
        }

        private void BrowserView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            loadingIndicator.IsActive = true;
            if (BrowserView.CanGoBack == true) { backButton.Visibility = Visibility.Visible; } else { backButton.Visibility = Visibility.Collapsed; }
            refreshButton.Visibility = Visibility.Collapsed;
            stopButton.Visibility = Visibility.Visible;
        }

        private void BrowserView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            loadingIndicator.IsActive = false;
            refreshButton.Visibility = Visibility.Visible;
            stopButton.Visibility = Visibility.Collapsed;

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserView.GoBack();
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserView.Reload();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserView.CoreWebView2.Stop();
        }

        private void urlButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(BrowserView.Source.ToString());
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush(); // Ensures the content remains available after the app closes
        }
    }
}
