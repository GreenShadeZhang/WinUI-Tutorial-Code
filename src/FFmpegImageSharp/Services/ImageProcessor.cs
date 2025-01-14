using FFmpegImageSharp.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;

namespace FFmpegImageSharp.Services;

public class ImageProcessor
{
    public void ProcessImage(FrameData frame)
    {
        using (var image = Image.Load(frame.ImageData))
        {
            // Example transformation: Resize the image
            image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));

            // Save the processed image or perform further processing
            image.Save($"path_to_save_processed_image_{DateTime.Now.Ticks}.png");
        }
    }
}