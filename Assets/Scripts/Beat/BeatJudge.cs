using System;

namespace BeatTemplate
{
    public static class BeatJudge
    {
        /// <summary>
        /// Evaluate the hit timing.
        /// </summary>
        /// <param name="tapDspTime">absolute DSP time at input</param>
        /// <param name="dspBeat0">absolute DSP time of beat-0 anchor</param>
        /// <param name="bpm"></param>
        /// <param name="subPerBeat"></param>
        /// <param name="windows"></param>
        /// <returns></returns>
        public static (HitGrade grade, double deltaSec) Evaluate(
            double tapDspTime, double dspBeat0, double bpm, int subPerBeat, HitWindows windows)
        {
            double tapSec = tapDspTime - dspBeat0; // shift to beat timeline (0 at beat-0)
            (double gridSec, double deltaSec) = Quantizer.NearestSubGrid(bpm, subPerBeat, tapSec);
            double a = Math.Abs(deltaSec);


            if (a <= windows.perfect) return (HitGrade.Perfect, deltaSec);
            if (a <= windows.great) return (HitGrade.Great, deltaSec);
            if (a <= windows.good) return (HitGrade.Good, deltaSec);
            return (HitGrade.Miss, deltaSec);
        }
    }
}