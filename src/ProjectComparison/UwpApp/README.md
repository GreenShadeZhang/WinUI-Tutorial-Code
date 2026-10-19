# UwpApp

This project uses Microsoft's modern .NET UWP project template. It remains a UWP app using `Windows.UI.Xaml`; it has not been converted to WinUI 3. See Microsoft's [UWP modernization guidance](https://learn.microsoft.com/en-us/windows/uwp/dotnet-native/modernize-uwp-apps-with-dotnet).

The project requires Visual Studio 2026 with the Universal Windows Platform tools and Windows SDK 10.0.26100.0. Build it with Visual Studio MSBuild, then use `winapp` to register and launch the packaged output:

```powershell
msbuild .\src\ProjectComparison\UwpApp\UwpApp.csproj /restore /t:Build /p:Configuration=Release /p:Platform=x64 /p:RuntimeIdentifier=win-x64
winapp run .\src\ProjectComparison\UwpApp\UwpApp.csproj --no-build --detach --arch x64 --configuration Release --property Platform=x64
```

Use Visual Studio MSBuild for the build because the UWP XAML compiler is supplied by the Visual Studio UWP tools; `dotnet build` does not load those UWP build targets.
