using CompactOverlayWindow.Activation;
using CompactOverlayWindow.Contracts.Services;
using CompactOverlayWindow.Core.Contracts.Services;
using CompactOverlayWindow.Core.Services;
using CompactOverlayWindow.Helpers;
using CompactOverlayWindow.Services;
using CompactOverlayWindow.ViewModels;
using CompactOverlayWindow.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CompactOverlayWindow;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();

            // Configuration
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var appWindow = MainWindow.GetAppWindow();

        appWindow.Changed += AppWindow_Changed;

        await App.GetService<IActivationService>().ActivateAsync(args);
    }

    private void AppWindow_Changed(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            switch (MainWindow.GetAppWindow().Presenter.Kind)
            {
                case AppWindowPresenterKind.CompactOverlay:
                    //MainWindow.GetAppWindow().MoveAndResize(new Windows.Graphics.RectInt32
                    //{
                    //    X = 0,
                    //    Y = 0,
                    //    Width = 240,
                    //    Height = 240
                    //});

                    App.MainWindow.Content = new BlankPage();

                    break;

                case AppWindowPresenterKind.Overlapped:
                    var frame = new Frame();
                    var nav = App.GetService<INavigationService>();
                    nav.Frame = frame;
                    //App.MainWindow.Content = null;
                    if (App.MainWindow.Content == null)
                    {
                        App.MainWindow.Content = frame;
                    }
                    nav.NavigateTo(typeof(MainViewModel).FullName!,null,true);
                    break;

                default:
                    // If we end up here the presenter was changed to something we don't know what it is.
                    // This would happen if a new presenter is introduced.
                    // We can ignore this situation since we are not aware of the presenter and have no UI that
                    // reacts to this unknown presenter.
                    break;
            }
        }
    }
}
