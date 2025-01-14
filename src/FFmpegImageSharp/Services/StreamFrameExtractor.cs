using FFmpeg.NET;
using FFmpegImageSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFmpegImageSharp.Services;

public class StreamFrameExtractor
{
    public async Task<List<FrameData>> ExtractFramesAsync(string filePath)
    {
        var frames = new List<FrameData>();
        var ffmpeg = new Engine("C:\\ffmpeg-n7.1-latest-win64-gpl-7.1\\ffmpeg-n7.1-latest-win64-gpl-7.1\\bin\\ffmpeg.exe"); // Specify the path to ffmpeg executable

        var mediaFile = new InputFile(filePath); // Use a concrete class instead of MediaFile
        var mediaInfo = await ffmpeg.GetMetaDataAsync(mediaFile, CancellationToken.None);
        var duration = mediaInfo.Duration;
        var frameRate = mediaInfo.VideoData.Fps;
        var frameCount = (int)(duration.TotalSeconds * frameRate);

        for (int i = 0; i < frameCount; i++)
        {
            using (var memoryStream = new MemoryStream())
            {
                var outputFilePath = $"pipe:1";
                var outputFile = new OutputFile(outputFilePath);

                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(i / frameRate) };
                await ffmpeg.GetThumbnailAsync(mediaFile, outputFile, options, CancellationToken.None);
                memoryStream.Position = 0;
                var frameImage = memoryStream.ToArray();
                var frameData = new FrameData
                {
                    ImageData = frameImage,
                    Timestamp = TimeSpan.FromSeconds(i / frameRate)
                };
                frames.Add(frameData);
            }
        }

        return frames;
    }
}
