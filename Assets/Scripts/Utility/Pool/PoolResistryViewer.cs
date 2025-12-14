namespace Utility.Pool
{
    using UnityEngine;

    /// <summary>
    /// 씬에 붙여두면, 커스텀 인스펙터가 PoolRegistry에 등록된 풀들을 표시해줌.
    /// </summary>
    public sealed class PoolRegistryBehaviour : MonoBehaviour
    {
        [SerializeField] private bool showOnlyInPlayMode = true;
        public bool ShowOnlyInPlayMode => showOnlyInPlayMode;
    }
}