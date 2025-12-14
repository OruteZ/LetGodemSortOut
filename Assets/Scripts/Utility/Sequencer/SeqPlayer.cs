namespace Utility.Sequencer
{
    public sealed class SeqPlayer
    {
        private readonly ITempoClock _clock;
        private readonly ISequencePattern _pattern;
        private readonly System.Action<SeqEvent, double> _onEvent; // <= Sink 대신 콜백

        private readonly double _lookAhead;
        private readonly System.Collections.Generic.List<SeqEvent> _buf = new(128);

        private long _startAbs;
        private long _scheduledUntilAbs;

        public SeqPlayer(
            ITempoClock clock, 
            ISequencePattern pattern,
            System.Action<SeqEvent, double> onEvent,
            double lookAheadSeconds = 0.15)
        {
            _clock = clock;
            _pattern = pattern;
            _onEvent = onEvent;
            _lookAhead = lookAheadSeconds;
        }

        public void StartNow()
        {
            _startAbs = _clock.NowTick;
            _scheduledUntilAbs = _startAbs;
        }

        public void Tick()
        {
            // 스케줄된 마지막 지점까지 재생이 끝났으면 새로 스케줄링
            long targetAbs = _clock.TickAtDspTime(_clock.DspTime + _lookAhead);
            if (targetAbs <= _scheduledUntilAbs) return;
            
            // 새로 스케줄링
            ScheduleRange(_scheduledUntilAbs, targetAbs);
            _scheduledUntilAbs = targetAbs;
        }
        
        /// <summary>
        ///  fromAbs 이상 toAbs 미만 구간의 이벤트들을 스케줄링.
        /// </summary>
        /// <param name="fromAbs"></param>
        /// <param name="toAbs"></param>
        private void ScheduleRange(long fromAbs, long toAbs)
        {
            long loopLen = _pattern.LoopLengthTicks;
            if (loopLen <= 0) return;

            long fromRel = fromAbs - _startAbs;
            long toRel = toAbs - _startAbs;

            long loopFrom = fromRel / loopLen;
            long loopTo = (toRel - 1) / loopLen;

            for (long loop = loopFrom; loop <= loopTo; loop++)
            {
                long segFrom = (loop == loopFrom) ? (fromRel - loop * loopLen) : 0;
                long segTo = (loop == loopTo) ? (toRel - loop * loopLen) : loopLen;

                _buf.Clear();
                _pattern.GetEventsInRange(segFrom, segTo, _buf);

                foreach (SeqEvent eLocal in _buf)
                {
                    long eAbs = _startAbs + loop * loopLen + eLocal.Tick;
                    double dsp = _clock.DspTimeAtTick(eAbs);
                    
                    /// 콜백 호출
                    _onEvent?.Invoke(eLocal, dsp);
                }
            }
        }
    }
}