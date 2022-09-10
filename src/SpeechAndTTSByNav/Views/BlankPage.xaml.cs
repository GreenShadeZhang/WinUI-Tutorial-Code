using Microsoft.UI.Xaml.Controls;

using SpeechAndTTSByNav.ViewModels;

namespace SpeechAndTTSByNav.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = App.GetService<BlankViewModel>();
        InitializeComponent();
    }
}
