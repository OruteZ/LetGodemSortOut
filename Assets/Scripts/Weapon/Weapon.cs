using System;
using OZ_Character.Stats;
using UnityEngine;

[Serializable]
public abstract class Weapon : ScriptableObject
{
    public Stat damage;
    public Stat attackSpeed;

    private WeaponHandler _handler;
    protected WeaponHandler Handler => _handler;

    private float _attackDelay;
    
    [SerializeField]
    private float _cooldown;
    
    public void Setup(WeaponHandler mHandler)
    {
        _handler = mHandler;
        _attackDelay = 1 / attackSpeed.Value;
        
        OnSetup();
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

    protected virtual void OnSetup() { }

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