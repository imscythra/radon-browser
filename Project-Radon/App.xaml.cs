using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Project_Radon;
using Project_Radon.Settings;
using Project_Radon.Views;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Foundation.Collections;
using Project_Radon.Controls;
using Windows.UI.WindowManagement;
using Windows.UI.Popups;
using Project_Radon.Helpers;
using Windows.UI.Core;
using CommunityToolkit.Mvvm.Messaging;
using Project_Radon.Contracts.Services;
using Project_Radon.ViewModels;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Project_Radon.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

// TODO: Import Cubekit.UI (Firecube's GlowUI refer https://github.com/FireCubeStudios/TemplateApp)

namespace Yttrium_browser
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    

    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>

        public static new App Current => (App)Application.Current;
        public CoreWindow? MainWindow => CoreWindow.GetForCurrentThread();
        public IServiceProvider Services { get; set; }

        public static T GetService<T>() where T : class
        {
            return App.Current is null || App.Current.Services is null
                ? throw new NullReferenceException("Application or Services are not properly initialized.")
                : App.Current.Services.GetService(typeof(T)) is not T service
                ? throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.")
                : service;
        }

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            object value = ApplicationData.Current.LocalSettings.Values["themeSetting"];

            if (value != null)
            {
                // Apply theme choice.
                App.Current.RequestedTheme = (ApplicationTheme)(int)value;
            }

            var pathToUDF = ApplicationData.Current.LocalFolder.Path;

            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", pathToUDF + @"\RadonBrowser\");

            SetupCoreFolders().ConfigureAwait(false);

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            Services = App.Current.ConfigureServices();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    rootFrame.Navigate(typeof(oobeHost), null);

                    // profile check mechanisms
                    //string username = localSettings.Values["username"] as string;
                    //string campaignRan = localSettings.Values["campaignRan"] as string;
                    //if (username == null)
                    //{
                    //    rootFrame.Navigate(typeof(oobe1), null);
                    //}
                    //else { rootFrame.Navigate(typeof(oobe1), e.Arguments); }
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter


                }
                // Ensure the current window is active
                Window.Current.Activate();

                //titlebar code
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                //coreTitleBar.ExtendViewIntoTitleBar = true;
                

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (coreTitleBar.ExtendViewIntoTitleBar == true)
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.BackgroundColor = Colors.Transparent;
                }

                else
                {
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                    titleBar.BackgroundColor = Colors.Transparent;
                }

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = Colors.Transparent;


                
                // set min window size
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                ApplicationView.GetForCurrentView().TryResizeView(new Size(1080,720)) ;
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(630,400));

                

                
            }
            
            
        }


        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            // Handle notification activation
            if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastActivationArgs.Argument);

                // Obtain any user input (text boxes, menu selections) from the notification
                ValueSet userInput = toastActivationArgs.UserInput;

                // TODO: Show the corresponding content
                
            }
        }

        static int tryCount = 0;
        private Task SetupCoreFolders()
        {


            if (tryCount > 2)
                return Task.FromException(new Exception("Radon Browser core folders can't be create in you documents"));

            tryCount++;

            var path = Environment.GetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER");

            if (path != null)
            {
                if (Directory.Exists(path))
                {

                    if (!Directory.Exists(Path.Combine(path, "Favorites")))
                    {
                        _ = Directory.CreateDirectory(Path.Combine(path, "Favorites"));

                        if (!File.Exists(Path.Combine(path, "Favorites\\favorites.json")))
                        {
                            using (var fs = File.Create(Path.Combine(path, "Favorites\\favorites.json"))) { };
                        }
                    }

                    if (!Directory.Exists(Path.Combine(path, "History")))
                    {
                        _ = Directory.CreateDirectory(Path.Combine(path, "History"));

                        if (!File.Exists(Path.Combine(path, "History\\history.json")))
                        {
                            using (var fs = File.Create(Path.Combine(path, "History\\history.json"))) { };
                        }
                    }

                    if (!Directory.Exists(Path.Combine(path, "Settings")))
                    {
                        _ = Directory.CreateDirectory(Path.Combine(path, "Settings"));

                        if (!File.Exists(Path.Combine(path, "Settings\\settings.json")))
                        {

                            var AppSettings = new RadonAppSettings() { DefaultSearchProvider = 0, HomeUrl = new Uri("https://google.com"), IsLocationOnOff = false, SideKick = false };

                            if (File.Exists(Path.Combine(path, "Settings", "settings.json")))
                            {
                                FileInfo fileInfo = new FileInfo(Path.Combine(path, "settings", "settings.json"));
                                if (fileInfo.Length <= 0)
                                {

                                    File.WriteAllText(Path.Combine(path, "settings", "settings.json"), JsonConvert.SerializeObject(AppSettings));
                                }
                            }
                            else
                            {
                                using (var file = File.Create(Path.Combine(path, "Settings", "settings.json")))
                                {

                                    byte[] data = new System.Text.UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(AppSettings));
                                    file.Write(data, 0, data.Length);

                                }

                            }


                        }
                    }

                    if (!Directory.Exists(Path.Combine(path, "WebView")))
                    {

                        _ = Directory.CreateDirectory(Path.Combine(path, "WebView"));

                        if (!File.Exists(Path.Combine(path, "WebView\\view.png")))
                            using (var fs = File.Create(Path.Combine(path, "WebView\\view.png"))) { };

                    }

                }
                else
                {
                    _ = Directory.CreateDirectory(path);
                    SetupCoreFolders().ConfigureAwait(false);
                }

            }
            return Task.CompletedTask;
        }
        public static string Get_Appx_AssemblyDirectory(Assembly assembly)
        {
            string assemblyLocation = assembly.Location;
            string directoryPath = Path.GetDirectoryName(assemblyLocation);

            return directoryPath ?? throw new DirectoryNotFoundException("Publish directory not found");

        }
        public static string GetFullPathToExe()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            int pos = path.LastIndexOf("\\");
            return path.Substring(0, pos);
        }

        public static string GetFullPathToAsset(string assetName)
        {
            return GetFullPathToExe() + "\\Assets\\" + assetName;
        }
        private System.IServiceProvider ConfigureServices()
        {
            // TODO WTS: Register your services, viewmodels and pages here
            var services = new ServiceCollection();
            // Default Activation Handler
            _ = services.AddSingleton<ISettingsService, SettingsService>();
            _ = services.AddSingleton<IWebViewService, WebViewService>();
            _ = services.AddTransient<MainPageViewModel>();
            // Core Services
            _ = services.AddSingleton<WeakReferenceMessenger>();
            _ = services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
                provider.GetRequiredService<WeakReferenceMessenger>());

            return services.BuildServiceProvider();
        }
    }
}
