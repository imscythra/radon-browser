using Microsoft.Extensions.DependencyInjection;
using Project_Radon.Contracts.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Yttrium_browser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class oobe3 : Page
    {
        private readonly ISettingsService _settingsService; 
        public oobe3()
        {
            _settingsService = App.Current.Services.GetService<ISettingsService>(); 
            this.InitializeComponent();

            
            string pfpsrc = _settingsService.AppSettings.ProfilePicture ?? "bot";

            profilePicture.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc, ".png" })));

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void themePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themePicker.SelectedIndex != 0) { themeDisplayText.Text = (string)(themePicker.SelectedItem as Image).Tag.ToString(); ; }
            else { themeDisplayText.Text = "None"; }

            _settingsService.AppSettings.AppColorTheme = themeDisplayText.Text;

        }

        private void profileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (profileName.Text != string.Empty)
            {
                _settingsService.AppSettings.Username = profileName.Text;
            }
            else { _settingsService.AppSettings.Username =  "Radon user"; }
        }

        private async void profilepictureButton_Click(object sender, RoutedEventArgs e)
        {
            oobe3pfpdialog dg = new oobe3pfpdialog();
            await dg.ShowAsync();

            string pfpsrc = _settingsService.AppSettings.ProfilePicture ?? "bot";
            profilePicture.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc.ToString(), ".png" })));
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(oobe4), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
