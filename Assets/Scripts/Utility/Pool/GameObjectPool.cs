using Unity.VisualScripting;

namespace Utility.Pool
{
    using System;
    using UnityEngine;
    using UnityEngine.Pool;

    /// <summary>Inspector/Editor에서 풀 상태를 표시하기 위한 디버그 인터페이스</summary>
    public interface IPoolDebugInfo
    {
        UnityEngine.Object PrefabObject { get; }
        GameObject PrefabGameObject { get; }
        Transform DefaultParent { get; }

        string PoolTypeName { get; }
        int CountAll { get; }
        int CountActive { get; }
        int CountInactive { get; }
        int MaxSize { get; }
        int DefaultCapacity { get; }
        bool CollectionCheck { get; }
    }

    /// <summary>
    /// Prefab(Component) 기반 GameObject 풀.
    /// </summary>
    public sealed class GameObjectPool<T> : IPoolDebugInfo where T : Component
    {
        // IObjectPool로도 쓰고 싶을 수 있으니 노출은 인터페이스로
        public IObjectPool<T> Pool => _pool;

        private readonly T _prefab;
        private readonly Transform _defaultParent;

        private readonly ObjectPool<T> _pool;

        private readonly int _maxSize;
        private readonly int _defaultCapacity;
        private readonly bool _collectionCheck;

        private bool _suppressCallbacks; // Prewarm 중 콜백/활성화를 막고 싶을 때

        public GameObjectPool(
            T prefab,
            Transform defaultParent = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 50,
            int prewarmCount = 10)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));

            _prefab = prefab;
            _defaultParent = defaultParent;

            _collectionCheck = collectionCheck;
            _defaultCapacity = Mathf.Max(1, defaultCapacity);
            _maxSize = Mathf.Max(1, maxSize);

            _pool = new ObjectPool<T>(
                createFunc: CreateInstance,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyInstance,
                collectionCheck: _collectionCheck,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );

            PoolRegistry.Register(this);

            if (prewarmCount > 0)
                Prewarm(prewarmCount);
        }

        public void Dispose()
        {
            PoolRegistry.Unregister(this);
            _pool.Clear();
        }

        // ---------------- Public API ----------------

        public T Get() => _pool.Get();

        public T Get(Vector3 position, Quaternion rotation, Transform parentOverride = null)
        {
            var item = _pool.Get();

            var tr = item.transform;
            if (parentOverride != null)
                tr.SetParent(parentOverride, worldPositionStays: false);

            tr.SetPositionAndRotation(position, rotation);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;
            _pool.Release(item);
        }

        public void Clear() => _pool.Clear();

        /// <summary>GameObject 풀을 예열합니다.</summary>
        public void Prewarm(int count)
        {
            count = Mathf.Max(0, count);
            if (count == 0) return;

            _suppressCallbacks = true;

            T[] temp = new T[count];
            for (int i = 0; i < count; i++) temp[i] = _pool.Get();
            for (int i = 0; i < count; i++) _pool.Release(temp[i]);

            _suppressCallbacks = false;
        }

        // ---------------- Pool Hooks ----------------

        private T CreateInstance()
        {
            var inst = UnityEngine.Object.Instantiate(_prefab, _defaultParent);
            if (inst is IPoolable poolable)
                poolable.SetPool(this);
            inst.gameObject.SetActive(false);
            return inst;
        }

        private void OnGet(T item)
        {
            if (!_suppressCallbacks)
            {
                if (item.gameObject == null)
                    throw new InvalidOperationException("풀에서 꺼낸 아이템이 null입니다.");
                item.gameObject.SetActive(true);

                if (item is IPoolable poolable)
                    poolable.OnSpawned();
            }
            else
            {
                // Prewarm 중에는 활성화/콜백 없이 비활성 유지
                item.gameObject.SetActive(false);
            }
        }

        private void OnRelease(T item)
        {
            if (!_suppressCallbacks)
            {
                if (item is IPoolable poolable)
                    poolable.OnDespawned();
            }

            if (_defaultParent != null)
                item.transform.SetParent(_defaultParent, worldPositionStays: false);

            item.gameObject.SetActive(false);
        }

        private void OnDestroyInstance(T item)
        {
            if (item != null)
                UnityEngine.Object.Destroy(item.gameObject);
        }

        // ---------------- Debug Info (for Inspector) ----------------

        public UnityEngine.Object PrefabObject => _prefab;
        public GameObject PrefabGameObject => _prefab != null ? _prefab.gameObject : null;
        public Transform DefaultParent => _defaultParent;

        public string PoolTypeName => typeof(T).Name;
        public int CountAll => _pool.CountAll;
        public int CountActive => _pool.CountActive;
        public int CountInactive => _pool.CountInactive;
        public int MaxSize => _maxSize;
        public int DefaultCapacity => _defaultCapacity;
        public bool CollectionCheck => _collectionCheck;
    }

    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
        
        void SetPool<T>(GameObjectPool<T> mPool) where T : Component;
    }
}
