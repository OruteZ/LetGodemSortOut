using System.Collections.Generic;

namespace Utility.Sequencer
{
    public interface ITempoClock
    {
        double DspTime { get; }
        float Bpm { get; }
        int Ppq { get; }

        long NowTick { get; }
        long TickAtDspTime(double dspTime);
        double DspTimeAtTick(long tick);
    }


    public interface ISequencePattern
    {
        long LoopLengthTicks { get; }
        
        /// <summary>
        ///  fromTickInclusive 이상 toTickExclusive 미만 구간의 이벤트들을 results에 추가.
        /// </summary>
        /// <param name="fromTickInclusive"></param>
        /// <param name="toTickExclusive"></param>
        /// <param name="results"></param>
        void GetEventsInRange(long fromTickInclusive, long toTickExclusive, List<SeqEvent> results);
    }
}