using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Item/Create New Weapon")]
public class WeaponData : ScriptableObject
{
    public Sprite weaponSprite;
    public string weaponName;

    public enum WeaponType
    {
        Melle,
        Pistol,
        SMG,
        AR
    }

    public enum ShootingType
    {
        Melle,
        Single,
        AutomaticFire,
        Burst
    }

    public WeaponType weaponType;
    public ShootingType shootingType;

    [Header("Stats")]
    public float delay;
    public int bulletCount;

    public WeaponData(string weaponName, float delay, int bulletCount)
    {
        this.weaponName = weaponName;
        this.delay = delay;
        this.bulletCount = bulletCount;
    }
}
