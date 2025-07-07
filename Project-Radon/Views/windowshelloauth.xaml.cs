using Project_Radon.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yttrium_browser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class windowshelloauth : Page
    {
        public windowshelloauth()
        {
            this.InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            authHelper();
        }
        private async void authHelper()
        {
            AuthButton.IsEnabled = false;
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();

            if (availability == UserConsentVerifierAvailability.Available)
            {
                var result = await UserConsentVerifier.RequestVerificationAsync("To continue to Radon, authenticate with Windows Hello.");

                if (result == UserConsentVerificationResult.Verified)
                {
                    await Task.Delay(1000);
                    this.Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    AuthButton.IsEnabled = true;
                }
            }
            else
            {
                AuthButton.IsEnabled = false;
                AuthButton.Content = "Windows Hello isn't set up";
            }
        }

        private void HelpLink_Click(object sender, RoutedEventArgs e)
        {
            RadonContentDialog dg = new RadonContentDialog();
            dg.TitleText = "Having trouble with Windows Hello?";
            dg.SubtitleText = "If you can't use Windows Hello on your device, visit Microsoft Support web or reset Radon via app settings.";
            dg.CloseButtonText = "OK";
            dg.ShowAsync();
        }
    }
}
