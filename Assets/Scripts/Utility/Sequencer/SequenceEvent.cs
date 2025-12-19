using UnityEngine;
using UnityEngine.Serialization;
using Utility.Attributes; // 추가: ReadOnly 어트리뷰트 네임스페이스

namespace Utility.Sequencer
{
    public enum EventKind : byte { Trigger, Note }
    
    [System.Serializable]
    public struct SeqEvent
    {
        [SerializeField, ReadOnly] private long _beat;  
        [SerializeField, ReadOnly] private EventKind _kind;     // 루프 내부 beat
        [SerializeField, ReadOnly] private int _midi;         // Note일 때 사용
        [SerializeField, ReadOnly] private int _lengthBeats;  // Note 길이
        [SerializeField, ReadOnly] private float _velocity;   // 공통(강약)
        [SerializeField, ReadOnly] private int _param;        // Trigger일 때 사용(샷 타입/드럼종류 등)
        
        #region Properties
        public int Param => _param;
        public long Beat => _beat;
        public int Midi => _midi;
        public float Velocity => _velocity;
        public EventKind Kind => _kind;
        public int LengthBeats => _lengthBeats;
        #endregion
        
        private SeqEvent(long beat, EventKind kind, int midi, int lenBeats, float vel, int param)
        { 
            _beat = beat; 
            _kind = kind;
            _midi = midi;
            _lengthBeats = lenBeats;
            _velocity = vel;
            _param = param;
        }

        public static SeqEvent Trigger(long beat, float vel=1f, int param=0)
            => new SeqEvent(beat, EventKind.Trigger, 0, 0, vel, param);

        public static SeqEvent Note(long beat, int midi, int lenBeats, float vel=1f, int param=0)
            => new SeqEvent(beat, EventKind.Note, midi, lenBeats, vel, param);
    }

}