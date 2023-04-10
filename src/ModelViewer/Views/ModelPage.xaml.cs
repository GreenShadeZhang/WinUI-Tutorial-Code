﻿using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

using ModelViewer.ViewModels;

namespace ModelViewer.Views;

public sealed partial class ModelPage : Page
{
    public ModelViewModel ViewModel
    {
        get;
    }
    public ModelPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.GetService<ModelViewModel>();
        viewport.OnMouse3DDown += Viewport_OnMouse3DDown;
    }

    private void Viewport_OnMouse3DDown(object sender, MouseDown3DEventArgs e)
    {
        if (e.HitTestResult == null)
        {
            return;
        }
        if (e.HitTestResult.ModelHit is SceneNode node && node.Tag is AttachedNodeViewModel vm)
        {
            vm.Selected = !vm.Selected;
        }
    }
}
