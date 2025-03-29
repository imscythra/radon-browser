using Windows.UI.Xaml.Controls;

namespace Project_Radon.Helpers
{
    public static class FrameExtensions
    {
        public static object GetPageViewModel(this Frame frame)
            => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
    }
}
