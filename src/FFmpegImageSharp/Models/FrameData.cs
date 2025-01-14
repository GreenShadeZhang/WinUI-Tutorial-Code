using System;

namespace FFmpegImageSharp.Models;

public class FrameData
{
    public byte[] ImageData
    {
        get; set;
    }
    public TimeSpan Timestamp
    {
        get; set;
    }
}