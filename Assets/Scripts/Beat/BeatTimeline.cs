using System;
using UnityEngine;


namespace BeatTemplate
{
    /// <summary>
    /// 비트와 서브비트가 변경될 때마다 이벤트를 발생시키는 컴포넌트
    /// </summary>
    public class BeatTimeline : MonoBehaviour
    {
        [SerializeField] private BpmClock beat;
        [SerializeField] private bool emitSubBeats = true;


        public event Action<int> OnBeat; // (beat)
        public event Action<int, int> OnSubBeat; // (beat, sub)


        int _lastBeat = -1;
        (int beat, int sub) _lastSub = (-1, -1);


        private void Update()
        {
            if (beat == null) return;
            if (beat.State is not (BeatState.Playing or BeatState.Paused)) return;


            int b = beat.CurrentBeat;
            int s = beat.CurrentSubBeat;


            if (b != _lastBeat)
            {
                _lastBeat = b;
                OnBeat?.Invoke(b);
            }


            if (emitSubBeats && (b != _lastSub.beat || s != _lastSub.sub))
            {
                _lastSub = (b, s);
                OnSubBeat?.Invoke(b, s);
            }
        }
    }
}