using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        private void opt1_Click(object sender, RoutedEventArgs e)
        {
            if (opt1.IsChecked == true) { opt2.IsChecked = false; page2nextBtn.IsEnabled = true; }
            else { page2nextBtn.IsEnabled = false; }
        }

        private void opt2_Click(object sender, RoutedEventArgs e)
        {
            if (opt2.IsChecked == true) { opt1.IsChecked = false; page2nextBtn.IsEnabled = true; }
            else { page2nextBtn.IsEnabled = false; }
        }

        private void migrateOptHandler()
        {

        }
    }
}
