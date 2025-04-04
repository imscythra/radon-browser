using Project_Radon.Helpers; 
using Project_Radon.Contracts.Services;
using Project_Radon.Contracts.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Yttrium_browser;


namespace Project_Radon.Services
{
    public class RightPaneService : IRightPaneService
    {
        private readonly IPageService _pageService;

        private SplitView _splitView;
        private Frame _frame;
        private object _lastParamUsed;

        public RightPaneService(IPageService pageService)
        {
            _pageService = pageService;
        }

        public void Initialize(Frame rightPaneFrame, SplitView splitView)
        {
            _splitView = splitView;
            _frame = rightPaneFrame;
            _frame.Navigated += OnNavigated;
        }

        public void OpenInRightPane(string pageKey, object parameter = null)
        {
            // Don't open the same page multiple times
            if (_frame.GetPageViewModel() == null || _frame.GetPageViewModel().GetType().FullName != pageKey || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var pageType = _pageService.GetPageType(pageKey);
                var vmBeforeNavigation = _frame.GetPageViewModel();
                var navigationResult = _frame.Navigate(pageType, parameter);
                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }
            }

            _splitView.OpenPaneLength = ((double?)(App.Current.MainWindow?.Bounds.Width * .33) >= 400 ? (double?)(App.Current.MainWindow?.Bounds.Width * .33) : 400) ?? 400;

            _splitView.IsPaneOpen = true;
        }

        public void CleanUp()
        {
            _frame.Navigated -= OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                frame.BackStack.Clear();
                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }
            }
        }
    }
}
