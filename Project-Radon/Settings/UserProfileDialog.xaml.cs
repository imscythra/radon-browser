using Microsoft.Extensions.DependencyInjection;
using Project_Radon.Contracts.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel;
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
        public ObservableCollection<string> ProfilePictures { get; private set; }
        public UserProfileDialog()
        {
            settingsService = Yttrium_browser.App.Current.Services.GetService<ISettingsService>();
            InitializeComponent();
            LoadProfilePictures();
            pfppreview.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", settingsService.AppSettings.ProfilePicture ?? "default", ".png" })));
        }


        private void LoadProfilePictures()
        {
            ProfilePictures = new ObservableCollection<string>();
            var folder = Path.Combine(Package.Current.InstalledLocation.Path, "accountpictures");
            var files = Directory.GetFiles(folder, "*.png");

            foreach (var file in files)
            {
                ProfilePictures.Add(Path.GetFileNameWithoutExtension(file));
            }
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
            pfppreview.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpchanged.SelectedItem?.ToString() ?? "default", ".png" })));
            settingsService.AppSettings.ProfilePicture = pfpchanged.SelectedItem?.ToString() ?? "default"; 

        }

        private void updateprofile_Click(object sender, RoutedEventArgs e)
        {

            Username_Display.Text = settingsService.AppSettings.Username;  
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            
            Username_Display.Text = settingsService.AppSettings.Username; 
        }

        private void KeyboardAccelerator_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            
            Username_Display.Text = settingsService.AppSettings.Username;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide(); 
        }
    }
}
