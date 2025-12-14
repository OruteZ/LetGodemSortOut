using UnityEngine;
using UnityEngine.Serialization;


namespace BeatTemplate
{
    public enum HitGrade { Perfect, Great, Good, Miss }
    
    /// <summary>
    /// 특정한 타이밍이 비트와 얼마나 가까운지에 따라 판정을 내리는 기준을 정의합니다.
    /// 뭐 고전적 리듬게임에서 플레이어의 입력이 될 수도 있고, 다른 무언가일 수도 있고.
    /// </summary>
    [CreateAssetMenu(menuName = "BeatTemplate/HitWindows", fileName = "HitWindows")]
    public class HitWindows : ScriptableObject
    {
        [Header("Absolute offsets in seconds (|delta| <= threshold)")]
        [Range(0, 0.200f)] public float perfect = 0.030f;
        [Range(0, 0.300f)] public float great = 0.060f;
        [Range(0, 0.400f)] public float good = 0.090f;
    }
    
    
}