using UnityEngine;

[CreateAssetMenu(fileName = "Wand", menuName = "Weapon/Wand")]
public class Wand : Weapon
{
    public GameObject projectilePrefab;
    
    protected override void Attack()
    {
        Projectile projectile = Instantiate(projectilePrefab).GetComponent<Projectile>();
        if (projectile == null)
        {
            Debug.LogWarning("Wand Attack Failed: Projectile Prefab does not have a Projectile component.");
            return;
        }
        
        // Set projectile's damage
        projectile.Damage = damage.Value;
        
        // Set projectile's direction
        projectile.transform.position = handler.transform.position;
        
        // Set projectile's target
        projectile.SetDirection(handler.transform.forward);
        
        // Set Vector
    }
}