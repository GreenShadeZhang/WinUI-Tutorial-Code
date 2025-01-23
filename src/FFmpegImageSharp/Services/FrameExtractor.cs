using FFmpeg.NET;
using FFmpegImageSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FFmpegImageSharp.Services;

public class FrameExtractor
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

        for (var i = 0; i < frameCount; i++)
        {
            var timestamp = TimeSpan.FromSeconds(i / frameRate);
            var outputFilePath = $"frame_{i}.jpg";
            var arguments = $"-i \"{filePath}\" -vf \"select='eq(n\\,{i})'\" -vsync vfr -q:v 2 \"{outputFilePath}\"";
            await ffmpeg.ExecuteAsync(arguments, CancellationToken.None);
            var frameImage = await File.ReadAllBytesAsync(outputFilePath);
            var frameData = new FrameData
            {
                ImageData = frameImage,
                Timestamp = timestamp
            };
            frames.Add(frameData);
        }

        return frames;
    }
}
