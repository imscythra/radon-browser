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
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Project_Radon.Views;
using Windows.UI.Xaml.Media.Animation;
using Yttrium_browser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class oobeHost : Page
    {
        public oobeHost()
        {
            this.InitializeComponent();

            // Title bar code-behind
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        private void hostFrame_Loaded(object sender, RoutedEventArgs e)
        {
            hostFrame.Navigate(typeof(WelcomeScreen), null, new DrillInNavigationTransitionInfo());
        }

        private void debugSkip_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void oobeTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(oobeTitleBar);
        }
    }
}
