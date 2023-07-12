using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MediaPipe.PoseDetection.Extensions;
public class PoseLine
{
    public Vector2 StartVector2
    {
        get;
        set;
    }

    public Vector2 EndVector2
    {
        get;
        set;
    }
}
