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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaPipe.PoseDetection;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private static PoseCpuSolution calculator =
        new (modelComplexity: 2, smoothLandmarks: false);

    public MainWindow()
    {
        this.InitializeComponent();
    }


    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\pose1.png");

        var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

        var dataMeta = mat2.Data;

        var length = mat2.Width * mat2.Height * mat2.Channels();

        var data = new byte[length];

        Marshal.Copy(dataMeta, data, 0, length);

        var widthStep = (int)mat2.Step();

        var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

        PoseOutput handsOutput = calculator.Compute(imgframe);

        if (handsOutput.PoseLandmarks != null)
        {
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
        MemoryStream ms = new MemoryStream();

        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        ms.Seek(0, SeekOrigin.Begin);

        // Create the decoder from the stream
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());

        // Get the SoftwareBitmap representation of the file
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        return softwareBitmap;
    }
}
