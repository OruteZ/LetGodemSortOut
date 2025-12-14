namespace Utility
{
    // GameObjectPool.cs
    using System;
    using UnityEngine;
    using UnityEngine.Pool;


    /// <summary>
    /// Prefab(Component) 기반 GameObject 풀.
    /// </summary>
    public sealed class GameObjectPool<T> where T : Component
    {
        public IObjectPool<T> Pool => _pool;

        private readonly T _prefab;
        private readonly Transform _defaultParent;
        private readonly IObjectPool<T> _pool;

        public GameObjectPool(
            T prefab,
            Transform defaultParent = null,
            bool collectionCheck = true,
            int defaultCapacity = 16,
            int maxSize = 1024,
            int prewarmCount = 0)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));

            _prefab = prefab;
            _defaultParent = defaultParent;

            _pool = new ObjectPool<T>(
                createFunc: CreateInstance,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroyInstance,
                collectionCheck: collectionCheck,
                defaultCapacity: Mathf.Max(1, defaultCapacity),
                maxSize: Mathf.Max(1, maxSize)
            );

            if (prewarmCount > 0)
                Prewarm(prewarmCount);
        }

        // ---------------- Public API ----------------

        public T Get()
        {
            return _pool.Get();
        }

        public T Get(Vector3 position, Quaternion rotation, Transform parentOverride = null)
        {
            var item = _pool.Get();

            var tr = item.transform;
            if (parentOverride != null) tr.SetParent(parentOverride, worldPositionStays: false);

            tr.SetPositionAndRotation(position, rotation);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;
            _pool.Release(item);
        }

        public void Clear()
        {
            // 풀에 들어있는 오브젝트들을 모두 파괴(내부적으로 actionOnDestroy 호출)
            _pool.Clear();
        }
        
        /// <summary>
        /// GameObject 풀을 예열합니다. 
        /// </summary>
        /// <param name="count"></param>
        public void Prewarm(int count)
        {
            count = Mathf.Max(0, count);
            if (count == 0) return;

            // 임시로 꺼냈다가 바로 반납해서 내부 캐시 채우기
            T[] temp = new T[count];
            for (int i = 0; i < count; i++) temp[i] = _pool.Get();
            for (int i = 0; i < count; i++) _pool.Release(temp[i]);
        }

        // ---------------- Pool Hooks ----------------

        private T CreateInstance()
        {
            var inst = UnityEngine.Object.Instantiate(_prefab, _defaultParent);
            inst.gameObject.SetActive(false); // 기본은 비활성으로 만들어둠
            return inst;
        }

        private void OnGet(T item)
        {
            // 풀에서 꺼낼 때: 활성화 + 콜백
            item.gameObject.SetActive(true);

            if (item is IPoolable poolable)
                poolable.OnSpawned();
        }

        private void OnRelease(T item)
        {
            // 풀에 돌려줄 때: 콜백 + 비활성 + 부모 정리
            if (item is IPoolable poolable)
                poolable.OnDespawned();

            if (_defaultParent != null)
                item.transform.SetParent(_defaultParent, worldPositionStays: false);

            item.gameObject.SetActive(false);
        }

        private void OnDestroyInstance(T item)
        {
            if (item != null)
                UnityEngine.Object.Destroy(item.gameObject);
        }
    }

    public interface IPoolable
    {
        /// 풀에서 꺼내질 때(활성화 직전/직후 원하는 타이밍에 맞춰 사용)
        void OnSpawned();

        /// 풀로 반환될 때(비활성화 직전/직후 원하는 타이밍에 맞춰 사용)
        void OnDespawned();
    }
}