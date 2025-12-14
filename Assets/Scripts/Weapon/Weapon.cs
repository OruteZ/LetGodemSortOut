using System;
using OZ_Character.Stats;
using UnityEngine;

[Serializable]
public abstract class Weapon : ScriptableObject
{
    public Stat damage;
    public Stat attackSpeed;

    protected WeaponHandler handler;

    private float _attackDelay;
    
    [SerializeField]
    private float _cooldown;
    
    public void Setup(WeaponHandler mHandler)
    {
        
        handler = mHandler;
        _attackDelay = 1 / attackSpeed.Value;
    }

    public bool TryAttack()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
            return false;
        }

        //atk code
        Attack();
        _cooldown = _attackDelay;

        return true;
    }

    protected abstract void Attack();

    #region EVENTS
    protected void OnStatChanged(Stat stat)
    {
        if (stat == attackSpeed)
        {
            _attackDelay = 1 / attackSpeed.Value;
        }
    }
    #endregion
}