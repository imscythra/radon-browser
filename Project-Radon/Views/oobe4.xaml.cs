using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials.UI;
using System.Diagnostics;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class oobe4 : Page
    {
        public oobe4()
        {
            this.InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void SearchEngineBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["searchEngine"] = SearchEngineBox.SelectedIndex;
        }

        private void SearchEngineBox_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["searchEngine"] != null) { SearchEngineBox.SelectedIndex = (int)localSettings.Values["searchEngine"]; }
            else { SearchEngineBox.SelectedIndex = 0; }
            // 0 - google.com, 1 - bing.com, 2 - duckduckgo.com, 3 - ecosia.org, 4 - search.brave.com, 5 - perplexity.ai
        }

        private async void WindowsHelloSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();

            if (availability != UserConsentVerifierAvailability.Available)
            {
                WindowsHelloCard.Description = "Windows Hello is not set up on this device.";
                WindowsHelloSwitch.IsEnabled = false;
            }
        }

        private async void WindowsHelloSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (WindowsHelloSwitch.IsOn == true)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("Please verify your identity");

                if (result == UserConsentVerificationResult.Verified)
                {
                    ContentDialog dg = new ContentDialog();
                    dg.Title = "Success!";
                    dg.Content = "You can now use Windows Hello to sign into Radon.";
                    dg.CloseButtonText = "Dismiss";
                    await dg.ShowAsync();
                }
                else
                {
                    ContentDialog dg = new ContentDialog();
                    dg.Title = "Something went wrong";
                    dg.Content = $"Radon is unable to set up Windows Hello. Please try again later. Error: {result}";
                    dg.CloseButtonText = "Dismiss";
                    await dg.ShowAsync();
                    WindowsHelloSwitch.IsOn = false;
                }
            }
        }
    }
}
