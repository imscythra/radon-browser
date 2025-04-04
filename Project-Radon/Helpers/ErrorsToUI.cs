
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Project_Radon.Services.Messages;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Notifications;
using Yttrium_browser;

namespace Project_Radon.Helpers
{
    public class ErrorsToUI : Exception
    {
        public Point thepoint = new Point(48, 10);
        private Exception inerror { get; set; }

        public override string Message
        {
            get
            {
                string inner = null;
                if (inerror != null)
                    inner = inerror.Message;
                else
                    inner = base.Message;

                return inner + "\r\n";
            }
        }

        public override Exception GetBaseException()
        {
            return inerror;
        }

        public ErrorsToUI(Exception errorTracked)
        {
            inerror = errorTracked;
        }

        public ErrorsToUI() : base()
        {
        }

        public ErrorsToUI(string message) : base(message)
        {

        }

        public ErrorsToUI(string message, Exception innerException) : base(message, innerException)
        {
        }

        public async Task SendMessage()
        {
            try
            {

                await Task.Factory.StartNew(() =>
                {
                    try
                    {

                        var toastContent = new ToastContentBuilder()
                           .AddArgument("action", "ToastClick")
                           .AddArgument("UserStatus", ((int)EnumMessageStatus.Updated).ToString())
                           .AddAppLogoOverride(new Uri("file://" + App.GetFullPathToAsset("StoreLogo.png")), ToastGenericAppLogoCrop.Circle)
                           .AddText("Error Occurred")
                           .AddText(this.Message!)
                           .GetToastContent();

                        var toastNotification = new ToastNotification(toastContent.GetXml());
                        ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                });


            }
            catch {; }
        }


    }
}
