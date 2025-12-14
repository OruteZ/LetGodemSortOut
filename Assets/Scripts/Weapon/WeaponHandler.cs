using System;
using System.Collections;
using System.Collections.Generic;
using OZ_Character;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponHandler : MonoBehaviour
{
    // equipable 6 weapons
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

    private void Update()
    {
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
