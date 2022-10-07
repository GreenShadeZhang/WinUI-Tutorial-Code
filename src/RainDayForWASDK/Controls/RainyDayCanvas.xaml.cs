using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using System.IO;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RainDayForWASDK.Controls;

public sealed partial class RainyDayCanvas:UserControl
{

    const float defaultDpi = 96;
    CanvasRenderTarget glassSurface;
    CanvasBitmap imgbackground;
    GaussianBlurEffect blurEffect;
    RainyDay rainday;
    float scalefactor;
    float imgW;
    float imgH;
    float imgX;
    float imgY;



    public RainyDayCanvas()
    {
        InitializeComponent();
    }
    private async void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        await PrepareRaindayAsync(sender);
    }

    private async Task PrepareRaindayAsync(CanvasControl sender, string demo = "demo1", bool isFullScreen = false)
    {
        var imgPath = Path.Combine(AppContext.BaseDirectory, $"Assets/Images/{demo}.jpg");

        imgbackground = await CanvasBitmap.LoadAsync(sender, imgPath);

        blurEffect = new GaussianBlurEffect()
        {
            Source = imgbackground,
            BlurAmount = 4.0f,
            BorderMode = EffectBorderMode.Soft
        };
        scalefactor = isFullScreen ? (float)Math.Max(sender.Size.Width / imgbackground.Size.Width, sender.Size.Height / imgbackground.Size.Height) : (float)Math.Min(sender.Size.Width / imgbackground.Size.Width, sender.Size.Height / imgbackground.Size.Height);
        imgW = (float)imgbackground.Size.Width * scalefactor;
        imgH = (float)imgbackground.Size.Height * scalefactor;
        imgX = (float)(sender.Size.Width - imgW) / 2;
        imgY = (float)(sender.Size.Height - imgH) / 2;
        glassSurface = new CanvasRenderTarget(sender, imgW, imgH, defaultDpi);

        List<List<float>> pesets;


        if (demo == "demo1")
        {
            rainday = new RainyDay(sender, imgW, imgH, imgbackground)
            {
                ImgSclaeFactor = scalefactor,
                GravityAngle = (float)Math.PI / 2
            };
            pesets = new List<List<float>>() {

            new List<float> { 3, 3, 0.88f },
            new List<float> { 5, 5, 0.9f },
            new List<float> { 6, 2, 1 }
            };
        }
        else if (demo == "demo2")
        {
            rainday = new RainyDay(sender, imgW, imgH, imgbackground)
            {
                ImgSclaeFactor = scalefactor,
                GravityAngle = (float)Math.PI / 9
            };
            pesets = new List<List<float>>()
            {
                new List<float> { 1, 0, 1000 },
                new List<float> { 3, 3, 1 },
            };
        }
        else if (demo == "demo3")
        {
            rainday = new RainyDay(sender, imgW, imgH, imgbackground)
            {
                ImgSclaeFactor = scalefactor,
                CurrentGravity = RainyDay.GravityType.Gravity_None_Linear,
                GravityAngle = (float)Math.PI / 2
            };
            pesets = new List<List<float>>() {
            new List<float> {0, 2, 200},
            new List<float> { 3, 3, 1 }

        };

        }
        else
        {
            rainday = new RainyDay(sender, imgW, imgH, imgbackground)
            {
                ImgSclaeFactor = scalefactor,
                GravityAngle = (float)Math.PI / 2,
                CurrentGravity = RainyDay.GravityType.Gravity_None_Linear,
                CurrentTrail = RainyDay.TrailType.Trail_Smudge
            };
            pesets = new List<List<float>>() {
            new List<float> { 3, 3, 0.1f }
        };
        }
        rainday.Rain(pesets, 100);
    }

    private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (imgbackground != null)
        {
            args.DrawingSession.DrawImage(blurEffect, new Rect(imgX, imgY, imgW, imgH), new Rect(0, 0, imgbackground.Size.Width, imgbackground.Size.Height));
            args.DrawingSession.DrawImage(glassSurface, imgX, imgY);

            using var ds = glassSurface.CreateDrawingSession();
            rainday.UpdateDrops(ds);

        }
        canvas.Invalidate();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        canvas.RemoveFromVisualTree();
        canvas = null;
    }



    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        InitDemoData();
    }

    void InitDemoData()
    {
        var demos = new List<string>()
        {
            "demo1","demo2","demo3","demo4"
        };
        demosCB.SelectionChanged += DemosCB_SelectionChanged;
        demosCB.ItemsSource = demos;
        demosCB.SelectedIndex = 0;
        this.DataContext = this;

    }
    private async void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        canvas.Width = bg.ActualWidth;
        canvas.Height = bg.ActualHeight;
        string demo;
        bool isFullScreen = (bool)btnFullScreen.IsChecked;
        if (demosCB.SelectedValue == null)
        {
            demo = "demo1";
        }
        else
        {
            demo = demosCB.SelectedValue.ToString();
        }

        if (glassSurface != null && imgbackground != null)
        {
            await PrepareRaindayAsync(canvas, demo, isFullScreen);
            canvas.Invalidate();
        }
    }
    private async void DemosCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var s = sender as ComboBox;
        var demo = s.SelectedValue.ToString();
        var w = canvas.ActualWidth;
        if (glassSurface != null && imgbackground != null)
        {
            await PrepareRaindayAsync(canvas, demo);
            canvas.Invalidate();
        }

    }

    private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        btnFullScreen.IsChecked = false;
    }

    private void btnFullScreen_IsChecked(object sender, RoutedEventArgs e)
    {
        //if (btnFullScreen.IsChecked == true)
        //{
        //    ApplicationView.TryUnsnapToFullscreen();

        //    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        //    toolSP.Visibility = Visibility.Collapsed;
        //}
        //else
        //{
        //    ApplicationView.GetForCurrentView().ExitFullScreenMode();
        //    toolSP.Visibility = Visibility.Visible;
        //}
    }
}
