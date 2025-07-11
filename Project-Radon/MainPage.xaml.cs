using Microsoft.UI.Xaml.Controls;
using Project_Radon.Controls;
using Project_Radon.Helpers;
using Project_Radon.Settings;
using Project_Radon.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Yttrium;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;



namespace Yttrium_browser
{
    public sealed partial class MainPage : Page
    {
        string OriginalUserAgent;
        string GoogleSignInUserAgent;
        public static string SearchValue;
        private readonly ObservableCollection<BrowserTabViewItem> CurrentTabs = new ObservableCollection<BrowserTabViewItem>();
        private readonly Debouncer _debouncer = new Debouncer();

        //these two are essentials for url share (Windows Share)
        private DataTransferManager dataTransferManager;
        private string urlToShare;
        public MainPage()
        {
            InitializeComponent();
            CurrentTabs.Add(new BrowserTabViewItem());
            CurrentTabs[0].Tab.PropertyChanged += SelectedTabPropertyChanged;

            Window.Current.CoreWindow.Activated += CoreWindow_Activated;

            // TitleBar customizations



            profileCheck();

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //load theme settings
            String colorthemevalue = localSettings.Values["appcolortheme"] as string;
            appthemebackground.Source = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/" + colorthemevalue + ".png" })));
            fullscreentopbarbackground.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/" + colorthemevalue + ".png" })));

            //load titlebar settings
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            String systemTitleBar = localSettings.Values["systemTitleBar"] as string;

            if (systemTitleBar == "True")
            {
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                localSettings.Values["systemTitleBar"] = "True";
            }

            else
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                coreTitleBar.ExtendViewIntoTitleBar = true;

                localSettings.Values["systemTitleBar"] = "False";
            }
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            // TODO: Add logics to initiate update announcements
            ShowUpdateAnnouncement();

            // TODO: Download prompt debug, remove after done debugging
            // new DownloadPrompt().ShowAsync();

        }

        private async void ShowUpdateAnnouncement()
        {
            SponsorDialog dialog = new SponsorDialog();
            await dialog.ShowAsync();
        }

        public class TabViewItemData
        {
            public string Header { get; set; }
            public string FaviconURI { get; set; }
            public string SourceURL { get; set; }
        }


