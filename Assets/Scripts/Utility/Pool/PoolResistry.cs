namespace Utility.Pool
{
    using System.Collections.Generic;

    public static class PoolRegistry
    {
        private static readonly List<IPoolDebugInfo> _pools = new();

        public static IReadOnlyList<IPoolDebugInfo> Pools => _pools;

        public static void Register(IPoolDebugInfo pool)
        {
            if (pool == null) return;
            if (!_pools.Contains(pool)) _pools.Add(pool);
        }

        public static void Unregister(IPoolDebugInfo pool)
        {
            if (pool == null) return;
            _pools.Remove(pool);
        }

        public static void Clear() => _pools.Clear();
    }
}