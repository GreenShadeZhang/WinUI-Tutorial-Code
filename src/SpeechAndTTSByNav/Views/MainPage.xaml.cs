using Microsoft.UI.Xaml.Controls;

using SpeechAndTTSByNav.ViewModels;

namespace SpeechAndTTSByNav.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }
}
