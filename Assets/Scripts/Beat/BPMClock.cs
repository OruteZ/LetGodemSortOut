using System;
using System.Collections;
using UnityEngine;

namespace BeatTemplate
{
    public enum BeatState
    {
        BeforeLoad, Loading, Loaded,
        Playing, Paused, Finished
    }

    public class BpmClock : MonoBehaviour
    {
        [Space(10)]
        [SerializeField, Range(0f, 0.49f)] private float inputLeadSub = 0.25f;
        [SerializeField, Min(1)] private int subPerBeat = 4;
        [SerializeField, Range(0.01f, 0.2f)] private double safetyLeadSec = 0.05;
        
        [Space(10)]
        [SerializeField] private bool loopTimeline = false;
        [SerializeField, Min(0)] private int loopBeats = 0; // 0이면 비루프, >0이면 해당 비트길이로 모듈러

        [Header("Data")]
        [SerializeField] private int bpm = 120;

        // runtime
        private double _dspAudioStart; // 오디오 시작 시각
        private double _dspBeat0;      // 메인 비트 0 시각(앵커)
        private BeatState _beatState = BeatState.BeforeLoad;
        
        // 누적 정지 시간(초)과 이번 정지의 시작 DSP 시각
        private double _pausedAccumSec = 0.0;   // NEW
        private double _pauseDspStart  = 0.0;   // NEW

        public double ElapsedSec
        {
            get
            {
                if (_beatState is not (BeatState.Playing or BeatState.Paused)) return 0.0;

                // 현재 DSP 시각. Paused일 땐 멈춘 순간의 DSP로 고정해 정지 중에도 값이 안 늘어나게 함
                double nowDsp = (_beatState == BeatState.Paused)
                    ? _pauseDspStart                      // 정지 시작 시각에 고정
                    : AudioSettings.dspTime;

                // 비트0 앵커로부터 경과시간에서 "누적 정지 시간"을 빼서 연속성 유지
                double elapsed = nowDsp - _dspBeat0 - _pausedAccumSec; 
                return Math.Max(0.0, elapsed);
            }
        }
        public int CurrentBeat
        {
            get
            {
                (int beat, int sb) = Quantizer.Quantize(bpm, ElapsedSec, subPerBeat, 0.0);
                return beat;
            }
        }
        public int CurrentSubBeat
        {
            get
            {
                (int beat, int sb) = Quantizer.Quantize(bpm, ElapsedSec, subPerBeat, 0.0);
                return sb;
            }
        }
        public float BeatProgress => (float)(ElapsedSec / (60.0 / (double)bpm)) - CurrentBeat;
        public BeatState State => _beatState;
        public double DspAudioStart => _dspAudioStart;
        public double DspBeat0 => _dspBeat0;

        // 입력 판정용(서브 리드 적용)
        public (int beat, int sub) InputQuantizedNow()
        {
            return Quantizer.Quantize(bpm, ElapsedSec, subPerBeat, inputLeadSub);
        }
        
        [Button("Init")]
        public void Init()
        {
            _beatState = BeatState.Loaded;
        }

        [Button("Play")]
        public void Play()
        {
            switch (_beatState)
            {
                case BeatState.BeforeLoad: 
                    Debug.LogWarning("BeatSystem: Play() called before Init(). Ignored."); 
                    return;
                case BeatState.Loaded: StartMusic(); return;
                case BeatState.Paused: 
                    _pausedAccumSec += AudioSettings.dspTime - _pauseDspStart;
                    _pauseDspStart = 0.0;

                    _beatState = BeatState.Playing;
                    return;
                default: return;
            }
        }

        [Button("Pause")]
        public void Pause()
        {
            if (_beatState == BeatState.Playing)
            {
                _pauseDspStart = AudioSettings.dspTime; // NEW: 정지 시작 시각 기록
                _beatState = BeatState.Paused;
            }
        }

        [Button("Stop")]
        public void Stop()
        {
            if (_beatState is BeatState.Playing or BeatState.Paused)
            {
                _beatState = BeatState.Finished;
            }
        }

        #region PRIVATE

        private void StartMusic()
        {
            double dspNow = AudioSettings.dspTime;
            _dspAudioStart   = dspNow + safetyLeadSec; // 오디오 시작
            _dspBeat0 = _dspAudioStart; //+ track.Offset; // 비트0 앵커

            _pausedAccumSec  = 0.0; 
            _pauseDspStart   = 0.0; 

            // audioSource.PlayScheduled(_dspAudioStart);
            _beatState = BeatState.Playing;
        }

        #endregion
    }
}
