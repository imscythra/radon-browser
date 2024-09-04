using Project_Radon.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yttrium_browser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class tempcampaign : Page
    {
        public tempcampaign()
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "0");
            this.InitializeComponent();
            initCampaign();
        }

        private async void initCampaign()
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/fragile.mp3"));
            mediaPlayer.IsLoopingEnabled = true;
            mediaPlayer.Volume = 0.5;
            mediaPlayer.Play();
            await Task.Delay(1000);
            TitleText.Opacity = 1;
            await Task.Delay(3000);
            TitleText.Opacity = 0;
            await Task.Delay(1000);
            TitleText.Text = "Radon is about to be sunset.";
            TitleText.Opacity = 1;
            await Task.Delay(3000);
            SubtitleText.Visibility = Visibility.Visible;
            await Task.Delay(500);
            SubtitleText.Opacity = 1;
            await Task.Delay(1000);
            imageFilter.Opacity = 0.7;
            detailsArea.Opacity = 0;
            await Task.Delay(1500);
            detailsArea.Visibility = Visibility.Visible;
            await Task.Delay(500);
            detailsArea.Opacity = 1;
        }

        private async void donateActionButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Uri url = new Uri("https://ko-fi.com/imscythra");
            localSettings.Values["openUrl"] = "https://ko-fi.com/imscythra";

            // TODO: This is template to open new window lmao

            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(BrowserWindowed), null);
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void skipButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }
    }
}
