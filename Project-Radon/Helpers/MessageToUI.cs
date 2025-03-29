
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;
using Yttrium_browser;

namespace Project_Radon.Helpers
{
    public class MessageToUI
    {
        private string Message { get; set; }
        private BitmapImage ViewImage { get; set; }
        private string Title { get; set; }
        private Uri UriViewd { get; set; }
        internal string CorePathWebView { get; set; } = @"WebDiveCore\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name! + @"\WebView\";
        public MessageToUI(string message)
        {
            Message = message;
        }

        public MessageToUI(string message, string title, BitmapImage bitmapImage, Uri uri)
        {
            Message = message;
            Title = title;
            ViewImage = bitmapImage;
            UriViewd = uri;
        }

        public async Task ShowMessageAndPicture()
        {
            await Task.Factory.StartNew(() =>
            {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                App.Current.MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    try
                    {
                        // Create the notification content
                        var builder = new ToastContentBuilder()
                            .AddText(this.Message)
                            .AddArgument("action", "viewDetails");

                        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CorePathWebView);
                        string webImage = Path.Combine(path, "view.png");

                        // Get the image file
                        if (File.Exists(webImage))
                        {
                            var url = new Uri(new Uri("file://"), webImage);
                            builder.AddInlineImage(url);
                        }

                        // Add arguments if UriViewd is not null
                        if (UriViewd != null)
                        {
                            builder.AddArgument("Uri", UriViewd.AbsoluteUri.ToString());
                        }

                        // Build the notification
                        var notification = builder.GetToastContent();

                        // Show the notification
                        ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(notification.GetXml()));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            });
        }

        public async Task ShowMessage()
        {
            await Task.Factory.StartNew(() =>
            {
                App.Current.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        var builder = new ToastContentBuilder()
                          .AddText(this.Message)
                          .SetToastDuration(ToastDuration.Short);

                        var notification = builder.GetToastContent();

                        ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(notification.GetXml()));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                });
            });
        }
    }
}
