using UnityEngine;
using OZ_Character.Interface;
using Utility.Pool;

public class Projectile : MonoBehaviour, IPoolable
{
    [Header("Stats")]
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 1f;
    [SerializeField] float lifeTime = 2f;

    [Header("Hit Query")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float castRadius = 0.05f;   // 총알 두께(대충)
    [SerializeField] float skin = 0.02f;         // 살짝 여유

    private float _expireAt;
    private Vector3 _dir;
    private Vector3 _prevPos;
    private GameObjectPool<Projectile> _pool;
    
    #region Properties

    public float Damage
    {
        get => damage;
        set => damage = value;
    }
    #endregion

    void OnEnable()
    {
        _expireAt = Time.time + lifeTime;
        _prevPos = transform.position;
    }

    void Update()
    {
        if (Time.time >= _expireAt)
        {
            if (_pool != null)
                _pool.Release(this);
            else
                Destroy(gameObject);
            return;
        }

        // 1) 위치 갱신
        Vector3 curPos = transform.position;
        Vector3 nextPos = curPos + _dir * (speed * Time.deltaTime);

        // 2) 이번 프레임 이동 구간만큼만 캐스트 (터널링 방지 + 불필요 검사 감소)
        Vector3 move = nextPos - _prevPos;
        float dist = move.magnitude;

        if (dist > 0f)
        {
            Vector3 castDir = move / dist;

            if (Physics.SphereCast(
                    _prevPos, castRadius, castDir, out RaycastHit hit,
                    dist + skin, targetLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(damage);
                
                if (_pool != null)
                    _pool.Release(this);
                else
                    Destroy(gameObject);
                return;
            }
        }

        transform.position = nextPos;
        _prevPos = nextPos;
    }

    public void SetDirection(Vector3 direction)
    {
        _dir = direction.normalized;
        transform.forward = _dir;
    }

    public void SetTarget(Transform target)
    {
        SetDirection(target.position - transform.position);
    }
    
    #region IPoolable Implementation
    public void OnSpawned()
    {
    }

    public void OnDespawned()
    {
    }

    public void SetPool<T>(GameObjectPool<T> mPool) where T : Component
    {
        _pool = mPool as GameObjectPool<Projectile>;
    }
    #endregion
    
    #if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, castRadius);
    }
    #endif
}