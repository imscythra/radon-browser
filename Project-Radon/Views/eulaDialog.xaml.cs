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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    public sealed partial class eulaDialog : ContentDialog
    {
        public eulaDialog()
        {
            this.InitializeComponent();
            setlicensetext();
        }

        public async void setlicensetext()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Windows.Storage.StorageFile file = await storageFolder.GetFileAsync(@"\Settings\License.txt");

            string text = await Windows.Storage.FileIO.ReadTextAsync(file);
            LicenseTextBox.Text = text;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
