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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class oobe3 : Page
    {
        public oobe3()
        {
            this.InitializeComponent();

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string pfpsrc = localSettings.Values["profilePicture"] as string;
            profilePicture.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc.ToString(), ".png" })));

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void themePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themePicker.SelectedIndex != 0) { themeDisplayText.Text = (string)(themePicker.SelectedItem as Image).Tag.ToString(); ; }
            else { themeDisplayText.Text = "None"; }

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["appcolortheme"] = themeDisplayText.Text;

        }

        private void profileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (profileName.Text != string.Empty)
            {
                ApplicationData.Current.LocalSettings.Values["username"] = profileName.Text;
            }
            else { ApplicationData.Current.LocalSettings.Values["username"] = "Radon user"; }
        }

        private async void profilepictureButton_Click(object sender, RoutedEventArgs e)
        {
            oobe3pfpdialog dg = new oobe3pfpdialog();
            await dg.ShowAsync();
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string pfpsrc = localSettings.Values["profilePicture"] as string;
            profilePicture.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc.ToString(), ".png" })));
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(oobe4), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void AppThemePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RestartTip.IsOpen = true;
            if (AppThemePicker.SelectedIndex == 0) { ApplicationData.Current.LocalSettings.Values["appDisplayTheme"] = null; }
            else if (AppThemePicker.SelectedIndex == 1) { ApplicationData.Current.LocalSettings.Values["appDisplayTheme"] = (int)0; }
            else if (AppThemePicker.SelectedIndex == 2) { ApplicationData.Current.LocalSettings.Values["appDisplayTheme"] = (int)1; }

        }

        private async void RestartTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            await CoreApplication.RequestRestartAsync("-fastInit -level 1 -foo"); ;
        }

        private void AppThemePicker_Loaded(object sender, RoutedEventArgs e)
        {
            AppThemePicker.SelectedIndex = 0;
        }
    }
}
