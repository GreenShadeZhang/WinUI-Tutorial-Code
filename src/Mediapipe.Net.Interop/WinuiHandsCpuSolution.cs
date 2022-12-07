using Mediapipe.Net.Solutions;

namespace Mediapipe.Net.Interop;
public class WinuiHandsCpuSolution : HandsCpuSolution
{
    public WinuiHandsCpuSolution(bool staticImageMode = false, int maxNumHands = 2, int modelComplexity = 1) : base(staticImageMode, maxNumHands, modelComplexity)
    {
    }
}
