using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
///  This is an enum of various possible weapon types.
///  It includes a "shield" type to allow shield PowerUp.
///  Items marked [NI] are Not Implemented just yet.
/// </summary>
public enum eWeaponType
{
    none,       // Default / no weapon
    blaster,    // Simple blaster
    spread,     // Multiple shots simultaneously
    phaser,     // [NI] Shows that move in waves
    missile,    // [NI] Homing missiles
    laser,      // [NI] Damage over time
    shield      // Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties 
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible. 
/// </summary> 

[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;

    [Tooltip("Letter to show on the PowerUp Cube")]
    public string letter;

    [Tooltip("Color of PowerUp Cube")]
    public Color powerUpColor = Color.white;

    [Tooltip("Prefab of Weapon model that is attached to Player Ship")]
    public GameObject weaponModelPrefab;

    [Tooltip("Prefab of projective that is fired")]
    public GameObject projectilePrefab;

    [Tooltip("Color of the projective that is fired")]
    public Color projectileColor = Color.white;

    [Tooltip("Damage caused when a single projectile hits an enemy")]
    public float damageOnHit = 0;

    [Tooltip("Damaage caused per second by the Laser [NI]")]
    public float damagePerSecond = 0;

    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;

    [Tooltip("Velocity of individual projectiles")]
    public float velocity = 50;
}
