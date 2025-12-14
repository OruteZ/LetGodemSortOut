using OZ_Character.Interface;
using OZ_Character.Stats;
using UnityEngine;

namespace OZ_Character
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        public Stat Hp => hp;
        public Stat MaxHp => maxHp;
        public Stat Atk => atk;
        public Stat Def => def;
        
        public virtual void TakeDamage(float dmg)
        {
            float finalDmg = Utility.CombatUtil.ApplyDef(dmg, Def.Value);
            
            Hp.baseValue -= finalDmg;
            Hp.baseValue = Mathf.Clamp(Hp.baseValue, 0, MaxHp.Value);
        }
        
        [SerializeField] private Stat hp;
        [SerializeField] private Stat maxHp;
        [SerializeField] private Stat atk;
        [SerializeField] private Stat def;
    }
}