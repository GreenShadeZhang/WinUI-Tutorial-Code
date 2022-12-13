// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CompactOverlayWindow.Contracts.Services;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompactOverlayWindow.Controls;
public sealed partial class CompactOverlayControl : UserControl
{
    private readonly AppWindow _mainAppWindow;
    public CompactOverlayControl()
    {
        this.InitializeComponent();
        _mainAppWindow = App.MainWindow.GetAppWindow();
    }
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _mainAppWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
    }
}
