﻿using System;
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using static System.Net.Mime.MediaTypeNames;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserWindowed : Page
    {
        public BrowserWindowed()
        {
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            // TODO Windows.UI.ViewManagement.ApplicationView is no longer supported. Use Microsoft.UI.Windowing.AppWindow instead. For more details see https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/windowing
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;

            webLoader();
        }
        private async void webLoader()
        {
            await BrowserView.EnsureCoreWebView2Async();
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            string openUrl = localSettings.Values["openUrl"].ToString();
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
