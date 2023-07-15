using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Mediapipe.Net.Solutions;
using Mediapipe.Net.Framework.Format;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Mediapipe.Net.Framework.Protobuf;
using OpenCvSharp.Extensions;
using System.Threading.Tasks;
using MediaPipe.PoseDetection.Extensions;
using Microsoft.Graphics.Canvas;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Graphics.Canvas.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPipe.PoseDetection;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private static PoseCpuSolution calculator =
        new(modelComplexity: 2, smoothLandmarks: false);

    CanvasBitmap _image;

    private PoseOutput _poseOutput;

    private ModelViewModel ViewModel;

    public MainWindow()
    {
        this.InitializeComponent();
        ViewModel = new ModelViewModel();
    }


    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\pose.jpg");

        var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

        var dataMeta = mat2.Data;

        var length = mat2.Width * mat2.Height * mat2.Channels();

        var data = new byte[length];

        Marshal.Copy(dataMeta, data, 0, length);

        var widthStep = (int)mat2.Step();

        var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

        var handsOutput = calculator.Compute(imgframe);

        if (handsOutput.PoseLandmarks != null)
        {
            _poseOutput = handsOutput;
            ViewModel.InitAsync(_poseOutput, (float)_image.Size.Width, (float)_image.Size.Height, 200);
            CanvasControl1.Invalidate();
            var landmarks = handsOutput.PoseLandmarks.Landmark;
            Console.WriteLine($"Got pose output with {landmarks.Count} landmarks");
        }
        else
        {
            Console.WriteLine("No pose landmarks");
        }
    }

    public async Task<SoftwareBitmap> BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
    {
        var ms = new MemoryStream();

        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        ms.Seek(0, SeekOrigin.Begin);

        // Create the decoder from the stream
        var decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());

        // Get the SoftwareBitmap representation of the file
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        return softwareBitmap;
    }

    private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (_image != null)
        {
            // Draw the image
            args.DrawingSession.DrawImage(_image);

        }

        if (_poseOutput != null)
        {


            var poseLineList = _poseOutput.GetPoseLines(_image.Size.Width, _image.Size.Height);
            foreach (var postLine in poseLineList)
            {
                args.DrawingSession.DrawLine(postLine.StartVector2, postLine.EndVector2, Microsoft.UI.Colors.Green, 4);
            }
            foreach (var Landmark in _poseOutput?.PoseLandmarks?.Landmark)
            {

                var x = (int)_image.Size.Width * Landmark.X;
                var y = (int)_image.Size.Height * Landmark.Y;
                // Draw a point at (100, 100)
                args.DrawingSession.DrawCircle(x, y, 2, Microsoft.UI.Colors.Red, 2);
            }
        }
    }

    private async void Canvas_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        // Load the image from a file
        var path = Package.Current.InstalledLocation.Path + $"\\Assets\\pose.jpg";
        _image = await CanvasBitmap.LoadAsync(sender, path);
    }
}
