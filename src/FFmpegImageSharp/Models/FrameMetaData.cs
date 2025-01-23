namespace FFmpegImageSharp.Models;
public class FrameMetaData
{
    public string Name { get; set; } = string.Empty;
    public string FileName
    {
        get; set;
    } = string.Empty;
    public int Width
    {
        get; set;
    }
    public int Height
    {
        get; set;
    }
    public List<byte[]> FrameDatas { get; set; } = [];
}
