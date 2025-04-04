
using Windows.UI.Xaml.Controls;

namespace Project_Radon.Contracts.Services
{
    public interface IRightPaneService
    {
        void OpenInRightPane(string pageKey, object parameter = null);

        void Initialize(Frame rightPaneFrame, SplitView splitView);

        void CleanUp();
    }
}
