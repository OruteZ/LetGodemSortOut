using System;
using System.Collections;
using System.Collections.Generic;
using OZ_Character;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.Sequencer;

public class WeaponHandler : MonoBehaviour
{
    // equipable 6 weapons
    [SerializeField] private Sequencer _weaponEventSequence;
    public List<Weapon> initialWeapons;

    [SerializeField]
    private List<Weapon> runtimeWeapons;
    public int maxWeapons = 6;

    [Header("Target Ref")] 
    private Character _nearestTarget;

    private void Awake()
    {
        // call setup
        foreach (Weapon weapon in initialWeapons)
        {
            AddWeapon(Instantiate(weapon));
        }
    }

    private void Start()
    {
        _weaponEventSequence.StartNow();
    }

    private void Update()
    {
        _weaponEventSequence.Tick();
        
        
        foreach(Weapon weapon in runtimeWeapons)
        {
            weapon.TryAttack();
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if (runtimeWeapons.Count < maxWeapons)
        {
            runtimeWeapons.Add(weapon);
            weapon.Setup(this);
        }
    }
}
