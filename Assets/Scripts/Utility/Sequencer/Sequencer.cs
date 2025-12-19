using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utility.Sequencer
{
    public sealed class Sequencer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MonoBehaviour clockBehaviour;
        [SerializeField] private ScriptableObject patternBehaviour;

        [Header("Playback")]
        // 기존 선언 유지(프로젝트에서 사용하는 형태에 맞게 유지)
        public UnityEvent<SeqEvent, double> onEvent;

        // Inspector에서 설정 가능하도록 readonly 제거
        private ITempoClock _clock;
        private ISequencePattern _pattern;

        [SerializeField]
        private double lookAhead = 0.1;

        // 버퍼를 재할당 가능하도록 readonly 제거
        private List<SeqEvent> _buf = new List<SeqEvent>(128);

        private long _startAbs;
        private long _scheduledUntilAbs;

        private void Awake()
        {
            // Inspector에 배치한 MonoBehaviour를 인터페이스로 캐스트
            if (clockBehaviour != null)
                _clock = clockBehaviour as ITempoClock;

            if (patternBehaviour != null)
                _pattern = patternBehaviour as ISequencePattern;

            if (_clock == null)
                Debug.LogWarning("Sequencer: clockBehaviour is not assigned or does not implement ITempoClock.");

            if (_pattern == null)
                Debug.LogWarning("Sequencer: patternBehaviour is not assigned or does not implement ISequencePattern.");
        }

        private void Update()
        {
            Tick();
        }

        public void StartNow()
        {
            if (_clock == null)
            {
                Debug.LogWarning("Sequencer: Cannot StartNow because clock is null.");
                return;
            }

            _startAbs = _clock.NowBeat;
            _scheduledUntilAbs = _startAbs;
        }

        public void Tick()
        {
            if (_clock == null || _pattern == null) return;

            // 스케줄된 마지막 지점까지 재생이 끝났으면 새로 스케줄링
            long targetAbs = _clock.BeatAtDspTime(_clock.DspTime + lookAhead);
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
            long loopLen = _pattern.LoopLengthBeats;
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
                    long eAbs = _startAbs + loop * loopLen + eLocal.Beat;
                    double dsp = _clock.DspTimeAtBeat(eAbs);

                    // 콜백 호출
                    onEvent?.Invoke(eLocal, dsp);
                }
            }
        }
    }
}