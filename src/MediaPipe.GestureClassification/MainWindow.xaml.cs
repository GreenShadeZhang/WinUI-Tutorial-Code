// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using FFmpeg.AutoGen;
using Mediapipe.Net.Examples.Hands;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Microsoft.UI.Xaml;
using SeeShark;
using SeeShark.Device;
using SeeShark.FFmpeg;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPipe.GestureClassification;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private static Camera? camera;
    private static FrameConverter? converter;
    private static HandsCpuSolution? calculator;
    private int frameCount = 0;
    public MainWindow()
    {
        this.InitializeComponent();
        calculator = new HandsCpuSolution();
    }
    private async void myButton_Click(object sender, RoutedEventArgs e)
    {
        // Get and parse command line arguments
        Options? parsed = new Options();
        if (parsed == null)
            return;

        (int, int)? videoSize = null;
        if (parsed.Width != null && parsed.Height != null)
            videoSize = ((int)parsed.Width, (int)parsed.Height);
        myButton.Content = "Clicked";
        FFmpegManager.SetupFFmpeg(@"C:\ffmpeg\v5.0_x64\", "/usr/lib");


        // Get a camera device
        using (CameraManager manager = new CameraManager())
        {
            try
            {
                camera = manager.GetDevice(0,
                    new VideoInputOptions
                    {
                        InputFormat = parsed.InputFormat,
                        Framerate = parsed.Framerate == null ? null : new AVRational
                        {
                            num = (int)parsed.Framerate,
                            den = 1,
                        },
                        VideoSize = videoSize,
                    });
                Console.WriteLine($"Using camera {camera.Info}");
            }
            catch (Exception ex)
            {
                return;
            }
        }

        calculator = new HandsCpuSolution();

        camera.OnFrame += OnFrameEventHandler;
        camera.StartCapture();
    }

    private async void OnFrameEventHandler(object? sender, SeeShark.FrameEventArgs e)
    {
        if (calculator == null)
            return;

        Frame frame = e.Frame;
        if (frame.Width == 0 || frame.Height == 0)
            return;

        converter ??= new FrameConverter(frame, PixelFormat.Rgba);
        Frame cFrame = converter.Convert(frame);

        ImageFrame imgframe = new ImageFrame(ImageFormat.Types.Format.Srgba,
            cFrame.Width, cFrame.Height, cFrame.WidthStep, cFrame.RawData);

        HandsOutput handsOutput = calculator.Compute(imgframe);

        if (handsOutput.MultiHandLandmarks != null)
        {
            var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;
            Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks"
                + $" at frame {frameCount}");

            //await HandDataFormatHelper.SaveDataToTextAsync(landmarks.ToList());

            var result = HandDataFormatHelper.PredictResult(landmarks.ToList());

            this.DispatcherQueue.TryEnqueue(() =>
            {
                HandResult.Text = result;
            });
        }
        else
        {
            Debug.WriteLine("No hand landmarks");
        }

        frameCount++;
    }
}
