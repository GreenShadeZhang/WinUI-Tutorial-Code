using FFmpegImageSharp.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FFmpegImageSharp.Services;

public class ImageProcessor
{
    public byte[] ProcessImage(FrameData frame)
    {
        using (var image = Image.Load(frame.ImageData))
        {
            // Resize the image to 240x240
            image.Mutate(x => x.Resize(240, 240));

            // Create a new 320x240 image with a custom background color
            using (var background = new Image<Bgra32>(320, 240, new Bgra32(0, 0, 0))) // Custom color: black
            {
                // Calculate the position to center the 240x240 image on the 320x240 background
                var x = (background.Width - image.Width) / 2;
                var y = (background.Height - image.Height) / 2;

                // Draw the resized image onto the background
                background.Mutate(ctx => ctx.DrawImage(image, new Point(x, y), 1f));
                background.Mutate(x => x.Rotate(90));
                using Image<Bgr24> converted2inch4Image = background.CloneAs<Bgr24>();
                var byteList = GetImageBytes(converted2inch4Image);
                return byteList;
                // Save the processed image or perform further processing
                //background.Save($"path_to_save_processed_image_{DateTime.Now.Ticks}.png");
            }
        }
    }

    public byte[] GetImageBytes(Image<Bgr24> image, int xStart = 0, int yStart = 0)
    {
        int imwidth = image.Width;
        int imheight = image.Height;
        var pix = new byte[imheight * imwidth * 2];
        for (int y = 0; y < imheight; y++)
        {
            for (int x = 0; x < imwidth; x++)
            {
                var color = image[x, y];
                pix[(y * imwidth + x) * 2] = (byte)((color.R & 0xF8) | (color.G >> 5));
                pix[(y * imwidth + x) * 2 + 1] = (byte)(((color.G << 3) & 0xE0) | (color.B >> 3));
            }
        }
        return pix;
    }
}
