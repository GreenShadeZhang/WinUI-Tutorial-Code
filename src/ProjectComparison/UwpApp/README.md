# UwpApp

This project uses Microsoft's modern .NET UWP project template. It remains a UWP app using `Windows.UI.Xaml`; it has not been converted to WinUI 3. See Microsoft's [UWP modernization guidance](https://learn.microsoft.com/en-us/windows/uwp/dotnet-native/modernize-uwp-apps-with-dotnet).

## Visual Studio setup

Visual Studio 2026 includes the modern .NET UWP project templates; a separate template or NuGet template pack is not required. In Visual Studio Installer, modify the installation and select **WinUI application development** under **Desktop & Mobile**. Under its optional components, install **Universal Windows Platform tools** and **Windows SDK 10.0.26100.0** or later. The UWP tools component ID is `Microsoft.VisualStudio.ComponentGroup.UWP.Support`.

After installation, reopen `WinUITutorialCode.slnx`, set `UwpApp` as the startup project if Visual Studio has a previous local startup selection, choose **Local Machine** and `Debug | x64`, then press F5. See Microsoft's [UWP deployment and debugging guide](https://learn.microsoft.com/en-us/windows/uwp/debug-test-perf/deploying-and-debugging-uwp-apps) and [Visual Studio workload/component ID list](https://learn.microsoft.com/en-us/visualstudio/install/workload-and-component-ids?view=visualstudio).

The project requires Visual Studio 2026 with the Universal Windows Platform tools and Windows SDK 10.0.26100.0. Build it with Visual Studio MSBuild, then use `winapp` to register and launch the packaged output:

The root `WinUITutorialCode.slnx` marks this project as the default startup project. Visual Studio keeps a manually selected startup project in local solution state, so use **Set as Startup Project** on `UwpApp` if a previous selection overrides that default.

```powershell
msbuild .\src\ProjectComparison\UwpApp\UwpApp.csproj /restore /t:Build /p:Configuration=Release /p:Platform=x64 /p:RuntimeIdentifier=win-x64
winapp run .\src\ProjectComparison\UwpApp\UwpApp.csproj --no-build --detach --arch x64 --configuration Release --property Platform=x64
```

Use Visual Studio MSBuild for the build because the UWP XAML compiler is supplied by the Visual Studio UWP tools; `dotnet build` does not load those UWP build targets.

If Visual Studio starts and immediately exits, check **Event Viewer > Windows Logs > Application**. A packaged UWP launch should have a package identity. If the `Faulting package full name` field is empty and the failing executable is the `bin\...\UwpApp.exe` output, Visual Studio launched the EXE directly instead of activating the package. Use the **Local Machine** UWP deployment target; for an already registered package, Visual Studio also provides **Debug > Other Debug Targets > Debug Installed App Package**.
