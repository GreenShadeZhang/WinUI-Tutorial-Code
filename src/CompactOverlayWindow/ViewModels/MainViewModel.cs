using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompactOverlayWindow.Views;
using Microsoft.UI.Windowing;

namespace CompactOverlayWindow.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly AppWindow _mainAppWindow;
    public MainViewModel()
    {
        _mainAppWindow = App.MainWindow.GetAppWindow();
    }

    [RelayCommand]
    private void OnCompactOverlay()
    {
        _mainAppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
    }
}
