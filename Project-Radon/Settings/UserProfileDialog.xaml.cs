using Microsoft.Extensions.DependencyInjection;
using Project_Radon.Contracts.Services;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Yttrium
{
    public sealed partial class UserProfileDialog : ContentDialog
    {
        private readonly ISettingsService settingsService;
        public UserProfileDialog()
        {
            settingsService = Yttrium_browser.App.Current.Services.GetService<ISettingsService>();

            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            
            Username_Display.Text = settingsService.AppSettings.Username;
        }

        private void pfpchanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pfppreview.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", (pfpchanged.SelectedItem as ComboBoxItem).Content.ToString(), ".png" })));


        }

        private void updateprofile_Click(object sender, RoutedEventArgs e)
        {

            Username_Display.Text = settingsService.AppSettings.Username;  
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            
            Username_Display.Text = settingsService.AppSettings.Username; 
        }

        private void KeyboardAccelerator_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            
            Username_Display.Text = settingsService.AppSettings.Username;
        }
    }
}
