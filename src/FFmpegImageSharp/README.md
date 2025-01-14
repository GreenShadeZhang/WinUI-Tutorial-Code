# dotnet-ffmpeg-imagesharp

This project demonstrates how to use FFmpeg.NET for frame extraction from MP4 files and ImageSharp for image processing in a .NET application. The application is designed to run on ARM64 architecture, making it suitable for devices like Raspberry Pi.

## Project Structure

```
dotnet-ffmpeg-imagesharp
├── src
│   ├── Program.cs          # Entry point of the application
│   ├── Services
│   │   ├── FrameExtractor.cs  # Extracts frames from MP4 files
│   │   └── ImageProcessor.cs   # Processes image data
│   └── Models
│       └── FrameData.cs       # Represents frame data structure
├── dotnet-ffmpeg-imagesharp.csproj  # Project file with dependencies
├── README.md                # Documentation for the project
└── .gitignore               # Files and directories to ignore by Git
```

## Setup Instructions

1. **Clone the repository:**
   ```
   git clone https://github.com/yourusername/dotnet-ffmpeg-imagesharp.git
   cd dotnet-ffmpeg-imagesharp
   ```

2. **Install dependencies:**
   Ensure you have the .NET SDK installed. You can install the required packages using the following command:
   ```
   dotnet restore
   ```

3. **Build the project:**
   ```
   dotnet build
   ```

4. **Run the application:**
   ```
   dotnet run --project src/dotnet-ffmpeg-imagesharp.csproj
   ```

## Usage Example

To extract frames from an MP4 file and process them, you can modify the `Program.cs` file to include your specific file path and processing logic.

## Dependencies

- **FFmpeg.NET**: Used for extracting frames from video files.
- **ImageSharp**: Used for processing image data.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.