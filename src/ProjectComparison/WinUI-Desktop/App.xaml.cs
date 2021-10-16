using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_Desktop
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            //m_window = new MainWindow(); //old

            m_window = new Window();

            EnsureWindow();

            //m_window.Activate();
        }

        private static Window m_window;

        public static Window CurrentWindow
        {
            get
            {
                return m_window;
            }
        }


        private void EnsureWindow()
        {

            Frame rootFrame = GetRootFrame();

            Type targetPageType = typeof(HomePage);

            string targetPageArguments = string.Empty;

            rootFrame.Navigate(targetPageType, targetPageArguments);

            //((Microsoft.UI.Xaml.Controls.NavigationViewItem)(((ShellRootPage)(App.CurrentWindow.Content)).NavigationView.MenuItems[0])).IsSelected = true;

            // Ensure the current window is active
            CurrentWindow.Activate();
        }

        private Frame GetRootFrame()
        {
            Frame rootFrame;

            ShellRootPage rootPage = CurrentWindow.Content as ShellRootPage;

            if (rootPage == null)
            {
                rootPage = new ShellRootPage();

                rootFrame = (Frame)rootPage.FindName("contentFrame");

                if (rootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }

                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                CurrentWindow.Content = rootPage;
            }
            else
            {
                rootFrame = (Frame)rootPage.FindName("contentFrame");
            }

            return rootFrame;
        }
    }
}
