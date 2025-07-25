using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using WinUI.UseLiteDB.Interfaces;
using WinUI.UseLiteDB.Models;
using WinUI.UseLiteDB.Repository;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI.UseLiteDB
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IPersonalInfoRepository Repository { get; private set; }
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
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var assetsDbDataPath = Package.Current.InstalledLocation.Path + @"\Assets\data-litedb.db";

            var dbDataPath = ApplicationData.Current.LocalFolder.Path + @"\data-litedb.db";

            var dbData = Package.Current.InstalledLocation.Path + @"\Assets\db-data.json";           

            File.Copy(assetsDbDataPath, dbDataPath, true);

            Repository = new PersonalInfoRepository(dbDataPath);

            var dataModel = System.Text.Json.JsonSerializer.Deserialize<PersonalInfoModel>(File.ReadAllText(dbData));

            await Repository.BatchAddAsync(dataModel.Data);

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
