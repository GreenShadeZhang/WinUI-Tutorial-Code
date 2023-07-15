﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MediaPipe.PoseDetection.Extensions;
public class PoseLine3D
{
    public Vector3 StartVector3
    {
        get;
        set;
    }

    public Vector3 EndVector3
    {
        get;
        set;
    }
}
