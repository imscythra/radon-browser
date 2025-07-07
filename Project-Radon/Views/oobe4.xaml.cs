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
using Windows.UI.Xaml.Media.Animation;


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
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (WindowsHelloSwitch.IsOn == true)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("Please verify your identity");

                if (result == UserConsentVerificationResult.Verified)
                {
                    Controls.RadonContentDialog dg = new Controls.RadonContentDialog();
                    dg.TitleText = "Success!";
                    dg.SubtitleText = "You can now use Windows Hello to log into Radon.";
                    dg.DialogIconGlyph = "\uE930";
                    dg.CloseButtonText = "OK";
                    localSettings.Values["WindowsHelloAuth"] = true;
                    await dg.ShowAsync();
                }
                else
                {
                    Controls.RadonContentDialog dg = new Controls.RadonContentDialog();
                    dg.TitleText = "Something happened.";
                    dg.SubtitleText = "We ran into some problems setting up Windows Hello, maybe give it another try?";
                    dg.DialogIconGlyph = "\uEA39";
                    dg.CloseButtonText = "OK";
                    localSettings.Values["WindowsHelloAuth"] = false;
                    await dg.ShowAsync();
                    WindowsHelloSwitch.IsOn = false;
                }
            }
            else { localSettings.Values["WindowsHelloAuth"] = false; }
        }

        private void DialogClose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            var dialog = FindParent<ContentDialog>(button);

            dialog?.Hide();
        }

        // 👇 Add the helper method here inside the class
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.oobe5), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight});
        }
    }
}
