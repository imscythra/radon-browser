using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QRCoder;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;
using Windows.Storage;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Views
{
    public sealed partial class qrcodedialog : ContentDialog
    {
        public qrcodedialog()
        {
            this.InitializeComponent();
            qrGenHandler();
        }

        

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private async void qrGenHandler()
        {

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String text = localSettings.Values["qrUrl"] as string;

            // Step 1: Generate QR code bitmap using QRCoder
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(qrData))
                {
                    // Generate PNG byte array of the QR code
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);

                    // Step 2: Convert byte array to a BitmapImage
                    var bitmapImage = new BitmapImage();
                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        await stream.WriteAsync(qrCodeBytes.AsBuffer());
                        stream.Seek(0);

                        bitmapImage.SetSource(stream);
                    }

                    // Step 3: Display the BitmapImage in the Image control
                    qrImage.Source = bitmapImage;
                }
            }
        }
    }
}
