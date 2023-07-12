using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Mediapipe.Net.Solutions;

namespace MediaPipe.PoseDetection.Extensions;
public static class PoseOutputExtensions
{
    public static List<PoseLine> GetPoseLines(this PoseOutput poseOutput,double x,double y)
    {
        var result = new List<PoseLine>();
        if (poseOutput is { PoseLandmarks.Landmark: not null })
        {
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[14].X * (float)x, poseOutput.PoseLandmarks.Landmark[14].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[16].X * (float)x, poseOutput.PoseLandmarks.Landmark[16].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[13].X * (float)x, poseOutput.PoseLandmarks.Landmark[13].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[15].X * (float)x, poseOutput.PoseLandmarks.Landmark[15].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[13].X * (float)x, poseOutput.PoseLandmarks.Landmark[13].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[14].X * (float)x, poseOutput.PoseLandmarks.Landmark[14].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[26].X * (float)x, poseOutput.PoseLandmarks.Landmark[26].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[25].X * (float)x, poseOutput.PoseLandmarks.Landmark[25].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[26].X * (float)x, poseOutput.PoseLandmarks.Landmark[26].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[28].X * (float)x, poseOutput.PoseLandmarks.Landmark[28].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[25].X * (float)x, poseOutput.PoseLandmarks.Landmark[25].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[27].X * (float)x, poseOutput.PoseLandmarks.Landmark[27].Y * (float)y)
            });
        }
        return result;
    }
}
