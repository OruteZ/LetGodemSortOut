using UnityEngine;
using OZ_Character.Interface;

public class Projectile : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float speed = 10f;
    [SerializeField] float damage = 1f;
    [SerializeField] float lifeTime = 2f;

    [Header("Hit Query")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float castRadius = 0.05f;   // 총알 두께(대충)
    [SerializeField] float skin = 0.02f;         // 살짝 여유

    float expireAt;
    Vector3 dir;
    Vector3 prevPos;
    
    #region Properties

    public float Damage
    {
        get => damage;
        set => damage = value;
    }
    #endregion

    void OnEnable()
    {
        expireAt = Time.time + lifeTime;
        prevPos = transform.position;
    }

    void Update()
    {
        if (Time.time >= expireAt)
        {
            Despawn();
            return;
        }

        // 1) 위치 갱신
        Vector3 curPos = transform.position;
        Vector3 nextPos = curPos + dir * (speed * Time.deltaTime);

        // 2) 이번 프레임 이동 구간만큼만 캐스트 (터널링 방지 + 불필요 검사 감소)
        Vector3 move = nextPos - prevPos;
        float dist = move.magnitude;

        if (dist > 0f)
        {
            Vector3 castDir = move / dist;

            if (Physics.SphereCast(prevPos, castRadius, castDir, out RaycastHit hit,
                    dist + skin, targetLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(damage);

                Despawn();
                return;
            }
        }

        transform.position = nextPos;
        prevPos = nextPos;
    }

    public void SetDirection(Vector3 direction)
    {
        dir = direction.normalized;
        transform.forward = dir;
    }

    public void SetTarget(Transform target)
    {
        SetDirection(target.position - transform.position);
    }

    void Despawn()
    {
        // 뱀서라이크면 Destroy 대신 풀링(비활성화) 권장
        gameObject.SetActive(false);
    }
}