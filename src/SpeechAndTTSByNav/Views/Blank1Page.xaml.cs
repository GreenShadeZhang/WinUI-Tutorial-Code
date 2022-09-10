using Microsoft.UI.Xaml.Controls;

using SpeechAndTTSByNav.ViewModels;

namespace SpeechAndTTSByNav.Views;

public sealed partial class Blank1Page : Page
{
    public Blank1ViewModel ViewModel
    {
        get;
    }

    public Blank1Page()
    {
        ViewModel = App.GetService<Blank1ViewModel>();
        InitializeComponent();
    }
}
