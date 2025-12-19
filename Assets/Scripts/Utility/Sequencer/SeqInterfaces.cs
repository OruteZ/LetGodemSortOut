using System.Collections.Generic;

namespace Utility.Sequencer
{
    public interface ITempoClock
    {
        double DspTime { get; }
        float Bpm { get; }
        int Ppq { get; }

        long NowBeat { get; }
        long BeatAtDspTime(double dspTime);
        double DspTimeAtBeat(long beat);
    }


    public interface ISequencePattern
    {
        long LoopLengthBeats { get; }
        
        /// <summary>
        ///  fromBeatInclusive 이상 toBeatExclusive 미만 구간의 이벤트들을 results에 추가.
        /// </summary>
        /// <param name="fromBeatInclusive"></param>
        /// <param name="toBeatExclusive"></param>
        /// <param name="results"></param>
        void GetEventsInRange(long fromBeatInclusive, long toBeatExclusive, List<SeqEvent> results);
    }
}