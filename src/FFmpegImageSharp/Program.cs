using System.Text.Json;
using FFmpegImageSharp.Models;
using FFmpegImageSharp.Services;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
            .AddSingleton<FrameExtractor>()
            .AddSingleton<StreamFrameExtractor>()
            .AddSingleton<ImageProcessor>()
            .BuildServiceProvider();

var frameExtractor = serviceProvider.GetRequiredService<FrameExtractor>();
//var streamFrameExtractor = serviceProvider.GetRequiredService<StreamFrameExtractor>();
var imageProcessor = serviceProvider.GetRequiredService<ImageProcessor>();

var videoFilePath = "anger.mp4"; // Update with your video file path
var data = new FrameMetaData
{
    Name = Path.GetFileNameWithoutExtension(videoFilePath),
    FileName = videoFilePath,
    Width = 240,
    Height = 320
};
var frames = await frameExtractor.ExtractFramesAsync(videoFilePath);
foreach (var frame in frames)
{
    var list = imageProcessor.ProcessImage(frame);
    data.FrameDatas.Add(list);
}
// JSON serialization
await File.WriteAllTextAsync($"{data.Name}.json", JsonSerializer.Serialize(data));
// JSON deserialization
var deserializedData = JsonSerializer.Deserialize<FrameMetaData>(await File.ReadAllTextAsync($"{data.Name}.json"));

// Verify deserialization
Console.WriteLine($"Name: {deserializedData?.Name}, Width: {deserializedData?.Width}, Height: {deserializedData?.Height}");
Console.WriteLine("Frame extraction and processing completed. Metadata saved to frame_metadata.json.");