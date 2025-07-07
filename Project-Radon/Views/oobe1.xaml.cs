using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Project_Radon.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomeScreen : Page
    {
        public WelcomeScreen()
        {
            this.InitializeComponent();
        }

        

        private void migrateOptHandler()
        {

        }

        private void eulaBtn_Click(object sender, RoutedEventArgs e)
        {
            eulaDialog dialog = new eulaDialog();
            dialog.ShowAsync();
        }

        private void eulaCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(eulaCheckBox.IsChecked == true)  { nextBtn.IsEnabled = true ;}
            else { nextBtn.IsEnabled = false; }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Views.oobe3), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}
