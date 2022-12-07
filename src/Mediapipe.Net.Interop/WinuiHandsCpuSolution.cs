using Mediapipe.Net.Solutions;

namespace Mediapipe.Net.Interop;
public class WinuiHandsCpuSolution : HandsCpuSolution
{
    public WinuiHandsCpuSolution(string graphBasePath, bool staticImageMode = false, int maxNumHands = 2, int modelComplexity = 1) : base(graphBasePath, staticImageMode, maxNumHands, modelComplexity)
    {
    }
}
