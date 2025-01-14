using FFmpegImageSharp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;
using FFmpegImageSharp.Models;

var serviceProvider = new ServiceCollection()
            .AddSingleton<FrameExtractor>()
            .AddSingleton<StreamFrameExtractor>()
            .AddSingleton<ImageProcessor>()
            .BuildServiceProvider();

var frameExtractor = serviceProvider.GetService<FrameExtractor>();
var streamFrameExtractor = serviceProvider.GetService<StreamFrameExtractor>();
var imageProcessor = serviceProvider.GetService<ImageProcessor>();

string videoFilePath = "anger.mp4"; // Update with your video file path
List<FrameData> frames = await frameExtractor.ExtractFramesAsync(videoFilePath);

foreach (var frame in frames)
{
    imageProcessor.ProcessImage(frame);
}

Console.WriteLine("Frame extraction and processing completed.");