        private void profileCheck()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // profile check mechanisms
            string username = localSettings.Values["username"] as string;
            if (username == null)
            {
                //this.Frame.Navigate(typeof(oobe1), null);
            }
        }
        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            await new ConfirmExitDialog().ShowAsync();
        }

        private void CoreWindow_Activated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                appthemebackground.Opacity = 0.7;
            }

            else
            {
                appthemebackground.Opacity = 1;
            }
        }
        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            backbutton_icon.Translation = new Vector3(-12, 0, 0);
            await Task.Delay(150);
            backbutton_icon.Translation = new Vector3(0, 0, 0);

            CurrentTabs[BrowserTabs.SelectedIndex].Tab.BackButtonSender();
            ElementSoundPlayer.Play(ElementSoundKind.MovePrevious);

        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            forwardbutton_icon.Translation = new Vector3(12, 0, 0);
            await Task.Delay(150);
            forwardbutton_icon.Translation = new Vector3(0, 0, 0);
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.FowardButtonSender();
            ElementSoundPlayer.Play(ElementSoundKind.MoveNext);
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Task1();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loadingbar.IsIndeterminate = true;
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Reload();
            ElementSoundPlayer.Play(ElementSoundKind.GoBack);

        }

        //TODO: On window size changed, if navbuttonbar.Width is less than 140, hide the standard nav buttons and show the overflow buttons.

        //navigation completed
        private async void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {

            if (CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri.Contains("https"))
            {
                //change icon to lock
                SSLIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLIcon.Glyph = "\xe705";

                //change icon to lock
                SSLFlyoutIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLFlyoutIcon.Glyph = "\xe705";
                SSLFlyoutHeader.Text = "Your connection is secured.";
                SSLFlyoutStatus.Text = "This site has a valid SSL certificate.";
                SSLFlyoutStatus2.Text = "Your data will be securely sent to the site and will not be intercepted or seen by others.";
            }

            else
            {
                //change icon to warning
                SSLIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLIcon.Glyph = "\xe783";

                //change icon to lock
                SSLFlyoutIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLFlyoutIcon.Glyph = "\xe783";
                SSLFlyoutHeader.Text = "This site seems dangerous";
                SSLFlyoutStatus.Text = "This site does not have a valid SSL certificate.";
                SSLFlyoutStatus2.Text = "You data may be at risk of being stolen or intercepted. Be careful on this website.";
            }

            if (SearchBar.FocusState == FocusState.Unfocused)
            {

            }

            if (SearchBar.Text.Equals("radon://radon-ntp/"))
            {
                SearchBar.Text = string.Empty;
            }


            //website load status
            try
            {
                Uri icoURI = new Uri("http://www.google.com/s2/favicons?domain=" + SearchValue);
                faviconicon.UriSource = icoURI;
                faviconicon.ShowAsMonochrome = false;


            }
            catch (Exception ExLoader)
            {
                ErrorDialog dialog = new ErrorDialog(ExLoader.ToString());
                await dialog.ShowAsync();
            }
            RefreshButton.Visibility = Visibility.Visible;
            StopRefreshButton.Visibility = Visibility.Collapsed;



        }

        private async void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(SearchBar.Text))
            {
                if (SearchBar.Text.Contains("://settings"))
                {
                    settings_click_helper();
                }
                else
                {
                    SearchBar.RemoveFocusEngagement();
                    if ((searchSuggestionList.Items.Count == 0) || (searchSuggestionList.SelectedIndex == 0))
                    {
                        await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(SearchBar.Text);
                    }
                    else { await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(searchSuggestionList.SelectedItem.ToString()); }
                }
            }

            if (e.Key == VirtualKey.Escape)
            {
                //TODO: Pressing ESC will set the SearchBar.Text to WebView2 source (ESC will cancel URL changes)
                SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;

                //TODO: WebView2 will steal the focus for keyboard and pointer
                SearchBar.RemoveFocusEngagement();
                BrowserTabs.Focus(FocusState.Keyboard);
            }

            if (e.Key == VirtualKey.Up && searchSuggestionList.SelectedIndex != 0) { searchSuggestionList.SelectedIndex -= 1; }
            if (e.Key == VirtualKey.Down && searchSuggestionList.SelectedIndex != searchSuggestionList.Items.Count - 1) { searchSuggestionList.SelectedIndex += 1; }

            if (e.Key != VirtualKey.Up && e.Key != VirtualKey.Down)
            {
                if (searchSuggestionsFlyout.Visibility == Visibility.Collapsed)
                {
                    searchSuggestionsFlyout.Visibility = Visibility.Visible;
                    searchSuggestionList.ItemsSource = null;
                }
                searchSuggestionLoadIndicator.Visibility = Visibility.Visible;
                _debouncer.Debounce(async () =>
                {
                    var query = SearchBar.Text;
                    var suggestions = await SuggestionService.GetSuggestionsAsync(query);
                    if (SearchBar.Text != string.Empty) { suggestions.Insert(0, SearchBar.Text); }

                    suggestions = suggestions.Distinct().ToList();
                    searchSuggestionList.ItemsSource = suggestions;
                    searchSuggestionList.SelectedIndex = 0;
                    searchSuggestionLoadIndicator.Visibility = Visibility.Collapsed;
                });
            }

            // ======This line is used to quickly renavigate to MainPage========

            // this.Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());

        }


        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            //TODO: Experiment with autosuggestvox select all experience
            //SearchBar.SelectAll();
            RefreshButton.Visibility = Visibility.Collapsed;
            SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshButton.Visibility = Visibility.Visible;
            if (searchSuggestionList.FocusState != FocusState.Unfocused) { searchSuggestionsFlyout.Visibility = Visibility.Visible; }
            else { searchSuggestionsFlyout.Visibility = Visibility.Collapsed;   }

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBar.Text = string.Empty;
        }


        //handles progressring and refresh behavior
        private void WebBrowser_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            RefreshButton.Visibility = Visibility.Collapsed;
            StopRefreshButton.Visibility = Visibility.Visible;
            RefreshButton.IsEnabled = true;
            loadingbar.IsIndeterminate = true;
            BrowserTabs.Focus(FocusState.Keyboard);
        }

        //stops refreshing if clicked on progressbar
        private async void StopRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Stop();
            loadingbar.ShowError = true;
            await Task.Delay(2000);
            loadingbar.ShowError = false;
            loadingbar.IsIndeterminate = true;
        }

        //titlebar
        private void DragArea_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(DragArea);
        }

        #region Tabs
        private void SelectedTabPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BrowserTabs.SelectedIndex >= 0)
            {
                SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;
                if (SearchBar.Text.Contains("://settings"))
                    SearchBar.IsEnabled = false;
                loadingbar.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                StopRefreshButton.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                RefreshButton.Visibility = !CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                BackButton.IsEnabled = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoBack ? IsEnabled : false;
                ForwardButton.IsEnabled = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoFoward ? IsEnabled : false;
            }
        }
        private async void NewTabRequested(object s, string e)
        {
            var b = new BrowserTabViewItem();
            CurrentTabs.Add(b);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
            _ = b.Tab.GoTo(e);


        }
        private void BrowserTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var r = e.RemovedItems.Select(x => (BrowserTabViewItem)x).ToList();
            r.ForEach(x =>
            {
                x.Tab.PropertyChanged -= SelectedTabPropertyChanged;
                x.Tab.NewTabRequested -= NewTabRequested;
            });
            var s = e.AddedItems.Select(x => (BrowserTabViewItem)x).ToList();
            s.ForEach(x =>
            {
                x.Tab.PropertyChanged += SelectedTabPropertyChanged;
                x.Tab.NewTabRequested += NewTabRequested;
            });
            SelectedTabPropertyChanged(null, null);
            SearchBarStateHandler();
        }

        private void SearchBarStateHandler()
        {
            // TODO: Inspect searchbar state handler
            //if (CurrentTabs[BrowserTabs.SelectedIndex + 1].Content.ToString() != "Project_Radon.Controls.BrowserTab")
            //{
            //    SearchBar.IsEnabled = false;
            //    RefreshButton.IsEnabled = false;
            //    SSLButton.IsEnabled = false;
            //}
            //else
            //{ 
            //    SearchBar.IsEnabled = true;
            //    RefreshButton.IsEnabled = true;
            //    SSLButton.IsEnabled = true;
            //}
        }

        private async void BrowserTabs_AddTabButtonClick(TabView sender, object args)
        {
            CurrentTabs.Add(new BrowserTabViewItem());
            await Task.Delay(25);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count <= 1)
                BrowserTabs_AddTabButtonClick(sender, args);

            sender.TabItems.Remove(args.Tab);
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
        }
        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
        }
        private void BrowserTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (CurrentTabs.Count == 1)
                Application.Current.Exit();

            (args.Item as BrowserTabViewItem).Tab.PropertyChanged -= SelectedTabPropertyChanged;
            (args.Item as BrowserTabViewItem).Tab.Close();
            CurrentTabs.Remove(args.Item as BrowserTabViewItem);
        }
        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            BrowserTabs_AddTabButtonClick(null, null);
            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (CurrentTabs.Count == 1)
                Application.Current.Exit();

            CurrentTabs[BrowserTabs.SelectedIndex].Tab.PropertyChanged -= SelectedTabPropertyChanged;
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Close();
            CurrentTabs.RemoveAt(BrowserTabs.SelectedIndex);
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = CurrentTabs.Count - 1;
                    break;
                case VirtualKey.LeftButton:
                    tabToSelect = CurrentTabs.Count == 1 ? 1 : BrowserTabs.SelectedIndex - 1;
                    break;
                case VirtualKey.RightButton:
                    tabToSelect = BrowserTabs.SelectedIndex + 1 == CurrentTabs.Count ? BrowserTabs.SelectedIndex : BrowserTabs.SelectedIndex + 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < CurrentTabs.Count)
            {
                BrowserTabs.SelectedIndex = tabToSelect;
            }

            args.Handled = true;
        }
        #endregion

        #region Flyout Handlers
        //opens about app dialog
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            ContentDialog aboutdialog = new AboutDialog();
            var result = await aboutdialog.ShowAsync();
        }
        private void printbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            _ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.PrintTask();

        }
        private async void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            controlCenterButton.Flyout.Hide();
            //await new Downloads_Dialog().ShowAsync();

            //if (BrowserTabs.SelectedIndex >= 0)
            //_ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.OpenDownloadsDialog();
            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "30868ItzBluebxrry.RadonBrowserDev_qc4twqjhevfwm!App");

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(downloadsPath);
            Debug.WriteLine(folder);
            await Launcher.LaunchFolderAsync(folder);
            if (Directory.Exists(downloadsPath))
            {
            }

        }

        private async void editprofile_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            controlCenterButton.Flyout.Hide();
            await new UserProfileDialog().ShowAsync();
        }

        private void fullscreenbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.TryEnterFullScreenMode();
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                fullscreenbutton_icon.Glyph = "\uE740";
                BrowserTabs.Margin = new Windows.UI.Xaml.Thickness(0, 0, 0, 0);
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(40);
                fullscreentopbar.Visibility = Visibility.Collapsed;
            }
            else
            {
                view.TryEnterFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                fullscreenbutton_icon.Glyph = "\uE73F";
                BrowserTabs.Margin = new Windows.UI.Xaml.Thickness(0, -40, 0, 0);
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(0);
                fullscreentopbar.Visibility = Visibility.Visible;
            }
        }

        private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            settings_click_helper();
        }

        private async void settings_click_helper()
        {
            var t = new BrowserTabViewItem()
            {
                CustomContentType = typeof(RadonSettings),
                ShowCustomContent = true,
                CustomHeader = "Radon Settings",
                CustomIcon = new SymbolIconSource() { Symbol = Symbol.Setting }
            };
            t.Tab.Close();
            CurrentTabs.Add(t);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;

        }
        #endregion

        private void BaseGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int width = (int)SearchBar.ActualWidth;
            searchSuggestionList.Width = width;


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void addtab_button_Click(object sender, RoutedEventArgs e)
        {
            var b = new BrowserTabViewItem();
            CurrentTabs.Add(b);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
            //addtabtip.IsOpen = true;
            RadonOverflowMenu.Hide();

        }

        private async void tabactions_devtools_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentTabs[BrowserTabs.SelectedIndex].ShowCustomContent)
                _ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.OpenDevTools();
            actionMsg_Icon.Glyph = "\uE930";
            actionMsg_Text.Text = "DevTools launched in background!";
            actionMsg.IsOpen = true;
            actionMsgTimeoutHandler();
        }
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

        }

        private void tabaction_titlebar_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            if (coreTitleBar.ExtendViewIntoTitleBar == true)
            {
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                titleBar.ButtonBackgroundColor = null;
                titleBar.ButtonInactiveBackgroundColor = null;
                titleBar.BackgroundColor = null;

                localSettings.Values["systemTitleBar"] = "True";
            }

            else
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                coreTitleBar.ExtendViewIntoTitleBar = true;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = null;

                localSettings.Values["systemTitleBar"] = "False";
            }
        }

        private void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            ThemePopup.IsOpen = true;

        }

        private void ThemePickerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["appcolortheme"] = (ThemePickerComboBox.SelectedItem as ComboBoxItem).Content.ToString();


            appthemebackground.Source = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/", (ThemePickerComboBox.SelectedItem as ComboBoxItem).Content.ToString(), ".png" })));

        }

        private void ThemePopupDoneButton_Click(object sender, RoutedEventArgs e)
        {
            ThemePopup.IsOpen = false;
        }

        private void RefreshButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private void controlCenterToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void controlCenter_Closed(object sender, object e)
        {
        }

        private void controlCenter_sound_Click(object sender, RoutedEventArgs e)
        {

        }

        private void controlcenter_sidebar_Click(object sender, RoutedEventArgs e)
        {

        }

        // Forgot to name the ToggleButton, btw it's the toggle for profileCenter
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            profileCenter.IsOpen = true;
        }

        private void profileCenter_Closed(object sender, object e)
        {

        }

        private void profileCenter_Opened(object sender, object e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue = localSettings.Values["username"] as string;

            if (localValue != null)
            {
                profileCenter_UsernameHeader.Text = localValue;
            }
            else {
                // this.Frame.Navigate(typeof(oobe1), null, new EntranceNavigationTransitionInfo());
            }
        }

        private void fullscreentopbar_Click(object sender, RoutedEventArgs e)
        {
            fullscreentopbar_flyout.IsOpen = true;
        }

        private async void newWindow_Click(object sender, RoutedEventArgs e)
        {
            // TODO: This is template to open new window lmao
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(MainPage), null);
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void verttablist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void verttablist_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            
        }

        

        private void updatedDialog_Closed(TeachingTip sender, TeachingTipClosedEventArgs args)
        {
            
        }

        private void copyurltoclipboardBtn_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri.ToString());
            Clipboard.SetContent(dataPackage);
            controlCenterButton.Flyout.Hide();
            actionMsg_Icon.Glyph = "\xe71b";
            actionMsg_Text.Text = "Link copied to clipboard!";
            actionMsg.IsOpen = true;
            actionMsgTimeoutHandler();
        }

        private async void continueonmobileBtn_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["qrUrl"] = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri.ToString();
            localSettings.Values["qrFavicon"] = CurrentTabs[BrowserTabs.SelectedIndex].Tab.Favicon.ToString();
            qrcodedialog dialog = new qrcodedialog();
            dialog.TitleText = CurrentTabs[BrowserTabs.SelectedIndex].Tab.Title.ToString();
            dialog.UrlText = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri.ToString();
            controlCenterButton.Flyout.Hide();
            await dialog.ShowAsync();
        }

        private async void actionMsgTimeoutHandler()
        {
            actionMsg_Timeout.Value = 300;
            while (actionMsg_Timeout.Value > 0)
            {
                actionMsg_Timeout.Value -= 2;
                await Task.Delay(20);
            }
            actionMsg.IsOpen = false;
        }

        private void whatsnew_click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            
        }

        private async void searchSuggestionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SearchBar.RemoveFocusEngagement();
            await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(searchSuggestionList.SelectedItem.ToString());
        }

        private void moresharingBtn_Click(object sender, RoutedEventArgs e)
        {
            urlToShare = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri; ; // <- your URL string
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetWebLink(new Uri(urlToShare)); // <- Share the URL as a weblink
            request.Data.Properties.Title = "Share this link"; // title of the share UI
            request.Data.Properties.Description = "Sharing a link from my app!"; // optional
        }

        private void profileToolbarPicture_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string pfpsrc = localSettings.Values["profilePicture"] as string;
            profileToolbarPicture.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc.ToString(), ".png" })));
        }

        private void profileToolbarFlyoutPicture_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string pfpsrc = localSettings.Values["profilePicture"] as string;
            profileToolbarFlyoutPicture.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", pfpsrc.ToString(), ".png" })));
            profileToolbarFlyoutUsername.Text = ApplicationData.Current.LocalSettings.Values["username"].ToString();
        }
    }
}