using UnityEngine;
using Utility.Pool;

[CreateAssetMenu(fileName = "Wand", menuName = "Weapon/Wand")]
public class Wand : Weapon
{
    public GameObject projectilePrefab;

    private GameObjectPool<Projectile> _projectilePool;

    protected override void OnSetup()
    {
        base.OnSetup();
        
        SetPool();
    }

    private void OnDestroy()
    {
        _projectilePool.Clear();
        _projectilePool = null;
    }
    
    private void SetPool()
    {
        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogWarning("Wand Initialization Failed: Projectile Prefab does not have a Projectile component.");
            return;
        }

        _projectilePool =
            new GameObjectPool<Projectile>(projectile, Handler.transform);
    }
    
    protected override void Attack()
    {
        Projectile projectile = _projectilePool.Get();
        if (projectile == null)
        {
            Debug.LogWarning("Wand Attack Failed: Projectile Prefab does not have a Projectile component.");
            return;
        }
        
        // Set projectile's damage
        projectile.Damage = damage.Value;
        
        // Set projectile's direction
        projectile.transform.position = Handler.transform.position;
        
        // Set projectile's target
        projectile.SetDirection(Handler.transform.forward);
        
        // Set Vector
    }
}