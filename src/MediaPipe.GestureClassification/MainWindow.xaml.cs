// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CommunityToolkit.WinUI.Helpers;
using Mediapipe.Net.Examples.Hands;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Interop;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPipe.GestureClassification;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private static WinuiHandsCpuSolution? calculator;

    private int frameCount = 0;

    readonly CameraHelper cameraHelper = new();

    private readonly string modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";
    public MainWindow()
    {
        this.InitializeComponent();

        Closed += MainWindow_Closed;
    }

    private async void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        if (cameraHelper != null)
        {
            cameraHelper.FrameArrived -= CameraHelper_FrameArrived;
            await cameraHelper.CleanUpAsync();
        }

    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var destinationFolder = await KnownFolders.PicturesLibrary
        .CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);


        calculator = new WinuiHandsCpuSolution();

        //CameraHelper cameraHelper = new CameraHelper();
        var result = await cameraHelper.InitializeAndStartCaptureAsync();

        if (result == CameraHelperResult.Success)
        {
            // Subscribe to get frames as they arrive
            cameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }
        else
        {
            // Get error information
            var errorMessage = result.ToString();
        }
    }

    private async void CameraHelper_FrameArrived(object sender, CommunityToolkit.WinUI.Helpers.FrameEventArgs e)
    {
        try
        {
            // Gets the current video frame
            VideoFrame currentVideoFrame = e.VideoFrame;

            // Gets the software bitmap image
            SoftwareBitmap softwareBitmap = currentVideoFrame.SoftwareBitmap;

            if (softwareBitmap != null)
            {
                //if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                // softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                //{
                //    softwareBitmap = SoftwareBitmap.Convert(
                //        softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                //}

                //using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                //var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                //// Set the software bitmap
                //encoder.SetSoftwareBitmap(softwareBitmap);

                //await encoder.FlushAsync();

                //var image = new Bitmap(stream.AsStream());

                //var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\hand.png");

                var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                var dataMeta = mat2.Data;

                var length = mat2.Width * mat2.Height * mat2.Channels();

                var data = new byte[length];

                Marshal.Copy(dataMeta, data, 0, length);

                var widthStep = (int)mat2.Step();

                var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                var handsOutput = calculator.Compute(imgframe);

                if (handsOutput.MultiHandLandmarks != null)
                {
                    var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

                    Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks" + $" at frame {frameCount}");

                    var result = HandDataFormatHelper.PredictResult(landmarks.ToList(), modelPath);


                    this.DispatcherQueue.TryEnqueue(async() =>
                    {
                        HandResult.Text = result;
                    });
                }
                else
                {
                    Debug.WriteLine("No hand landmarks");
                }
            }
        }
        catch (Exception ex)
        {

        }
        frameCount++;
    }
}
