using System.Collections.Generic;
using UnityEngine;
using Utility.Attributes;

namespace Utility.Sequencer
{
    public class SeqPattern : ScriptableObject, ISequencePattern
    {
        [SerializeField, ReadOnly] private List<SeqEvent> events = new List<SeqEvent>();
        public IReadOnlyList<SeqEvent> Events => events;
        
        
        #region ISequencePattern implementation
        [SerializeField, ReadOnly] private long loopLengthBeats;
        public long LoopLengthBeats => loopLengthBeats;

        public void GetEventsInRange(long fromBeatInclusive, long toBeatExclusive, List<SeqEvent> results)
        {
            // ...existing code...
            // 내부적으로는 이제 private 'events'를 사용
        }
        #endregion
    }
}