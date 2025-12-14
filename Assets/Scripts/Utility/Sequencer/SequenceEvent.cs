namespace Utility.Sequencer
{
    public enum EventKind : byte { Trigger, Note }

    public readonly struct SeqEvent
    {
        public readonly long Tick;        // 루프 내부 tick
        public readonly EventKind Kind;

        public readonly int Midi;         // Note일 때 사용
        public readonly int LengthTicks;  // Note 길이
        public readonly float Velocity;   // 공통(강약)
        public readonly int Param;        // Trigger일 때 사용(샷 타입/드럼종류 등)

        private SeqEvent(long tick, EventKind kind, int midi, int len, float vel, int param)
        { 
            Tick=tick; 
            Kind=kind;
            Midi=midi;
            LengthTicks=len;
            Velocity=vel;
            Param=param;
        }

        public static SeqEvent Trigger(long tick, float vel=1f, int param=0)
            => new SeqEvent(tick, EventKind.Trigger, 0, 0, vel, param);

        public static SeqEvent Note(long tick, int midi, int lenTicks, float vel=1f, int param=0)
            => new SeqEvent(tick, EventKind.Note, midi, lenTicks, vel, param);
    }

}